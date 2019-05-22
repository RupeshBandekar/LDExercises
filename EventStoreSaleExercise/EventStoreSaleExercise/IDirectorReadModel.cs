﻿using System.Threading.Tasks;

namespace EventStoreSaleExercise
{
    using EventStore.ClientAPI;
    using System.Collections.Generic;
    public interface IDirectorReadModel : IViewer
    {
        decimal GetTotalSalesAmount(List<byte[]> recordedEvents);

        string PrintTotalSalesAmount(decimal totalSalesAmount);

        Task ReceivedEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent evt);
    }
}
