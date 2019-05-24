namespace EventStoreSaleExercise
{
    using System;
    public class Viewer : IViewer
    {
        private readonly IViewer _viewer;

        public Viewer()
        { }

        public Viewer(IViewer viewer)
        {
            _viewer = viewer;
        }
        public void PerformAction()
        {
            ActionService svc = new ActionService();
            svc.PerformActionByRole(_viewer);
        }

        public static string ConsoleWrite(string message)
        {
            Console.WriteLine(message);
            return message;
        }
    }
}
