using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalance.Reactive.Commands
{
    using System.Threading;
    using ReactiveDomain.Messaging;

    public class DepositeCheque : Command
    {
        public Guid AccountId;
        public decimal Fund;
        public DateTime DepositDate;

        public DepositeCheque()
            : base(CorrelatedMessage.NewRoot())
        {
        }

        public DepositeCheque(CorrelatedMessage source, CancellationToken? token = null)
            : base(source, token)
        {
        }

        public DepositeCheque(CorrelationId correlationId, SourceId sourceId, CancellationToken? token = null)
            : base(correlationId, sourceId, token)
        {
        }
    }
}
