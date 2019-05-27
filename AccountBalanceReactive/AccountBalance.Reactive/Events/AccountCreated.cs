namespace AccountBalance.Reactive
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    public sealed class AccountCreated : Event
    {
        public AccountCreated(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public AccountCreated(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public string AccountHolderName { get; set; }
    }
}
