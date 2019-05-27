namespace AccountBalance.Reactive.Commands
{
    using System;
    using System.Threading;
    using ReactiveDomain.Messaging;

    public class DepositCash : Command
    {
        public Guid AccountId;
        public decimal Fund;

        public DepositCash()
            : base(CorrelatedMessage.NewRoot())
        {
        }

        public DepositCash(CorrelatedMessage source, CancellationToken? token = null)
            : base(source, token)
        {
        }

        public DepositCash(CorrelationId correlationId, SourceId sourceId, CancellationToken? token = null)
            : base(correlationId, sourceId, token)
        {
        }
    }
}
