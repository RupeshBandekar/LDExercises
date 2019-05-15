namespace EventStoreSaleExercise
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    public class InventoryManagerReadModel : IReadModel
    {
        public int? Checkpoint { get; private set; }
        private string _streamName;
        private IEventStoreConnection conn = null;
        private IDictionary<string, int> _dictSoldItems;
        public InventoryManagerReadModel(string streamName)
        {
            conn = Globals.CreateConnection();
            _streamName = streamName;
            Checkpoint = null;
            _dictSoldItems = new Dictionary<string, int>();
            ReadAllSaleAddedEvents();
        }

        public void Subscribe()
        {
            conn.SubscribeToStreamFrom(_streamName, Checkpoint, false, (s,e) => ReceivedEvent(s,e), subscriptionDropped: Dropped);
        }

        public void ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt)
        {
            try
            {
                Console.WriteLine("Fetching updated sales info:");
                var checkpoint = evt.Event.EventNumber;
                if (evt.Event.EventType == Globals.SaleAddedEvent)
                {
                    Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(evt.Event.Data));
                    if (_dictSoldItems.ContainsKey(objSales.ProductName))
                    {
                        _dictSoldItems[objSales.ProductName] += objSales.Quantity;
                    }
                    else
                    {
                        _dictSoldItems.Add(objSales.ProductName, objSales.Quantity);
                    }

                    foreach (var item in _dictSoldItems)
                    {
                        //Console.WriteLine("|" + item.Key.PadRight(20, ' ') + "|" +
                        //                  item.Value.ToString().PadRight(10, ' ') + "|");
                        Console.WriteLine($"|{item.Key.PadRight(20, ' ')}|{item.Value.ToString().PadRight(10, ' ')}|");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Event exception: " + ex.Message);
            }
        }

        public void ReadAllSaleAddedEvents()
        {
            var slice = 10;
            var checkpoint = 0;
            _dictSoldItems = new Dictionary<string, int>();

            while (true)
            {
                var eventSlice = conn.ReadStreamEventsForwardAsync(_streamName, checkpoint, slice, true).Result;
                
                foreach (var _event in eventSlice.Events)
                {
                    Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(_event.Event.Data));
                    if (_dictSoldItems.ContainsKey(objSales.ProductName))
                    {
                        _dictSoldItems[objSales.ProductName] += objSales.Quantity;
                    }
                    else
                    {
                        _dictSoldItems.Add(objSales.ProductName, objSales.Quantity);
                    }

                    checkpoint = (int) _event.Event.EventNumber + 1;
                }

                if (eventSlice.IsEndOfStream)
                    break;
            }

            foreach (var item in _dictSoldItems)
            {
                //Console.WriteLine("|" + item.Key.PadRight(20, ' ') + "|" +
                //                  item.Value.ToString().PadRight(10, ' ') + "|");
                Console.WriteLine($"|{item.Key.PadRight(20, ' ')}|{item.Value.ToString().PadRight(10, ' ')}|");
            }

            if (checkpoint > 0)
                Checkpoint = checkpoint - 1;

            Subscribe();
        }

        public void Dropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
        {
            Console.WriteLine("Subscription dropped, please enter to reconnect.");
            Subscribe();
        }
    }
}
