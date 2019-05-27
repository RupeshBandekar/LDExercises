namespace AccountBalance.Reactive.Commands
{
    using System;
    using System.Threading;
    using ReactiveDomain.Messaging;

    public class UnblockAccount : Command
    {
        public Guid AccountId;

        public UnblockAccount()
            : base(CorrelatedMessage.NewRoot())
        {
        }

        public UnblockAccount(CorrelatedMessage source, CancellationToken? token = null)
            : base(source, token)
        {
        }

        public UnblockAccount(CorrelationId correlationId, SourceId sourceId, CancellationToken? token = null)
            : base(correlationId, sourceId, token)
        {
        }
    }
}
