namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    public interface ISalesman : IViewer
    {
        string AddSale(IEventStoreConnection conn);
    }
}
