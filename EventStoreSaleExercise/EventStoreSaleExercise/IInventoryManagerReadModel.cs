using System.Threading.Tasks;

namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    using System.Collections.Generic;
    public interface IInventoryManagerReadModel : IViewer
    {
        Dictionary<string, int> GetProductNameQuantityList(List<byte[]> recordedEvents);
        
        string PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity);

        Task ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt);
    }
}
