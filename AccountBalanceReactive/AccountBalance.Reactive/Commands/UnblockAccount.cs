using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalance.Reactive.Commands
{
    using System.Threading;
    using ReactiveDomain.Messaging;

    public class UnblockAccount : Command
    {
        public Guid AccountId;

        public UnblockAccount(Command source)
            : base(source)
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
