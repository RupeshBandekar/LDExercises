namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    using System.Collections.Generic;
    public interface IDirectorReadModel
    {
        decimal GetTotalSalesAmount(List<EventStore.ClientAPI.RecordedEvent> recordedEvents);

        void PrintTotalSalesAmount(decimal totalSalesAmount);

        List<EventStore.ClientAPI.RecordedEvent> ReadEventsFromStream(IEventStoreConnection conn,
            string streamName, ref int checkpoint, int slice);
    }
}
