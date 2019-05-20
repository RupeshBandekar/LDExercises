using System.Collections.Generic;

namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    public interface IEventStoreDataProvider
    {
        List<byte[]> ReadStreamEventsForwardAsync(string streamName, long start, ref int count, bool resolveLinkTos);
    }
}
