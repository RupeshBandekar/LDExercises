namespace EventStoreSaleExercise
{
    using System;
    using System.Collections.Generic;
    using EventStore.ClientAPI;
    public interface IEventReader
    {
        void SubscribeEventStream(long start);
    }
}
