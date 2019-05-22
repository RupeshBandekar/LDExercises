using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStoreSaleExercise
{
    public class Viewer : IViewer
    {
        private IViewer _viewer;

        public Viewer()
        { }

        public Viewer(IViewer viewer)
        {
            if (viewer.GetType() == typeof(Sales))
            {
                _viewer = ((ISalesman)viewer);
            }

            if (viewer.GetType() == typeof(InventoryManagerRM))
            {
                _viewer = ((IInventoryManagerReadModel)viewer);
            }

            if (viewer.GetType() == typeof(DirectorRM))
            {
                _viewer = ((IDirectorReadModel)viewer);
            }
        }

        public static string ConsoleWrite(string message)
        {
            Console.WriteLine(message);
            return "success";
        }

        public void PerformAction()
        {
            if (_viewer.GetType() == typeof(Sales))
            {
                Salesman((ISalesman)_viewer);
            }

            if (_viewer.GetType() == typeof(InventoryManagerRM))
            {
                InventoryManager((IInventoryManagerReadModel)_viewer);
                Console.ReadLine();
            }

            if (_viewer.GetType() == typeof(DirectorRM))
            {
                Director((IDirectorReadModel)_viewer);
                Console.ReadLine();
            }
        }
        private void Salesman(ISalesman objSales)
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
                    objSales = new Sales(productName, Convert.ToInt32(quantity), Convert.ToDecimal(price));
                    if (objSales.AddSale(EventStoreSetup.conn) == "Success")
                    {
                        ConsoleWrite($"Sale published on {EventStoreSetup.StreamName} stream.");
                        ConsoleWrite("Do you want to add another sale? Y/N:");
                        var action = Console.ReadLine();
                        if (action != null && action.ToUpper() == "Y")
                            continue;
                        else break;
                    }
                    else
                    {
                        ConsoleWrite($"Error occured while publishing data to stream");
                    }
                }
                catch (Exception ex)
                {
                    ConsoleWrite(ex.Message);
                }
            }
        }

        private void InventoryManager(IInventoryManagerReadModel inventory)
        {
            var eventStoreDataProvider = new EventStoreDataProvider(EventStoreSetup.conn);
            var count = 10;
            var receivedEvents =
                eventStoreDataProvider.ReadStreamEventsForwardAsync(EventStoreSetup.StreamName, 0, ref count,
                    false);
            inventory = new InventoryManagerRM();
            var eventReader = new EventReader(EventStoreSetup.StreamName,
                (s, e) => inventory.ReceivedEvent(s, e));
            eventReader.SubscribeEventStream(count);
            var dictProductNameQuantity = inventory.GetProductNameQuantityList(receivedEvents);
            var printStatus = inventory.PrintProductNameQuantity(dictProductNameQuantity);
        }

        private void Director(IDirectorReadModel director)
        {
            var eventStoreDataProvider = new EventStoreDataProvider(EventStoreSetup.conn);
            var count = 10;
            var receivedEvents = eventStoreDataProvider.ReadStreamEventsForwardAsync(EventStoreSetup.StreamName, 0, ref count, false);
            director = new DirectorRM();
            var eventReader = new EventReader(EventStoreSetup.StreamName, (s, e) => director.ReceivedEvent(s, e));
            eventReader.SubscribeEventStream(count);
            var totalSales = director.GetTotalSalesAmount(receivedEvents);
            var printStatus = director.PrintTotalSalesAmount(totalSales);
        }
    }
}
