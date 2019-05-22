using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Xunit;

namespace EventStoreSaleExercise.Tests
{
    public class SalesmanTests
    {
        //[Fact]
        //public void can_save_achieved_sales()
        //{
        //    var conn = EventStoreSetup.CreateConnection();

        //    Sales objSales = new Sales("monitor", 1, 150);
        //    Assert.Equal("Success", objSales.AddSale(conn));
        //}

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void can_not_save_invalid_product_name(string productName)
        {
            var exception = Assert.Throws<Exception>(() => new Sales(productName, 1, 150));
            Assert.Equal("Invalid product name", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void can_not_save_invalid_quantity(int quantity)
        {
            var exception = Assert.Throws<Exception>(() => new Sales("monitor", quantity, 150));
            Assert.Equal("Invalid quantity", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void can_not_save_invalid_price(decimal price)
        {
            var exception = Assert.Throws<Exception>(() => new Sales("monitor", 10, price));
            Assert.Equal("Invalid price", exception.Message);
        }
    }
}
