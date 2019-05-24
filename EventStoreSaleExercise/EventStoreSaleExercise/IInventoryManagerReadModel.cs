namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IInventoryManagerReadModel : IViewer
    {
        Dictionary<string, int> GetProductNameQuantityList(List<Sales> recordedEvents);
        
        bool PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity);

        Task ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt);
    }
}
