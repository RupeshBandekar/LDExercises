namespace EventStoreSaleExercise
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    public class Sales : ISalesman
    {
        public readonly Guid Id;
        public readonly string ProductName;
        public readonly int Quantity;
        public readonly decimal Price;

        public Sales(string productName, int quantity, decimal price)
        {
            if (!(productName != null && productName.Trim() != string.Empty && productName.Trim() != ""))
            {
                throw new Exception("Invalid product name");
            }
            if (quantity <= 0)
            {
                throw new Exception("Invalid quantity");
            }
            if (price <= 0)
            {
                throw new Exception("Invalid price");
            }

            Id = Guid.NewGuid();
            ProductName = productName.ToUpper();
            Quantity = quantity;
            Price = price;
        }

        public string AddSale(IEventStoreConnection conn)
        {
            try
            {
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
