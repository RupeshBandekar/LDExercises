using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Commands
{
    public class SetOverdraftLimit : Command
    {
        public readonly Guid AccountId;
        public decimal OverdraftLimit;

        public SetOverdraftLimit(Guid accountId, decimal overdraftLimit, CorrelatedMessage source)
        :this(accountId, overdraftLimit, new CorrelationId(source), new SourceId(source))
            { }

        [JsonConstructor]
        public SetOverdraftLimit(Guid accountId, decimal overdraftLimit, CorrelationId correlationId,
            SourceId sourceId)
            : base(correlationId, sourceId)
        {
            AccountId = accountId;
            OverdraftLimit = overdraftLimit;
        }
    }
}
