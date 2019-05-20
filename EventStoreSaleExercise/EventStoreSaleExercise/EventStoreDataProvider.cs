using System.Collections.Generic;

namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    public class EventStoreDataProvider : IEventStoreDataProvider
    {
        private IEventStoreConnection _conn;

        public EventStoreDataProvider(IEventStoreConnection conn)
        {
            _conn = conn;
        }

        public List<byte[]> ReadStreamEventsForwardAsync(string streamName, long start,ref int count, bool resolveLinkTos)
        {
            var recordedEvents = new List<byte[]>();

            while (true)
            {
                var eventSlice =  _conn.ReadStreamEventsForwardAsync(streamName, start, count, resolveLinkTos).Result;

                foreach (var _event in eventSlice.Events)
                {
                    recordedEvents.Add(_event.Event.Data);
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
