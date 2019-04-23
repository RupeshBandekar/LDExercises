namespace AccountBalanceDomain
{
    using System;
    public class AccountCreated : IBaseAccountEvent
    {
        public readonly Guid AccountId;
        public readonly string AccountHolderName;
        public AccountCreated(Guid id, string accountHolderName)
        {
            AccountId = id;
            AccountHolderName = accountHolderName;
        }
    }

    public class OverdraftLimitApplied : IBaseAccountEvent
    {
        public readonly decimal OverdraftLimit;

        public OverdraftLimitApplied(decimal overdraftLimit)
        {
            OverdraftLimit = overdraftLimit;
        }
    }

    public class DailyWireTransferLimitApplied : IBaseAccountEvent
    {
        public readonly decimal DailyWireTransferLimit;
        public readonly decimal DailyWireTransferLimitUtilization;

        public DailyWireTransferLimitApplied(decimal dailyWireTransferLimit, decimal dailyWireTransferLimitUtilization)
        {
            DailyWireTransferLimit = dailyWireTransferLimit;
            DailyWireTransferLimitUtilization = dailyWireTransferLimitUtilization;
        }
    }

    public class ChequeDeposited : IBaseAccountEvent
    {
        public readonly decimal Fund;
        public readonly DateTime DepositDate;
        public readonly DateTime ClearanceBussinessDay;

        public ChequeDeposited(decimal fund, DateTime depositDate, DateTime clearanceBusinessDay)
        {
            Fund = fund;
            DepositDate = depositDate;
            ClearanceBussinessDay = clearanceBusinessDay;
        }
    }
    
    public class CashDeposited : IBaseAccountEvent
    {
        public readonly decimal Fund;

        public CashDeposited(decimal fund)
        {
            Fund = fund;
        }
    }

    public class CashWithdrew : IBaseAccountEvent
    {
        public readonly decimal Fund;

        public CashWithdrew(decimal fund)
        {
            Fund = fund;
        }
    }

    public class WireTransferred : IBaseAccountEvent
    {
        public readonly decimal Fund;

        public WireTransferred(decimal fund)
        {
            Fund = fund;
        }
    }

    public class AccountBlocked : IBaseAccountEvent
    {
        public readonly AccountState AccountState;

        public AccountBlocked(AccountState accountState)
        {
            AccountState = accountState;
        }
    }

    public class AccountBlockedOverdraftLimitBreach : AccountBlocked
    {
        public AccountBlockedOverdraftLimitBreach(AccountState accountState) : base(accountState)
        {
        }
    }

    public class AccountBlockedDailyWireTransferLimitBreach : AccountBlocked
    {
        public AccountBlockedDailyWireTransferLimitBreach(AccountState accountState) : base(accountState)
        {
        }
    }

    public class AccountUnblocked : IBaseAccountEvent
    {
        public readonly AccountState AccountState;

        public AccountUnblocked(AccountState accountState)
        {
            AccountState = accountState;
        }
    }
}
