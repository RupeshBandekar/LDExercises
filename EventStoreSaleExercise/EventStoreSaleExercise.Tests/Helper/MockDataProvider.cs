using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace EventStoreSaleExercise.Tests.Helper
{
    public class MockDataProvider : IEventStoreDataProvider
    {
        public List<Sales> ReadStreamEventsForwardAsync(string streamName, long start, ref int count, bool resolveLinkTos)
        {
            List<byte[]> eventStream = new List<byte[]>();

            List<Sales> recordedEvents = new List<Sales>
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
                new Sales( "pencil", 10, 2),
            };

            return recordedEvents;
        }
    }
}
