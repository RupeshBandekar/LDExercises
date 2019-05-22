using System.Threading.Tasks;
using static EventStoreSaleExercise.EventStoreSetup;

namespace EventStoreSaleExercise
{
    using System;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class DirectorRM : IDirectorReadModel
    {
        private decimal _totalSales;

        public DirectorRM()
        {
        }
        
        public Task ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt)
        {
            try
            {
                if (evt.Event.EventType == SaleAddedEvent)
                {
                    var receivedEvent = new List<byte[]> { evt.Event.Data };
                    var totalSales = GetTotalSalesAmount(receivedEvent);
                    var printStatus = PrintTotalSalesAmount(totalSales);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Event exception: " + ex.Message);
            }

            return Task.CompletedTask;
        }
        
        public decimal GetTotalSalesAmount(List<byte[]> recordedEvents)
        {
            foreach (var eventData in recordedEvents)
            {
                Sales objSales = JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(eventData));
                _totalSales += objSales.Quantity * objSales.Price;
            }

            return _totalSales;
        }

        public string PrintTotalSalesAmount(decimal totalSalesAmount)
        {
            Console.WriteLine("Fetching total sales:");
            Console.WriteLine($"|{"Total sales($)".PadRight(20, ' ')}|");
            Console.WriteLine($"|{totalSalesAmount.ToString().PadRight(20, ' ')}|");

            return "Success";
        }
    }
}
