﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalance.Reactive.Events
{
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    public class OverdraftLimitApplied : Event
    {
        public OverdraftLimitApplied(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public OverdraftLimitApplied(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal OverdraftLimit { get; set; }
    }
}
