namespace AccountBalance.Reactive.Commands
{
    using System;
    using System.Threading;
    using ReactiveDomain.Messaging;

    public class WireTransfer : Command
    {
        public Guid AccountId;
        public decimal Fund;
        public DateTime WireTransferDate;

        public WireTransfer()
            : base(CorrelatedMessage.NewRoot())
        {
        }

        public WireTransfer(CorrelatedMessage source, CancellationToken? token = null)
            : base(source, token)
        {
        }

        public WireTransfer(CorrelationId correlationId, SourceId sourceId, CancellationToken? token = null)
            : base(correlationId, sourceId, token)
        {
        }
    }
}
