using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Xunit;
using EventStoreSaleExercise;
using Newtonsoft.Json;


namespace EventStoreSaleExercise.Tests
{
    public class InventoryManagerTests
    {
        [Fact]
        public void can_read_product_name_quantity_list()
        {
            List<Sales> recordedEvents = new List<Sales>
            {
                new Sales( "MONITOR", 10, 100),
                new Sales( "KEYBOARD", 10, 80),
                new Sales( "MOUSE", 10, 50),
                new Sales( "MOUSE", 10, 50),
                new Sales( "MOUSE", 10, 50),
                new Sales( "MOUSE", 10, 50),
            };

            var streamData = new List<byte[]>();
            foreach (var eventData in recordedEvents)
            {
                var events = JsonConvert.SerializeObject(eventData);
                streamData.Add(Encoding.UTF8.GetBytes(events));
            }

            IInventoryManagerReadModel inventory = new InventoryManager("Dummy");
            var dictSoldItems =  inventory.GetProductNameQuantityList(streamData);

            Assert.Equal(3, dictSoldItems.Count);
            //var mockEventStore =new MockIEventStoreConnection();
            //inventory.ReadEventsFromStream(mockEventStore,)
        }

        [Fact]
        public void can_print_product_name_quantity()
        {
            var dictProductNameQuantity = new Dictionary<string, int>();
            IInventoryManagerReadModel inventory = new InventoryManager("Dummy");
            Assert.Equal("Success", inventory.PrintProductNameQuantity(dictProductNameQuantity));
        }
    }
}
