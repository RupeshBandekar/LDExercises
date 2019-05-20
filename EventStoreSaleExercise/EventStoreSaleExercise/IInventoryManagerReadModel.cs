namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    using System.Collections.Generic;
    public interface IInventoryManagerReadModel
    {
        Dictionary<string, int> GetProductNameQuantityList(List<byte[]> recordedEvents);
        
        string PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity);
    }
}
