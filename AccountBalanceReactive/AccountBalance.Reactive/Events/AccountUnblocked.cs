using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalance.Reactive.Events
{
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    public class AccountUnblocked : Event
    {
        public Guid AccountId;

        public AccountUnblocked(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public AccountUnblocked(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        {
        }
    }
}
