namespace EventStoreSaleExercise
{
    using System.Collections.Generic;
    public interface IEventStoreDataProvider
    {
        List<Sales> ReadStreamEventsForwardAsync(string streamName, long start, ref int count, bool resolveLinkTos);
    }
}
