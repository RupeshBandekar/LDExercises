namespace AccountBalance.Reactive.Events
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    public class CashWithdrawn : Event
    {
        public Guid AccountId;
        public decimal Fund;

        public CashWithdrawn(CorrelatedMessage source)
            : base(source)
        {
        }

         [JsonConstructor]
        public CashWithdrawn(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        {
        }
    }
}
