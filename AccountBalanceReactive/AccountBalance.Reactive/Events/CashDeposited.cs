using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalance.Reactive.Events
{
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    public class CashDeposited : Event
    {
        public Guid AccountId;
        public decimal Fund;

        public CashDeposited(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public CashDeposited(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        {
        }
    }
}
