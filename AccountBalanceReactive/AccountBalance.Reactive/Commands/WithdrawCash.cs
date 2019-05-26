using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalance.Reactive.Commands
{
    using System.Threading;
    using ReactiveDomain.Messaging;

    public class WithdrawCash : Command
    {
        public Guid AccountId;
        public decimal Fund;

        public WithdrawCash()
            : base(CorrelatedMessage.NewRoot())
        {
        }

        public WithdrawCash(CorrelatedMessage source, CancellationToken? token = null)
            : base(source, token)
        {
        }

        public WithdrawCash(CorrelationId correlationId, SourceId sourceId, CancellationToken? token = null)
            : base(correlationId, sourceId, token)
        {
        }
    }
}
