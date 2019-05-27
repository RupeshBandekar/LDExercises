namespace AccountBalance.Reactive.Commands
{
    using System;
    using System.Threading;
    using ReactiveDomain.Messaging;

    public class DepositCheque : Command
    {
        public Guid AccountId;
        public decimal Fund;
        public DateTime DepositDate;

        public DepositCheque()
            : base(CorrelatedMessage.NewRoot())
        {
        }

        public DepositCheque(CorrelatedMessage source, CancellationToken? token = null)
            : base(source, token)
        {
        }

        public DepositCheque(CorrelationId correlationId, SourceId sourceId, CancellationToken? token = null)
            : base(correlationId, sourceId, token)
        {
        }
    }
}
