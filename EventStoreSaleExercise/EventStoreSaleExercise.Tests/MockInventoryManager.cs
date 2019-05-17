using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventStoreSaleExercise.Tests
{
    public class MockInventoryManager : IInventoryManagerReadModel
    {
        public List<byte[]> ReadEventsFromStream(IEventStoreConnection conn, string streamName, int checkpoint, int slice)
        {
            List<Sales> recordedEvents = new List<Sales>
            {
                new Sales( "MONITOR", 10, 100),
                new Sales( "KEYBOARD", 10, 80),
                new Sales( "MOUSE", 10, 50),
            };

            List<byte[]> streamData = new List<byte[]>();
            foreach (var eventData in recordedEvents)
            {
                var events = JsonConvert.SerializeObject(eventData);
                streamData.Add(Encoding.UTF8.GetBytes(events));
            }

            return streamData;
        }

        public Dictionary<string, int> GetProductNameQuantityList(List<byte[]> recordedEvents)
        {
            throw new NotImplementedException();
        }

        public void PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity)
        {
            throw new NotImplementedException();
        }

        private byte[] GetDataByte(Sales objSales)
        {
            var events = JsonConvert.SerializeObject(objSales);
            return Encoding.UTF8.GetBytes(events);
        }

        string IInventoryManagerReadModel.PrintProductNameQuantity(Dictionary<string, int> dictProductNameQuantity)
        {
            throw new NotImplementedException();
        }
    }
}
