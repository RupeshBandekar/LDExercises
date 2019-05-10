using System;
using System.ServiceModel.Channels;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventStoreSaleExercise
{
    public class DirectorReadModel
    {
        public int? Checkpoint { get; private set; }
        private string _streamName;
        private IEventStoreConnection conn = null;
        private int _totalItemsSold;
        private decimal _totalSales;

        public DirectorReadModel(string streamName)
        {
            int? checkpoint = null;
            conn = Globals.CreateConnection();
            _streamName = streamName;
            Checkpoint = checkpoint;
            Subscribe();
        }

        private void Subscribe()
        {
            conn.SubscribeToStreamFrom(_streamName, Checkpoint, false, (s, e) => ReceivedEvent(s, e), subscriptionDropped: Dropped);
        }

        private void ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt)
        {
            try
            {
                var checkpoint = evt.Event.EventNumber;
                
                if (evt.Event.EventType == Globals.SaleAddedEvent)
                {
                    Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(evt.Event.Data));
                    _totalItemsSold += objSales.Quantity;
                    _totalSales += objSales.Price;
                    Console.WriteLine("Total items sold".PadRight(20,' ') + "|" + "Total sales($)".PadRight(20,' ') + "|");
                    Console.WriteLine(_totalItemsSold.ToString().PadRight(20, ' ') + "|" + _totalSales.ToString().PadRight(20, ' ') + "|");
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
