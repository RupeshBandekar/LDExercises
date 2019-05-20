namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    using System.Collections.Generic;
    public interface IDirectorReadModel
    {
        decimal GetTotalSalesAmount(List<byte[]> recordedEvents);

        string PrintTotalSalesAmount(decimal totalSalesAmount);
    }
}
