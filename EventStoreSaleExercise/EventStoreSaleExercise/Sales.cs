using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventStoreSaleExercise
{
    public class Sales
    {
        public readonly Guid Id;
        public readonly string ProductName;
        public readonly int Quantity;
        public readonly decimal Price;

        public Sales(string productName, int quantity, decimal price)
        {
            Id = Guid.NewGuid();
            ProductName = productName.ToUpper();
            Quantity = quantity;
            Price = price;
        }

        public void AddSale(Sales objSales)
        {
            var conn = Globals.CreateConnection();
            var streamName = "Sales";
            var eventData = ProcessSalesData(objSales);
            conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventData).Wait();
            Console.WriteLine($"Sale published on {streamName} stream.");
        }

        private List<EventData> ProcessSalesData(Sales objSales)
        {
            var events = JsonConvert.SerializeObject(objSales);
            var eventData = new List<EventData>();
            
                var id = Guid.NewGuid().ToString();
                var eventType = Globals.SaleAddedEvent;
                eventData.Add(new EventData(Guid.Parse(id), eventType, true,
                    Encoding.UTF8.GetBytes(events), null));
           

            return eventData;
        }
    }
}
