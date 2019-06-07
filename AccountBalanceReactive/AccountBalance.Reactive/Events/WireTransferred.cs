namespace AccountBalance.Reactive.Events
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    public class WireTransferred : Event
    {
        public Guid AccountId;
        public decimal Fund;
        public DateTime WireTransferDate;

        public WireTransferred(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public WireTransferred(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        {
        }
    }
}
