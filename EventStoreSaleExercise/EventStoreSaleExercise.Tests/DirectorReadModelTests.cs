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

            Director director = new Director();
            var totalSalesAmount = director.GetTotalSalesAmount(eventStream);

            Assert.Equal(4760, totalSalesAmount);
        }

        [Fact]
        public void can_print_total_sales_amount()
        {
            var totalSalesAmount = 1000;
            Director director = new Director();
            Assert.Equal("Success", director.PrintTotalSalesAmount(totalSalesAmount));
        }

        [Fact]
        public void can_execute_callback_method()
        {
            EventStoreCatchUpSubscription _fakeSubscription = null;
            ResolvedEvent rEvent = new ResolvedEvent();

            Director director = new Director();
            Assert.Equal(Task.CompletedTask, director.ReceivedEvent(_fakeSubscription, rEvent));
        }
    }
}
