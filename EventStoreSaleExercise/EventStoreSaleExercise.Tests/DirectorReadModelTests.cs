namespace EventStoreSaleExercise.Tests
{
    using Xunit;
    using EventStoreSaleExercise.Tests.Helper;

    public class DirectorReadModelTests
    {
        [Fact]
        public void can_get_total_sales_amount()
        {
            var eventStoreDataProvider = new MockDataProvider();
            var count = 100;
            var eventStream = eventStoreDataProvider.ReadStreamEventsForwardAsync("Dummy", 0, ref count, false);

            var director = new DirectorRM();
            var totalSalesAmount = director.GetTotalSalesAmount(eventStream);

            Assert.Equal(4760, totalSalesAmount);
        }

        [Fact]
        public void can_get_total_sales_amount_when_distinct_products_sold()
        {
            var eventStoreDataProvider = new MockDistinctDataProvider();
            var count = 100;
            var eventStream = eventStoreDataProvider.ReadStreamEventsForwardAsync("Dummy", 0, ref count, false);

            var director = new DirectorRM();
            var totalSalesAmount = director.GetTotalSalesAmount(eventStream);

            Assert.Equal(2390, totalSalesAmount);
        }

        [Fact]
        public void can_get_zero_sales_amount_when_no_data()
        {
            var eventStoreDataProvider = new MockEmptyDataProvider();
            var count = 100;
            var eventStream = eventStoreDataProvider.ReadStreamEventsForwardAsync("Dummy", 0, ref count, false);

            var director = new DirectorRM();
            var totalSalesAmount = director.GetTotalSalesAmount(eventStream);

            Assert.Equal(0, totalSalesAmount);
        }

        [Fact]
        public void can_print_total_sales_amount()
        {
            var totalSalesAmount = 1000;
            var director = new DirectorRM();
            Assert.Equal("Success", director.PrintTotalSalesAmount(totalSalesAmount));
        }

        [Fact]
        public void can_print_zero_when_total_sale_amount_is_zero()
        {
            var totalSalesAmount = 0;
            var director = new DirectorRM();
            Assert.Equal("Success", director.PrintTotalSalesAmount(totalSalesAmount));
        }
    }
}
