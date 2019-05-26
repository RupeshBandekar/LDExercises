using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalance.Reactive.Events
{
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    public class DailyWireTransferLimitApplied : Event
    {
        public Guid AccountId;
        public decimal DailyWireTransferLimit;

        public DailyWireTransferLimitApplied(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public DailyWireTransferLimitApplied(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        {
        }
    }
}
