namespace EventStoreSaleExercise.Tests.Helper
{
    using System.Collections.Generic;

    public class MockHappyDataProvider : IEventStoreDataProvider
    {
        public List<Sales> ReadStreamEventsForwardAsync(string streamName, long start, ref int count, bool resolveLinkTos)
        {
            var recordedEvents = new List<Sales>
            {
                new Sales( "monitor", 10, 100),
            };

            return recordedEvents;
        }
    }
    public class MockDataProvider : IEventStoreDataProvider
    {
        public List<Sales> ReadStreamEventsForwardAsync(string streamName, long start, ref int count, bool resolveLinkTos)
        {
            var recordedEvents = new List<Sales>
            {
                new Sales( "monitor", 10, 100),
                new Sales( "monitor", 15, 100),
                new Sales( "keyboard", 10, 80),
                new Sales( "keyboard", 10, 80),
                new Sales( "mouse", 10, 50),
                new Sales( "notepad", 10, 5),
                new Sales( "notepad", 10, 5),
                new Sales( "pen", 10, 2),
                new Sales( "pen", 10, 2),
                new Sales( "pencil", 10, 2)
            };

            return recordedEvents;
        }
    }

    public class MockDistinctDataProvider : IEventStoreDataProvider
    {
        public List<Sales> ReadStreamEventsForwardAsync(string streamName, long start, ref int count, bool resolveLinkTos)
        {
            var recordedEvents = new List<Sales>
            {
                new Sales( "monitor", 10, 100),
                new Sales( "keyboard", 10, 80),
                new Sales( "mouse", 10, 50),
                new Sales( "notepad", 10, 5),
                new Sales( "pen", 10, 2),
                new Sales( "pencil", 10, 2)
            };

            return recordedEvents;
        }
    }

    public class MockEmptyDataProvider : IEventStoreDataProvider
    {
        public List<Sales> ReadStreamEventsForwardAsync(string streamName, long start, ref int count, bool resolveLinkTos)
        {
            var recordedEvents = new List<Sales>();

            return recordedEvents;
        }
    }
}
