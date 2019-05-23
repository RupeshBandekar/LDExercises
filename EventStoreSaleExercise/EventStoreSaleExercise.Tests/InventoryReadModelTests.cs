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

            IInventoryManagerReadModel inventory = new InventoryManagerRM();
            var dictSoldItems =  inventory.GetProductNameQuantityList(eventStream);

            Assert.Equal(6, dictSoldItems.Count);
            Assert.Equal(105, dictSoldItems.Values.Sum());
            Assert.Equal(25, dictSoldItems["MONITOR"]);
            Assert.Equal(20, dictSoldItems["KEYBOARD"]);
            Assert.Equal(10, dictSoldItems["MOUSE"]);
            Assert.Equal(20, dictSoldItems["NOTEPAD"]);
            Assert.Equal(20, dictSoldItems["PEN"]);
            Assert.Equal(10, dictSoldItems["PENCIL"]);
        }

        [Fact]
        public void can_print_product_name_quantity()
        {
            var dictProductNameQuantity = new Dictionary<string, int>();
            dictProductNameQuantity.Add("MONITOR", 10);
            dictProductNameQuantity.Add("MOUSE", 10);
            dictProductNameQuantity.Add("KEYBOARD", 10);

            IInventoryManagerReadModel inventory = new InventoryManagerRM();
            Assert.Equal("Success", inventory.PrintProductNameQuantity(dictProductNameQuantity));
        }
    }
}
