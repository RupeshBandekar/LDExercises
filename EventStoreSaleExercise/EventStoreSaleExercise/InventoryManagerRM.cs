using System.Threading.Tasks;

namespace EventStoreSaleExercise
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    public class InventoryManagerRM : IInventoryManagerReadModel
    {
        private Dictionary<string, int> _dictSoldItems;

        public InventoryManagerRM()
        {
            _dictSoldItems = new Dictionary<string, int>();
        }

        public Task ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt)
        {
            try
            {
                if (evt.Event.EventType == EventStoreSetup.SaleAddedEvent)
                {
                    var receivedEvent = new List<byte[]> { evt.Event.Data };
                    var dictProductNameQuantity = GetProductNameQuantityList(receivedEvent);
                    var printStatus = PrintProductNameQuantity(dictProductNameQuantity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Event exception: " + ex.Message);
            }

            return Task.CompletedTask;
        }

        public Dictionary<string, int> GetProductNameQuantityList(List<byte[]> recordedEvents)
        {
            foreach (var eventData in recordedEvents)
            {
                Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(eventData));
                if (_dictSoldItems.ContainsKey(objSales.ProductName))
                {
                    _dictSoldItems[objSales.ProductName] += objSales.Quantity;
                }
                else
                {
                    _dictSoldItems.Add(objSales.ProductName, objSales.Quantity);
                }
            }

            return _dictSoldItems;
        }

        public string PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity)
        {
            Console.WriteLine("Fetching current sales info:");
            Console.WriteLine($"|{"Name".PadRight(20, ' ')}|{"Quantity".PadRight(10, ' ')}|");

            foreach (var item in dictProductNameQuantity)
            {
                Console.WriteLine($"|{item.Key.PadRight(20, ' ')}|{item.Value.ToString().PadRight(10, ' ')}|");
            }

            return "Success";
        }
    }
}
