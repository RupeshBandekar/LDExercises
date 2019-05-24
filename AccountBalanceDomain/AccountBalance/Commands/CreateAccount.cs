namespace AccountBalanceDomain.Commands
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;
    public class CreateAccount : Command
    {
        public readonly Guid AccountId;
        public string AccountHolderName { get; set; }

        //public CreateAccount(CorrelatedMessage source)
        //    : base(source)
        //{ }

        public CreateAccount(Guid accountId, string accountHolderName, CorrelatedMessage source)
            : this(accountId, accountHolderName, new CorrelationId(source), new SourceId(source))
        {}

        [JsonConstructor]
        public CreateAccount(Guid accountId, string accountHolderName, CorrelationId correlationId, SourceId sourceId)
            : base(correlationId: correlationId, sourceId: sourceId)
        {
            AccountId = accountId;
            AccountHolderName = accountHolderName;
        }
    }
}
