using System;
using System.Configuration;

namespace EventStoreSaleExercise
{
    class Program
    {
        public static void Main(string[] args)
        {
            EventStoreSetup.CreateConnection();
            Viewer view = new Viewer();

            Console.WriteLine("1 - Salesman");
            Console.WriteLine("2 - Inventory Manager");
            Console.WriteLine("3 - Director");
            var input = Console.ReadLine();

            if (input == "1")
            {
                Console.WriteLine("You have entered as a Salesman");

                while (true)
                {
                    view.Salesman();
                }
            }
            else if (input == "2")
            {
                Console.WriteLine("You have entered as a Inventory Manager");
                
                view.InventoryManager();

                Console.ReadLine();
            }
            else if (input == "3")
            {
                Console.WriteLine("You have entered as a Director");
                
                view.Director();

                Console.ReadLine();
            }
        }
    }
}
