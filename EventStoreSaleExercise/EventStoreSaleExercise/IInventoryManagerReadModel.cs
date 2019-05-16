namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    using System.Collections.Generic;
    public interface IInventoryManagerReadModel
    {
        Dictionary<string, int> GetProductNameQuantityList(List<EventStore.ClientAPI.RecordedEvent> recordedEvents);
        
        void PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity);

        List<EventStore.ClientAPI.RecordedEvent> ReadEventsFromStream(IEventStoreConnection conn,
            string streamName, ref int checkpoint, int slice);
    }
}
