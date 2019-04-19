using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AccountBalanceDomain
{
    public interface IBaseAccountEvent
    {
    }

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
        public decimal OverdraftLimit { get; set; }

        public OverdraftLimitApplied(decimal overdraftLimit)
        {
            OverdraftLimit = overdraftLimit;
        }
    }

    public class DailyWireTransferLimitApplied : IBaseAccountEvent
    {
        public decimal DailyWireTransferLimit { get; set; }

        public DailyWireTransferLimitApplied(decimal dailyWireTransferLimit)
        {
            DailyWireTransferLimit = dailyWireTransferLimit;
        }
    }

    public class ChequeDeposited : IBaseAccountEvent
    {
        public decimal Fund { get; set; }
        public DateTime DepositDate { get; set; }
        public int ChequeNumber { get; set; }
        public DateTime ClearanceBussinessDay { get; set; }

        public ChequeDeposited(decimal fund, DateTime depositDate, int chequeNumber, DateTime clearanceBusinessDay)
        {
            Fund = fund;
            DepositDate = depositDate;
            ChequeNumber = chequeNumber;
            ClearanceBussinessDay = clearanceBusinessDay;
        }
    }

    public class ChequeFundMadeAvailable : IBaseAccountEvent
    {
        public decimal Fund { get; set; }
        public int ChequeNumber { get; set; }
        public bool IsCleared { get; set; }

        public ChequeFundMadeAvailable(decimal fund, int chequeNumber, bool isCleared)
        {
            Fund = fund;
            ChequeNumber = chequeNumber;
            IsCleared = isCleared;
        }
    }

    public class CashDeposited : IBaseAccountEvent
    {
        public decimal Fund { get; set; }

        public CashDeposited(decimal fund)
        {
            Fund = fund;
        }
    }

    public class CashWithdrew : IBaseAccountEvent
    {
        public decimal Fund;

        public CashWithdrew(decimal fund)
        {
            Fund = fund;
        }
    }

    public class WireTransferred : IBaseAccountEvent
    {
        public decimal Fund;

        public WireTransferred(decimal fund)
        {
            Fund = fund;
        }
    }

    public class AccountBlocked : IBaseAccountEvent
    {
        public AccountState AccountState;

        public AccountBlocked()
        {
            AccountState = AccountState.Blocked;
        }
    }

    public class AccountBlockedOverdraftLimitBreach : AccountBlocked
    {
    }

    public class AccountBlockedDailyWireTransferLimitBreach : AccountBlocked
    {
    }

    public class AccountUnblocked : IBaseAccountEvent
    {
        public AccountState AccountState;

        public AccountUnblocked()
        {
            AccountState = AccountState.Unblocked;
        }
    }
}
