namespace EventStoreSaleExercise
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    public class InventoryManager : IInventoryManagerReadModel
    {
        private int? Checkpoint;
        private string _streamName;
        private Dictionary<string, int> _dictSoldItems;

        public InventoryManager(string streamName)
        {
            _streamName = streamName;
            _dictSoldItems = new Dictionary<string, int>();
            Checkpoint = null;
        }
        
        private void Subscribe(string streamName)
        {
            //if (Checkpoint == 0)
            //    Checkpoint = null;

            EventStoreSetup.conn.SubscribeToStreamFrom(streamName, lastCheckpoint: Checkpoint, resolveLinkTos: false, eventAppeared: (s, e) => ReceivedEvent(s, e),
                subscriptionDropped: Dropped);
        }

        private void ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt)
        {
            try
            {
                if (evt.Event.EventType == EventStoreSetup.SaleAddedEvent)
                {
                    var receivedEvent = new List<byte[]> { evt.Event.Data };
                    var dictProductNameQuantity = GetProductNameQuantityList(receivedEvent);
                    PrintProductNameQuantity(dictProductNameQuantity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Event exception: " + ex.Message);
            }
        }

        public List<byte[]> ReadEventsFromStream(IEventStoreConnection conn,
            string streamName, int checkpoint, int slice)
        {
            List<byte[]> recordedEvents = new List<byte[]>();
            while (true)
            {
                var eventSlice = conn.ReadStreamEventsForwardAsync(streamName, checkpoint, slice, true).Result;

                foreach (var _event in eventSlice.Events)
                {
                    recordedEvents.Add(_event.Event.Data);
                    checkpoint = (int)_event.Event.EventNumber + 1;
                }

                slice = (int)eventSlice.LastEventNumber;

                if (eventSlice.IsEndOfStream)
                    break;
            }

            if (checkpoint > 0)
                Checkpoint = checkpoint - 1;

            Subscribe(streamName);

            return recordedEvents;
        }

        private void Dropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
        {
            Console.WriteLine("Subscription dropped, please enter to reconnect.");
            Subscribe(_streamName);
        }

        public Dictionary<string, int> GetProductNameQuantityList(List<byte[]> recordedEvents)
        {
            foreach (var eventData in recordedEvents)
            {
                Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(eventData));
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
        
        public string PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity)
        {
            try
            {
                Console.WriteLine("Fetching current sales info:");
                Console.WriteLine($"|{"Name".PadRight(20, ' ')}|{"Quantity".PadRight(10, ' ')}|");

                foreach (var item in dictProductNameQuantity)
                {
                    Console.WriteLine($"|{item.Key.PadRight(20, ' ')}|{item.Value.ToString().PadRight(10, ' ')}|");
                }

                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
