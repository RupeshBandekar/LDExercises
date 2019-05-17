using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace EventStoreSaleExercise.Tests
{
    using Xunit;
    public class DirectorReadModelTests
    {
        [Fact]
        public void can_read_total_sales_amount()
        {
            List<Sales> recordedEvents = new List<Sales>
            {
                new Sales( "MONITOR", 10, 100),
                new Sales( "KEYBOARD", 10, 80),
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

            IDirectorReadModel director = new Director("Dummy");
            var totalSalesAmount = director.GetTotalSalesAmount(streamData);

            Assert.Equal(330, totalSalesAmount);
        }

        [Fact]
        public void can_print_total_sales_amount()
        {
            var totalSalesAmount = 0;
            IDirectorReadModel director = new Director("Dummy");
            Assert.Equal("Success", director.PrintTotalSalesAmount(totalSalesAmount));
        }
    }
}
