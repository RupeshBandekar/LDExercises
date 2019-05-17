using System;

namespace EventStoreSaleExercise
{
    class Program
    {
        public static void Main(string[] args)
        {
            EventStoreSetup.CreateConnection();

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
            else if (input == "2")
            {
                Console.WriteLine("You have entered as a Inventory Manager");
                IInventoryManagerReadModel inventoryManager = new InventoryManager(EventStoreSetup.StreamName);
                var recordedEvents = inventoryManager.ReadEventsFromStream(EventStoreSetup.conn, EventStoreSetup.StreamName, 0, 10);
                var dictProductNameQuantity = inventoryManager.GetProductNameQuantityList(recordedEvents);
                inventoryManager.PrintProductNameQuantity(dictProductNameQuantity);
                
                Console.ReadLine();
            }
            else if (input == "3")
            {
                Console.WriteLine("You have entered as a Director");
                IDirectorReadModel director = new Director(EventStoreSetup.StreamName);
                var recordedEvents = director.ReadEventsFromStream(EventStoreSetup.conn, EventStoreSetup.StreamName, 0, 10);
                var totalSales = director.GetTotalSalesAmount(recordedEvents);
                director.PrintTotalSalesAmount(totalSales);

                Console.ReadLine();
            }
        }
    }
}
