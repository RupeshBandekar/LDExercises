using System;
using System.Configuration;

namespace EventStoreSaleExercise
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Viewer.ConsoleWrite("1 - Salesman");
            Viewer.ConsoleWrite("2 - Inventory Manager");
            Viewer.ConsoleWrite("3 - Director");
            var input = Console.ReadLine();
            IViewer view = new Viewer();
            PerformAction(input, view);
        }

        public static string PerformAction(string input, IViewer view)
        {
            EventStoreSetup.CreateConnection();
            
            var welcomeMsg = "";
            if (input == "1")
            {
                welcomeMsg = "You have entered as a Salesman";
                Viewer.ConsoleWrite(welcomeMsg);

                try
                {
                    if (view.GetType() == typeof(Viewer))
                    {
                        ISalesman sales = new Sales("Dummy", 1, 1);
                        view = new Viewer(sales);
                        ((Viewer)view).PerformAction();
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
                    view = new Viewer(inventory);
                    ((Viewer)view).PerformAction();
                }
            }
            else if (input == "3")
            {
                welcomeMsg = "You have entered as a Director";
                Viewer.ConsoleWrite(welcomeMsg);

                if (view.GetType() == typeof(Viewer))
                {
                    IDirectorReadModel director = new DirectorRM();
                    view = new Viewer(director);
                    ((Viewer)view).PerformAction();
                }
            }

            return welcomeMsg;
        }
    }
}
