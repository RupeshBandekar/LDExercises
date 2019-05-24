namespace AccountBalanceDomain.Events
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;
    public class AccountCreated : Event
    {
        public Guid AccountId { get; set; }
        public string AccountHolderName { get; set; }

        public AccountCreated(CorrelatedMessage source) : base(source)
        { }

        [JsonConstructor]
        public AccountCreated(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId: correlationId, sourceId: sourceId)
        { }
    }
}
