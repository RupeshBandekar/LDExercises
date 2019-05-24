namespace EventStoreSaleExercise.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Helper;

    public class InventoryManagerTests
    {
        [Fact]
        public void can_get_distinct_product_name_quantity_list()
        {
            var eventStoreDataProvider = new MockDataProvider();
            var count = 100;
            var eventStream = eventStoreDataProvider.ReadStreamEventsForwardAsync("Dummy", 0, ref count, false);

            IInventoryManagerReadModel inventory = new InventoryManagerRM();
            var dictSoldItems =  inventory.GetProductNameQuantityList(eventStream);

            Assert.Equal(6, dictSoldItems.Count);
        }

        [Fact]
        public void can_get_cumulative_quantity_of_duplicate_products()
        {
            var eventStoreDataProvider = new MockDataProvider();
            var count = 100;
            var eventStream = eventStoreDataProvider.ReadStreamEventsForwardAsync("Dummy", 0, ref count, false);

            IInventoryManagerReadModel inventory = new InventoryManagerRM();
            var dictSoldItems = inventory.GetProductNameQuantityList(eventStream);

            Assert.Equal(105, dictSoldItems.Values.Sum());
        }

        [Fact]
        public void can_get_product_wise_quantities_from_duplicate_products()
        {
            var eventStoreDataProvider = new MockDataProvider();
            var count = 100;
            var eventStream = eventStoreDataProvider.ReadStreamEventsForwardAsync("Dummy", 0, ref count, false);

            IInventoryManagerReadModel inventory = new InventoryManagerRM();
            var dictSoldItems = inventory.GetProductNameQuantityList(eventStream);

            Assert.Equal(25, dictSoldItems["MONITOR"]);
            Assert.Equal(20, dictSoldItems["KEYBOARD"]);
            Assert.Equal(10, dictSoldItems["MOUSE"]);
            Assert.Equal(20, dictSoldItems["NOTEPAD"]);
            Assert.Equal(20, dictSoldItems["PEN"]);
            Assert.Equal(10, dictSoldItems["PENCIL"]);
        }

        [Fact]
        public void can_read_empty_product_name_quantity_list()
        {
            var eventStoreDataProvider = new MockEmptyDataProvider();
            var count = 100;
            var eventStream = eventStoreDataProvider.ReadStreamEventsForwardAsync("Dummy", 0, ref count, false);

            IInventoryManagerReadModel inventory = new InventoryManagerRM();
            var dictSoldItems = inventory.GetProductNameQuantityList(eventStream);

            Assert.Empty(dictSoldItems);
            Assert.Equal(0, dictSoldItems.Values.Sum());
        }

        [Fact]
        public void can_print_product_name_quantity()
        {
            var dictProductNameQuantity = new Dictionary<string, int>
            {
                { "MONITOR", 10 },
                { "MOUSE", 10 },
                { "KEYBOARD", 10 }
            };

            IInventoryManagerReadModel inventory = new InventoryManagerRM();
            Assert.Equal("Success", inventory.PrintProductNameQuantity(dictProductNameQuantity));
        }

        [Fact]
        public void can_print_no_sale_found_message()
        {
            var dictProductNameQuantity = new Dictionary<string, int>();

            IInventoryManagerReadModel inventory = new InventoryManagerRM();
            Assert.Equal("No sales found", inventory.PrintProductNameQuantity(dictProductNameQuantity));
        }
    }
}
