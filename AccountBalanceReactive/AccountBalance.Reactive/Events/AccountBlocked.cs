using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalance.Reactive.Events
{
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    public class AccountBlocked : Event
    {
        public Guid AccountId;
        public string ReasonForAccountBlock;

        public AccountBlocked(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public AccountBlocked(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        {
        }
    }
}
