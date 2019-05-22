using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStoreSaleExercise.Tests.Helper;
using Newtonsoft.Json;

namespace EventStoreSaleExercise.Tests
{
    using Xunit;

    public class DirectorReadModelTests
    {
        [Fact]
        public void can_get_total_sales_amount()
        {
            MockDataProvider eventStoreDataProvider = new MockDataProvider();
            int count = 100;
            var eventStream = eventStoreDataProvider.ReadStreamEventsForwardAsync("Dummy", 0, ref count, false);

            DirectorRM director = new DirectorRM();
            var totalSalesAmount = director.GetTotalSalesAmount(eventStream);

            Assert.Equal(4760, totalSalesAmount);
        }

        [Fact]
        public void can_print_total_sales_amount()
        {
            var totalSalesAmount = 1000;
            DirectorRM director = new DirectorRM();
            Assert.Equal("Success", director.PrintTotalSalesAmount(totalSalesAmount));
        }
    }
}
