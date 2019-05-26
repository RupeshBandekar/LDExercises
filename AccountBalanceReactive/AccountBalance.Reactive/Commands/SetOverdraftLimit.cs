using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalance.Reactive.Commands
{
    using ReactiveDomain.Messaging;

    public class SetOverdraftLimit : Command
    {
        public SetOverdraftLimit()
            :base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public decimal OverdraftLimit { get; set; }
    }
}
