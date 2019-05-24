namespace AccountBalanceDomain.Aggregate
{
    using System;
    using ReactiveDomain;
    using ReactiveDomain.Messaging;
    using AccountBalanceDomain.Events;

    public class Account : EventDrivenStateMachine
    {
        private Account()
        {
            Register<AccountCreated>(evt =>
            {
                Id = evt.AccountId;
            });
        }

        public static Account CreateAccount(Guid accountId, string accountHolderName, CorrelatedMessage source)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException("Invalid Account ID");

            if (string.IsNullOrWhiteSpace(accountHolderName))
                throw new ArgumentException("Invalid Account Holder Name");

            var account = new Account();
            account.Raise(new AccountCreated(source)
            {
                AccountId = accountId,
                AccountHolderName = accountHolderName
            });

            return account;
        }

        public void SetOverdraftLimit(Guid accountId, decimal overdraftLimit, CorrelatedMessage source)
        {
            if (overdraftLimit <= 0)
                throw new ArgumentException("Invalid Overdraft Limit");

            Raise(new OverdraftLimitApplied(source)
            {
                AccountId = accountId,
                OverdraftLimit = overdraftLimit
            });
        }
    }
}
