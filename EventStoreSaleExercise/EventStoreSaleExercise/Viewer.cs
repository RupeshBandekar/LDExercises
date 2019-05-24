namespace EventStoreSaleExercise
{
    using System;
    public class Viewer : IViewer
    {
        private readonly IViewer _viewer;

        public Viewer()
        { }

        public static string ConsoleWrite(string message)
        {
            Console.WriteLine(message);
            return message;
        }
    }
}
