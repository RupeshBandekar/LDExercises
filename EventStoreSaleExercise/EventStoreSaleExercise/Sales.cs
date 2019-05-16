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
            if (!(ProductName != null && ProductName.Trim() != string.Empty && ProductName.Trim() != ""))
            {
                throw new Exception("Invalid product name");
            }
            if (Quantity <= 0)
            {
                throw new Exception("Invalid quantity");
            }
            if (Price <= 0)
            {
                throw new Exception("Invalid price");
            }

            Id = Guid.NewGuid();
            ProductName = productName.ToUpper();
            Quantity = quantity;
            Price = price;
        }

        public string AddSale()
        {
            try
            {
                var conn = EventStoreSetup.CreateConnection();
                var streamName = EventStoreSetup.StreamName;
                var eventData = ProcessSalesData(this);
                conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventData).Wait();

                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private List<EventData> ProcessSalesData(Sales objSales)
        {
            var events = JsonConvert.SerializeObject(objSales);
            var eventData = new List<EventData>();

            var id = Guid.NewGuid().ToString();
            var eventType = EventStoreSetup.SaleAddedEvent;
            eventData.Add(new EventData(Guid.Parse(id), eventType, true,
                Encoding.UTF8.GetBytes(events), null));

            return eventData;
        }
    }
}
