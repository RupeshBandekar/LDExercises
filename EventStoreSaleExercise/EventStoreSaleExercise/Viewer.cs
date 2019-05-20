using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStoreSaleExercise
{
    public class Viewer
    {
        public void Salesman()
        {
            while (true)
            {
                Console.WriteLine("Please enter your sales details");
                Console.WriteLine("Product Name:");
                var productName = Console.ReadLine();
                Console.WriteLine("Quantity:");
                var quantity = Console.ReadLine();
                Console.WriteLine("Price:");
                var price = Console.ReadLine();

                try
                {
                    ISalesman objSales = new Sales(productName, Convert.ToInt32(quantity), Convert.ToDecimal(price));
                    if (objSales.AddSale(EventStoreSetup.conn) == "Success")
                    {
                        Console.WriteLine($"Sale published on {EventStoreSetup.StreamName} stream.");
                    }
                    else
                    {
                        Console.WriteLine($"Error occured while publishing data to stream");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void InventoryManager()
        {
            var eventStoreDataProvider = new EventStoreDataProvider(EventStoreSetup.conn);
            var count = 10;
            var receivedEvents =
                eventStoreDataProvider.ReadStreamEventsForwardAsync(EventStoreSetup.StreamName, 0, ref count,
                    false);
            var inventoryManager = new InventoryManager();
            var eventReader = new EventReader(EventStoreSetup.StreamName,
                (s, e) => inventoryManager.ReceivedEvent(s, e));
            eventReader.SubscribeEventStream(count);
            var dictProductNameQuantity = inventoryManager.GetProductNameQuantityList(receivedEvents);
            var printStatus = inventoryManager.PrintProductNameQuantity(dictProductNameQuantity);
        }

        public void Director()
        {
            var eventStoreDataProvider = new EventStoreDataProvider(EventStoreSetup.conn);
            var count = 10;
            var receivedEvents = eventStoreDataProvider.ReadStreamEventsForwardAsync(EventStoreSetup.StreamName, 0, ref count, false);
            var director = new Director();
            var eventReader = new EventReader(EventStoreSetup.StreamName, (s, e) => director.ReceivedEvent(s, e));
            eventReader.SubscribeEventStream(count);
            var totalSales = director.GetTotalSalesAmount(receivedEvents);
            var printStatus = director.PrintTotalSalesAmount(totalSales);
        }
    }
}
