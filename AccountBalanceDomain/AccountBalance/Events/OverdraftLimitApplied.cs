namespace AccountBalanceDomain.Events
{
    using ReactiveDomain.Messaging;
    using System;
    using Newtonsoft.Json;

    class OverdraftLimitApplied : Event
    {
        public Guid AccountId { get; set; }
        public decimal OverdraftLimit { get; set; }

        public OverdraftLimitApplied(CorrelatedMessage source) : base(source)
        { }

        [JsonConstructor]
        public OverdraftLimitApplied(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId: correlationId, sourceId: sourceId)
        { }
    }
}
