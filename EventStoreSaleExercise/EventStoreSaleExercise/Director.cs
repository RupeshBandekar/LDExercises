namespace EventStoreSaleExercise
{
    using System;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class Director : IDirectorReadModel
    {
        public int? Checkpoint { get; private set; }
        private string _streamName;
        private IEventStoreConnection conn = null;
        private decimal _totalSales;


        public Director(string streamName)
        {
            int checkpoint = 0;
            conn = EventStoreSetup.CreateConnection();
            _streamName = streamName;
            var recordedEvents  = EventStoreSetup.ReadEventsFromStream(conn, streamName, ref checkpoint, 10);
            PrintTotalSalesAmount(GetTotalSalesAmount(recordedEvents));
            Checkpoint = checkpoint;
            Subscribe();
        }

        private void Subscribe()
        {
            if (Checkpoint == 0)
                Checkpoint = null;

                conn.SubscribeToStreamFrom(_streamName, lastCheckpoint: Checkpoint, resolveLinkTos: false, eventAppeared: (s, e) => ReceivedEvent(s, e),
                    subscriptionDropped: Dropped);
        }

        private void ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt)
        {
            try
            {
                if (evt.Event.EventType == EventStoreSetup.SaleAddedEvent)
                {
                    var receivedEvent = new List<RecordedEvent> { evt.Event };
                    PrintTotalSalesAmount(GetTotalSalesAmount(receivedEvent));
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

        public decimal GetTotalSalesAmount(List<EventStore.ClientAPI.RecordedEvent> recordedEvents)
        {
            foreach (var _event in recordedEvents)
            {
                Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(_event.Data));
                _totalSales += objSales.Price;
            }

            return _totalSales;
        }

        public void PrintTotalSalesAmount(decimal totalSalesAmount)
        {
            Console.WriteLine("Fetching total sales:");
            Console.WriteLine($"|{"Total sales($)".PadRight(20, ' ')}|");
            Console.WriteLine($"|{totalSalesAmount.ToString().PadRight(20, ' ')}|");
        }
    }
}
