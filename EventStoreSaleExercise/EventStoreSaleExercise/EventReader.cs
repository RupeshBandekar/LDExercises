using System.Threading.Tasks;

namespace EventStoreSaleExercise
{
    using System;
    using System.Collections.Generic;
    using EventStore.ClientAPI;
    public class EventReader : IEventReader
    {
        private readonly string _streamName;
        private readonly Func<EventStoreCatchUpSubscription, ResolvedEvent, Task> _receivedEvent;
        private long? Checkpoint;
        private IEventStoreDataProvider _eventStoreDataProvider;
        public EventReader(string streamName, Func<EventStoreCatchUpSubscription, ResolvedEvent, Task> receivedEvent)
        {
            _streamName = streamName;
            _receivedEvent = receivedEvent;
        }

        public void SubscribeEventStream(long checkpoint)
        {
            if (checkpoint > 0)
                Checkpoint = checkpoint - 1;

            Subscribe(_streamName);
        }

        private void Subscribe(string streamName)
        {
            EventStoreSetup.conn.SubscribeToStreamFrom(streamName, Checkpoint, false, 
                (s, e) => _receivedEvent(s, e), subscriptionDropped: Dropped);
        }

        private void Dropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
        {
            Subscribe(_streamName);
        }
    }
}
