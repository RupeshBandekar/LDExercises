namespace AccountBalance.Reactive.Events
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    public class ChequeDeposited : Event
    {
        public Guid AccountId;
        public decimal Fund;
        public DateTime DepositDate;
        public DateTime ClearanceBusinessDay;

        public ChequeDeposited(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public ChequeDeposited(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        {
        }
    }
}
