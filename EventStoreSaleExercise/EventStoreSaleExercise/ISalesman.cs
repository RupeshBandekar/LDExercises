namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    public interface ISalesman
    {
        string AddSale(IEventStoreConnection conn);
    }
}
