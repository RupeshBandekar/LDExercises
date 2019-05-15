namespace EventStoreSaleExercise
{
    using System;
    using EventStore.ClientAPI;
    public interface IReadModel
    {
        void Subscribe();

        void ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt);

        void ReadAllSaleAddedEvents();

        void Dropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex);
    }
}
