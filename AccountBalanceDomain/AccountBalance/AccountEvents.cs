namespace AccountBalanceDomain
{
    using System;

    public class AccountCreated : IBaseAccountEvent
    {
        public readonly Guid AccountId;
        public readonly string AccountHolderName;

        public AccountCreated(Guid accountId, string accountHolderName)
        {
            AccountId = accountId;
            AccountHolderName = accountHolderName;
        }
    }

    public class OverdraftLimitApplied : IBaseAccountEvent
    {
        public readonly Guid AccountId;
        public readonly decimal OverdraftLimit;

        public OverdraftLimitApplied(Guid accountId, decimal overdraftLimit)
        {
            AccountId = accountId;
            OverdraftLimit = overdraftLimit;
        }
    }

    public class DailyWireTransferLimitApplied : IBaseAccountEvent
    {
        public readonly Guid AccountId;
        public readonly decimal DailyWireTransferLimit;

        public DailyWireTransferLimitApplied(Guid accountId, decimal dailyWireTransferLimit)
        {
            AccountId = accountId;
            DailyWireTransferLimit = dailyWireTransferLimit;
        }
    }

    public class ChequeDeposited : IBaseAccountEvent
    {
        public readonly Guid AccountId;
        public readonly decimal Fund;
        public readonly DateTime DepositDate;
        public readonly DateTime ClearanceBusinessDay;

        public ChequeDeposited(Guid accountId, decimal fund, DateTime depositDate, DateTime clearanceBusinessDay)
        {
            AccountId = accountId;
            Fund = fund;
            DepositDate = depositDate;
            ClearanceBusinessDay = clearanceBusinessDay;
        }
    }

    public class CashDeposited : IBaseAccountEvent
    {
        public readonly Guid AccountId;
        public readonly decimal Fund;

        public CashDeposited(Guid accountId, decimal fund)
        {
            AccountId = accountId;
            Fund = fund;
        }
    }

    public class CashWithdrawn : IBaseAccountEvent
    {
        public readonly Guid AccountId;
        public readonly decimal Fund;

        public CashWithdrawn(Guid accountId, decimal fund)
        {
            AccountId = accountId;
            Fund = fund;
        }
    }

    public class WireTransferred : IBaseAccountEvent
    {
        public readonly Guid AccountId;
        public readonly decimal Fund;
        public readonly DateTime WireTransferDate;

        public WireTransferred(Guid accountId, decimal fund, DateTime wireTransferDate)
        {
            AccountId = accountId;
            Fund = fund;
            WireTransferDate = wireTransferDate;
        }
    }

    public class AccountBlocked : IBaseAccountEvent
    {
        public readonly Guid AccountId;
        public readonly string ReasonForAccountBlock;

        public AccountBlocked(Guid accountId, string reasonForAccountBlock)
        {
            AccountId = accountId;
            ReasonForAccountBlock = reasonForAccountBlock;
        }
    }
}
