using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalance.Reactive.Commands
{
    using System.Threading;
    using ReactiveDomain.Messaging;

    public class BlockAccount : Command
    {
        public Guid AccountId;
        public string ReasonForAccountBlock;

        public BlockAccount()
            : base(CorrelatedMessage.NewRoot())
        {
        }

        public BlockAccount(CorrelatedMessage source, CancellationToken? token = null)
            : base(source, token)
        {
        }

        public BlockAccount(CorrelationId correlationId, SourceId sourceId, CancellationToken? token = null)
            : base(correlationId, sourceId, token)
        {
        }
    }
}
