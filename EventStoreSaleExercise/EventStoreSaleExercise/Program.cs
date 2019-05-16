using System;
using EventStore.ClientAPI;

namespace EventStoreSaleExercise
{
    class Program
    {
        static void Main(string[] args)
        {
            IEventStoreConnection conn = EventStoreSetup.CreateConnection();

            Console.WriteLine("1 - Salesman");
            Console.WriteLine("2 - Inventory Manager");
            Console.WriteLine("3 - Director");
            var input = Console.ReadLine();

            if (input == "1")
            {
                Console.WriteLine("You have entered as a Salesman");
                while (true)
                {
                    Console.WriteLine("Please enter your sales details");
                    Console.WriteLine("Product Name:");
                    string productName = Console.ReadLine();
                    Console.WriteLine("Quantity:");
                    string quantity = Console.ReadLine();
                    Console.WriteLine("Price:");
                    string price = Console.ReadLine();

                    try
                    {
                        Sales objSales = new Sales(productName, Convert.ToInt32(quantity), Convert.ToDecimal(price));
                        if (objSales.AddSale() == "Success")
                        {
                            Console.WriteLine($"Sale published on {EventStoreSetup.StreamName} stream.");
                        }
                        else
                        {
                            Console.WriteLine($"Error occured while publishing data on stream");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                }
            }
            else if (input == "2")
            {
                Console.WriteLine("You have entered as a Inventory Manager");
                IInventoryManagerReadModel inventoryManager = new InventoryManager(EventStoreSetup.StreamName);
                var recordedEvents = inventoryManager.ReadEventsFromStream(EventStoreSetup.conn, streamName, ref checkpoint, 10);
                var dictProductNameQuantity = inventoryManager.GetProductNameQuantityList(recordedEvents);
                inventoryManager.PrintProductNameQuantity(dictProductNameQuantity);
                
                Console.ReadLine();
            }
            else if (input == "3")
            {
                Console.WriteLine("You have entered as a Director");
                IDirectorReadModel director = new Director(EventStoreSetup.StreamName);
                
                Console.ReadLine();
            }
        }
    }
}
