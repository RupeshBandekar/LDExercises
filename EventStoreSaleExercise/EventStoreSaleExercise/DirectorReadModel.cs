namespace EventStoreSaleExercise
{
    using System;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    public class DirectorReadModel : IReadModel
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
            ReadAllSaleAddedEvents();
        }

        public void Subscribe()
        {
            conn.SubscribeToStreamFrom(_streamName, Checkpoint, false, (s, e) => ReceivedEvent(s, e), subscriptionDropped: Dropped);
        }

        public void ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt)
        {
            try
            {
                var checkpoint = evt.Event.EventNumber;
                
                if (evt.Event.EventType == Globals.SaleAddedEvent)
                {
                    Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(evt.Event.Data));
                    _totalItemsSold += objSales.Quantity;
                    _totalSales += objSales.Price;
                    Console.WriteLine("|" + "Total items sold".PadRight(20,' ') + "|" + "Total sales($)".PadRight(20,' ') + "|");
                    Console.WriteLine("|" + _totalItemsSold.ToString().PadRight(20, ' ') + "|" + _totalSales.ToString().PadRight(20, ' ') + "|");
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

            while (true)
            {
                var eventSlice = conn.ReadStreamEventsForwardAsync(_streamName, checkpoint, slice, true).Result;

                foreach (var _event in eventSlice.Events)
                {
                    Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(_event.Event.Data));
                    _totalItemsSold += objSales.Quantity;
                    _totalSales += objSales.Price;

                    checkpoint = (int)_event.Event.EventNumber + 1;
                }

                if (eventSlice.IsEndOfStream)
                    break;
            }

            Console.WriteLine("|" + _totalItemsSold.ToString().PadRight(20, ' ') + "|" +
                              _totalSales.ToString().PadRight(20, ' ') + "|");
            
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
