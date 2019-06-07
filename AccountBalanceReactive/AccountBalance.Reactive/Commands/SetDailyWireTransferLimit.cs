namespace AccountBalance.Reactive.Commands
{
    using System;
    using System.Threading;
    using ReactiveDomain.Messaging;

    public class SetDailyWireTransferLimit : Command
    {
        public Guid AccountId;
        public decimal DailyWireTransferLimit;

        public SetDailyWireTransferLimit()
            : base(CorrelatedMessage.NewRoot())
        {
        }

        public SetDailyWireTransferLimit(CorrelatedMessage source, CancellationToken? token = null)
            : base(source, token)
        {
        }

        public SetDailyWireTransferLimit(CorrelationId correlationId, SourceId sourceId, CancellationToken? token = null)
            : base(correlationId, sourceId, token)
        {
        }
    }
}
