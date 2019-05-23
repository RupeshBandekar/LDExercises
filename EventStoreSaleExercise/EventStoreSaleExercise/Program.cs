namespace EventStoreSaleExercise
{
    using System;
    public class Program
    {
        public static void Main(string[] args)
        {
            Viewer.ConsoleWrite("1 - Salesman");
            Viewer.ConsoleWrite("2 - Inventory Manager");
            Viewer.ConsoleWrite("3 - Director");
            var input = Console.ReadLine();
            IViewer view = new Viewer();
            ActionService.PerformAction(input, view);
        }
    }
}
