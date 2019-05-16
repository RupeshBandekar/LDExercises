namespace EventStoreSaleExercise
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    public class InventoryManager : IInventoryManagerReadModel
    {
        public int? Checkpoint { get; private set; }
        private string _streamName;
        private Dictionary<string, int> _dictSoldItems;
        public InventoryManager(string streamName)
        {
            int checkpoint = 0;
            _streamName = streamName;
            _dictSoldItems = new Dictionary<string, int>();
            
            Checkpoint = checkpoint;
        }
        
        private void Subscribe()
        {
            if (Checkpoint == 0)
                Checkpoint = null;

            EventStoreSetup.conn.SubscribeToStreamFrom(_streamName, lastCheckpoint: Checkpoint, resolveLinkTos: false, eventAppeared: (s, e) => ReceivedEvent(s, e),
                subscriptionDropped: Dropped);
        }

        private void ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt)
        {
            try
            {
                if (evt.Event.EventType == EventStoreSetup.SaleAddedEvent)
                {
                    var receivedEvent = new List<RecordedEvent> { evt.Event };
                    PrintProductNameQuantity(GetProductNameQuantityList(receivedEvent));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Event exception: " + ex.Message);
            }
        }

        //public void ReadAllSaleAddedEvents()
        //{
        //    var slice = 10;
        //    var checkpoint = 0;
        //    _dictSoldItems = new Dictionary<string, int>();

        //    while (true)
        //    {
        //        var eventSlice = conn.ReadStreamEventsForwardAsync(_streamName, checkpoint, slice, true).Result;

        //        foreach (var _event in eventSlice.Events)
        //        {
        //            Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(_event.Event.Data));
        //            if (_dictSoldItems.ContainsKey(objSales.ProductName))
        //            {
        //                _dictSoldItems[objSales.ProductName] += objSales.Quantity;
        //            }
        //            else
        //            {
        //                _dictSoldItems.Add(objSales.ProductName, objSales.Quantity);
        //            }

        //            checkpoint = (int) _event.Event.EventNumber + 1;
        //        }

        //        if (eventSlice.IsEndOfStream)
        //            break;
        //    }

        //    foreach (var item in _dictSoldItems)
        //    {
        //        //Console.WriteLine("|" + item.Key.PadRight(20, ' ') + "|" +
        //        //                  item.Value.ToString().PadRight(10, ' ') + "|");
        //        Console.WriteLine($"|{item.Key.PadRight(20, ' ')}|{item.Value.ToString().PadRight(10, ' ')}|");
        //    }

        //    if (checkpoint > 0)
        //        Checkpoint = checkpoint - 1;

        //    Subscribe();
        //}

        public List<EventStore.ClientAPI.RecordedEvent> ReadEventsFromStream(IEventStoreConnection conn,
            string streamName, ref int checkpoint, int slice)
        {
            List<EventStore.ClientAPI.RecordedEvent> recordedEvents = new List<RecordedEvent>();
            while (true)
            {
                var eventSlice = conn.ReadStreamEventsForwardAsync(streamName, checkpoint, slice, true).Result;

                foreach (var _event in eventSlice.Events)
                {
                    recordedEvents.Add(_event.Event);
                    checkpoint = (int)_event.Event.EventNumber + 1;
                }

                slice = (int)eventSlice.LastEventNumber;

                if (eventSlice.IsEndOfStream)
                    break;
            }

            if (checkpoint > 0)
                checkpoint = checkpoint - 1;

            Subscribe();

            return recordedEvents;
        }

        private void Dropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
        {
            Console.WriteLine("Subscription dropped, please enter to reconnect.");
            Subscribe();
        }

        public Dictionary<string, int> GetProductNameQuantityList(List<EventStore.ClientAPI.RecordedEvent> recordedEvents)
        {
            foreach (var _event in recordedEvents)
            {
                Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(_event.Data));
                if (_dictSoldItems.ContainsKey(objSales.ProductName))
                {
                    _dictSoldItems[objSales.ProductName] += objSales.Quantity;
                }
                else
                {
                    _dictSoldItems.Add(objSales.ProductName, objSales.Quantity);
                }
            }

            return _dictSoldItems;
        }
        
        public void PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity)
        {
            Console.WriteLine("Fetching current sales info:");
            Console.WriteLine($"|{ "Name".PadRight(20, ' ')}|{"Quantity".PadRight(10, ' ')}|");

            foreach (var item in dictProductNameQuantity)
            {
                Console.WriteLine($"|{item.Key.PadRight(20, ' ')}|{item.Value.ToString().PadRight(10, ' ')}|");
            }
        }
    }
}
