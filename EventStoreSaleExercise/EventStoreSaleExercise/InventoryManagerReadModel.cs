using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventStoreSaleExercise
{
    public class InventoryManagerReadModel
    {
        public int? Checkpoint { get; private set; }
        private string _streamName;
        private IEventStoreConnection conn = null;
        //private IDictionary<string, int> _dictSoldItems;
        public InventoryManagerReadModel(string streamName)
        {
            int? checkpoint = null;
            conn = Globals.CreateConnection();
            _streamName = streamName;
            Checkpoint = checkpoint;
            Subscribe();
            //_dictSoldItems = new Dictionary<string, int>();
        }

        private void Subscribe()
        {
            conn.SubscribeToStreamFrom(_streamName, Checkpoint, false, (s,e) => ReceivedEvent(s,e), subscriptionDropped: Dropped);
        }

        private void ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt)
        {
            try
            {
                var checkpoint = evt.Event.EventNumber;
                if (evt.Event.EventType == Globals.SaleAddedEvent)
                {
                    Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(evt.Event.Data));
                    //if (_dictSoldItems.ContainsKey(objSales.ProductName))
                    //{
                    //    _dictSoldItems[objSales.ProductName] += objSales.Quantity;
                    //}

                    Console.WriteLine(objSales.Id.ToString().PadRight(38, ' ') + "|" +
                                      objSales.ProductName.PadRight(20, ' ') + "|" +
                                      objSales.Quantity.ToString().PadRight(10, ' ') + "|" +
                                      objSales.Price.ToString().PadRight(10, ' ') + "|");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Event exception: " + ex.Message);
            }
        }
        
        private void Dropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
        {
            Console.WriteLine("Subscription dropped, please enter to reconnect.");
            Subscribe();
        }
    }
}
