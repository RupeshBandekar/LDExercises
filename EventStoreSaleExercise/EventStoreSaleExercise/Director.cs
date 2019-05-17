namespace EventStoreSaleExercise
{
    using System;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class Director : IDirectorReadModel
    {
        private int? Checkpoint;
        private string _streamName;
        private decimal _totalSales;

        public Director(string streamName)
        {
            _streamName = streamName;
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
                    var totalSales = GetTotalSalesAmount(receivedEvent);
                    PrintTotalSalesAmount(totalSales);
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

        public decimal GetTotalSalesAmount(List<byte[]> recordedEvents)
        {
            foreach (var eventData in recordedEvents)
            {
                Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(eventData));
                _totalSales += objSales.Price;
            }

            return _totalSales;
        }

        public string PrintTotalSalesAmount(decimal totalSalesAmount)
        {
            try
            {
                Console.WriteLine("Fetching total sales:");
                Console.WriteLine($"|{"Total sales($)".PadRight(20, ' ')}|");
                Console.WriteLine($"|{totalSalesAmount.ToString().PadRight(20, ' ')}|");

                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
