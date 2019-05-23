namespace EventStoreSaleExercise
{
    using System;
    using System.Text;
    using EventStore.ClientAPI;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static EventStoreSaleExercise.EventStoreSetup;

    public class DirectorRM : IDirectorReadModel
    {
        private decimal _totalSales;

        public Task ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt)
        {
            try
            {
                if (evt.Event.EventType == SaleAddedEvent)
                {
                    var receivedEvent = new List<Sales> { JsonConvert.DeserializeObject<Sales>(Encoding.UTF8.GetString(evt.Event.Data)) };
                    var totalSales = GetTotalSalesAmount(receivedEvent);
                    PrintTotalSalesAmount(totalSales);
                }
            }
            catch (Exception ex)
            {
                Viewer.ConsoleWrite("Event exception: " + ex.Message);
            }

            return Task.CompletedTask;
        }
        
        public decimal GetTotalSalesAmount(List<Sales> recordedEvents)
        {
            foreach (var eventData in recordedEvents)
            {
                _totalSales += eventData.Quantity * eventData.Price;
            }

            return _totalSales;
        }

        public string PrintTotalSalesAmount(decimal totalSalesAmount)
        {
            Viewer.ConsoleWrite("Fetching total sales:");
            Viewer.ConsoleWrite($"|{"Total sales($)".PadRight(20, ' ')}|");
            Viewer.ConsoleWrite($"|{totalSalesAmount.ToString().PadRight(20, ' ')}|");

            return "Success";
        }
    }
}
