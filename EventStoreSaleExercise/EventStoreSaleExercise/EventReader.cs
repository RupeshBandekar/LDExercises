namespace EventStoreSaleExercise
{
    using System;
    using System.Threading.Tasks;
    using EventStore.ClientAPI;
    public class EventReader : IEventReader
    {
        private readonly string _streamName;
        private readonly Func<EventStoreCatchUpSubscription, ResolvedEvent, Task> _receivedEvent;
        private long? _checkpoint;
        public EventReader(string streamName, Func<EventStoreCatchUpSubscription, ResolvedEvent, Task> receivedEvent)
        {
            _streamName = streamName;
            _receivedEvent = receivedEvent;
        }

        public void SubscribeEventStream(long checkpoint)
        {
            if (checkpoint > 0)
                _checkpoint = checkpoint - 1;

            Subscribe(_streamName);
        }

        private void Subscribe(string streamName)
        {
            EventStoreSetup.conn.SubscribeToStreamFrom(streamName, _checkpoint, false, 
                (s, e) => _receivedEvent(s, e), subscriptionDropped: Dropped);
        }

        private void Dropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
        {
            Subscribe(_streamName);
        }
    }
}
