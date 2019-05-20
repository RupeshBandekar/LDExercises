using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using Xunit;
using EventStoreSaleExercise;
using EventStoreSaleExercise.Tests.Helper;
using Newtonsoft.Json;


namespace EventStoreSaleExercise.Tests
{
    public class InventoryManagerTests
    {
        [Fact]
        public void can_read_product_name_quantity_list()
        {
            MockDataProvider eventStoreDataProvider = new MockDataProvider();
            int count = 100;
            var eventStream = eventStoreDataProvider.ReadStreamEventsForwardAsync("Dummy", 0, ref count, false);

            IInventoryManagerReadModel inventory = new InventoryManager();
            var dictSoldItems =  inventory.GetProductNameQuantityList(eventStream);

            Assert.Equal(6, dictSoldItems.Count);
            Assert.Equal(105, dictSoldItems.Values.Sum());
            //var mockEventStore =new MockIEventStoreConnection();
            //inventory.ReadEventsFromStream(mockEventStore,)
        }

        [Fact]
        public void can_print_product_name_quantity()
        {
            var dictProductNameQuantity = new Dictionary<string, int>();
            dictProductNameQuantity.Add("MONITOR", 10);
            dictProductNameQuantity.Add("MOUSE", 10);
            dictProductNameQuantity.Add("KEYBOARD", 10);

            IInventoryManagerReadModel inventory = new InventoryManager();
            Assert.Equal("Success", inventory.PrintProductNameQuantity(dictProductNameQuantity));
        }
    }
}
