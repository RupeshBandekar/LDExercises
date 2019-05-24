namespace EventStoreSaleExercise
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

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
                    var receivedEvent = new List<Sales>
                        {JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(evt.Event.Data))};
                    var dictProductNameQuantity = GetProductNameQuantityList(receivedEvent);
                    var printStatus = PrintProductNameQuantity(dictProductNameQuantity);
                }
            }
            catch (Exception ex)
            {
                Viewer.ConsoleWrite("Event exception: " + ex.Message);
            }

            return Task.CompletedTask;
        }

        public Dictionary<string, int> GetProductNameQuantityList(List<Sales> recordedEvents)
        {
            foreach (var eventData in recordedEvents)
            {
                if (_dictSoldItems.ContainsKey(eventData.ProductName))
                {
                    _dictSoldItems[eventData.ProductName] += eventData.Quantity;
                }
                else
                {
                    _dictSoldItems.Add(eventData.ProductName, eventData.Quantity);
                }
            }

            return _dictSoldItems;
        }

        public string PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity)
        {
            Viewer.ConsoleWrite("Fetching current sales info:");

            if (dictProductNameQuantity.Count == 0)
            {
                return Viewer.ConsoleWrite("No sales found");
            }

            Viewer.ConsoleWrite($"|{"Name".PadRight(20, ' ')}|{"Quantity".PadRight(10, ' ')}|");

            foreach (var item in dictProductNameQuantity)
            {
                Viewer.ConsoleWrite($"|{item.Key.PadRight(20, ' ')}|{item.Value.ToString().PadRight(10, ' ')}|");
            }

            return "Success";

        }
    }
}
