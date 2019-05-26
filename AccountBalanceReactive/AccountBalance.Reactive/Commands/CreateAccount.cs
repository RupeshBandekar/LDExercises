namespace AccountBalance.Reactive
{
    using System;
    using ReactiveDomain.Messaging;

    public class CreateAccount : Command
    {
        public CreateAccount()
            : base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public string AccountHolderName { get; set; }
    }
}
