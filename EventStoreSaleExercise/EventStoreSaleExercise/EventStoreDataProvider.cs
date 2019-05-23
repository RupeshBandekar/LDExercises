namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Text;

    public class EventStoreDataProvider : IEventStoreDataProvider
    {
        private IEventStoreConnection _conn;

        public EventStoreDataProvider(IEventStoreConnection conn)
        {
            _conn = conn;
        }

        public List<Sales> ReadStreamEventsForwardAsync(string streamName, long start,ref int count, bool resolveLinkTos)
        {
            var recordedEvents = new List<Sales>();

            while (true)
            {
                var eventSlice =  _conn.ReadStreamEventsForwardAsync(streamName, start, count, resolveLinkTos).Result;

                foreach (var _event in eventSlice.Events)
                {
                    recordedEvents.Add(JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(_event.Event.Data)));
                    start = (int)_event.Event.EventNumber + 1;
                }

                count = (int)eventSlice.LastEventNumber + 1;

                if (eventSlice.IsEndOfStream)
                    break;
            }

            return recordedEvents;
        }
    }
}
