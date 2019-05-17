namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    using System.Collections.Generic;
    public interface IDirectorReadModel
    {
        List<byte[]> ReadEventsFromStream(IEventStoreConnection conn,
            string streamName, int checkpoint, int slice);
        decimal GetTotalSalesAmount(List<byte[]> recordedEvents);

        string PrintTotalSalesAmount(decimal totalSalesAmount);
    }
}
