namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    using System.Collections.Generic;
    public interface IInventoryManagerReadModel
    {
        List<byte[]> ReadEventsFromStream(IEventStoreConnection conn,
            string streamName, int checkpoint, int slice);
        Dictionary<string, int> GetProductNameQuantityList(List<byte[]> recordedEvents);
        
        string PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity);
    }
}
