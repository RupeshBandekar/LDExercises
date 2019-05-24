using System.Collections.Generic;

namespace EventStoreSaleExercise
{
    using System;

    public class ActionService
    {
        public static string PerformAction(string input, IViewer view)
        {
            if (view.GetType() == typeof(Viewer))
            {
                EventStoreSetup.CreateConnection();
            }

            var welcomeMsg = "";
            if (input == "1")
            {
                welcomeMsg = "You have entered as a Salesman";
                Viewer.ConsoleWrite(welcomeMsg);

                try
                {
                    if (view.GetType() == typeof(Viewer))
                    {
                        while (true)
                        {
                            Viewer.ConsoleWrite("Please enter your sales details");
                            Viewer.ConsoleWrite("Product Name:");
                            var productName = Console.ReadLine();
                            Viewer.ConsoleWrite("Quantity:");
                            var quantity = Console.ReadLine();
                            Viewer.ConsoleWrite("Price:");
                            var price = Console.ReadLine();

                            try
                            {
                                ISalesman sales = new Sales(productName, Convert.ToInt32(quantity),
                                    Convert.ToDecimal(price));
                                PerformActionByRole(sales);
                            }
                            catch (Exception ex)
                            {
                                Viewer.ConsoleWrite(ex.Message);
                            }

                            Viewer.ConsoleWrite("Do you want to add another sale? Y/N:");
                            var action = Console.ReadLine();
                            if (action != null && action.ToUpper() == "Y")
                                continue;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Viewer.ConsoleWrite(ex.Message);
                }
            }
            else if (input == "2")
            {
                welcomeMsg = "You have entered as a Inventory Manager";
                Viewer.ConsoleWrite(welcomeMsg);
                if (view.GetType() == typeof(Viewer))
                {
                    IInventoryManagerReadModel inventory = new InventoryManagerRM();
                    PerformActionByRole(inventory);
                }
            }
            else if (input == "3")
            {
                welcomeMsg = "You have entered as a Director";
                Viewer.ConsoleWrite(welcomeMsg);

                if (view.GetType() == typeof(Viewer))
                {
                    IDirectorReadModel director = new DirectorRM();
                    PerformActionByRole(director);
                }
            }

            return welcomeMsg;
        }

        private static void PerformActionByRole(IViewer viewer)
        {
            if (viewer.GetType() == typeof(Sales))
            {
                Salesman((ISalesman) viewer);
            }

            if (viewer.GetType() == typeof(InventoryManagerRM))
            {
                InventoryManager((IInventoryManagerReadModel) viewer);
                Console.ReadLine();
            }

            if (viewer.GetType() == typeof(DirectorRM))
            {
                Director((IDirectorReadModel) viewer);
                Console.ReadLine();
            }
        }

        private static void Salesman(ISalesman objSales)
        {
            try
            {
                if (objSales.AddSale(EventStoreSetup.conn) == "Success")
                {
                    Viewer.ConsoleWrite($"Sale published on {EventStoreSetup.StreamName} stream.");
                }

                Viewer.ConsoleWrite($"Error occured while publishing data to stream");
            }
            catch (Exception ex)
            {
                Viewer.ConsoleWrite(ex.Message);
            }
        }

        private static void InventoryManager(IInventoryManagerReadModel inventory)
        {
            try
            {
                var count = 10;
                var receivedEvents = GetReceivedEvents(ref count);
                var eventReader = new EventReader(EventStoreSetup.StreamName,
                    (s, e) => inventory.ReceivedEvent(s, e));
                eventReader.SubscribeEventStream(count);
                var dictProductNameQuantity = inventory.GetProductNameQuantityList(receivedEvents);
                var printStatus = inventory.PrintProductNameQuantity(dictProductNameQuantity);
            }
            catch (Exception ex)
            {
                Viewer.ConsoleWrite(ex.Message);
            }
        }

        private static void Director(IDirectorReadModel director)
        {
            try
            {
                var count = 10;
                var receivedEvents = GetReceivedEvents(ref count);
                var eventReader = new EventReader(EventStoreSetup.StreamName, (s, e) => director.ReceivedEvent(s, e));
                eventReader.SubscribeEventStream(count);
                var totalSales = director.GetTotalSalesAmount(receivedEvents);
                var printStatus = director.PrintTotalSalesAmount(totalSales);
            }
            catch (Exception ex)
            {
                Viewer.ConsoleWrite(ex.Message);
            }
        }

        private static List<Sales> GetReceivedEvents(ref int count)
        {
            var eventStoreDataProvider = new EventStoreDataProvider(EventStoreSetup.conn);
            var receivedEvents = eventStoreDataProvider.ReadStreamEventsForwardAsync(EventStoreSetup.StreamName, 0,
                ref count,
                false);

            return receivedEvents;
        }
    }
}
