using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace AccountBalanceDomain
{
    public class Account
    {
        private readonly List<IBaseAccountEvent> _allAccountEvents;
        private readonly Guid _accountId;
        private AccountState _state;
        private decimal _overdraftLimit;
        private decimal _dailyWireTransferLimit;
        private decimal _dailyWireTransferLimitUtilization;
        private decimal _availableFund;

        public Account(Guid accountId)
        {
            _accountId = accountId;
            _allAccountEvents = new List<IBaseAccountEvent>();
        }

        public static Account Create(Guid accountId, string accountHolderName)
        {
            if(accountId == Guid.Empty)
                throw new ArgumentException("Invalid Account ID");

            if (string.IsNullOrWhiteSpace(accountHolderName))
                throw new ArgumentException("Invalid Account Holder Name");

            Account account = new Account(accountId);
            var accountCreated = new AccountCreated(accountId, accountHolderName);
            account._allAccountEvents.Add(accountCreated);
            account.ApplyAccountEvents(accountCreated);

            return account;
        }

        public void SetOverdraftLimit(decimal overdraftLimit)
        {
            if (overdraftLimit <= 0)
                throw new ArgumentException("Invalid Overdraft Limit");

            var overdraftLimitApplied = new OverdraftLimitApplied(overdraftLimit);
            _allAccountEvents.Add(overdraftLimitApplied);
            ApplyAccountEvents(overdraftLimitApplied);
        }

        public void SetDailyWireTransferLimit(decimal dailyWireTransferLimit)
        {
            if (dailyWireTransferLimit <= 0)
                    throw new ArgumentException("Invalid Daily Wire Transfer Limit");

            var dailyWireTransferLimitApplied = new DailyWireTransferLimitApplied(dailyWireTransferLimit);
            _allAccountEvents.Add(dailyWireTransferLimitApplied);
            ApplyAccountEvents(dailyWireTransferLimitApplied);
        }

        public void DepositCheque(decimal depositFund, DateTime depositDate, int chequeNumber)
        {
            if (depositFund <= 0)
                throw new Exception("Invalid fund value");
            
            var clearanceBusinessDay = GetChequeClearanceDay(depositDate);
            var chequeDeposited = new ChequeDeposited(depositFund, depositDate, chequeNumber, clearanceBusinessDay);
            _allAccountEvents.Add(chequeDeposited);
        }

        public void MakeFundAvailableForDepositedCheque(DateTime clearanceBusinessDay)
        {
            var chequesToClear = GetEvents().Where(x => x.GetType() == typeof(ChequeDeposited)).ToList()
                .Where(x => ((ChequeDeposited) x).ClearanceBussinessDay.Date == clearanceBusinessDay.Date);

            var chequesCleared = GetEvents().Where(x => x.GetType() == typeof(ChequeFundMadeAvailable)).ToList()
                .Where(x => ((ChequeFundMadeAvailable)x).IsCleared == true);

            chequesToClear = chequesToClear.Where(x =>
                !chequesCleared.Any(p =>
                    ((ChequeDeposited) x).ChequeNumber == ((ChequeFundMadeAvailable) p).ChequeNumber)).ToList();

            foreach (ChequeDeposited cheque in chequesToClear)
            {
                var chequeCleared = new ChequeFundMadeAvailable(cheque.Fund, cheque.ChequeNumber, true);
                _allAccountEvents.Add(chequeCleared);
                ApplyAccountEvents(chequeCleared);

                if (_state == AccountState.Blocked)
                {
                    UnblockAccount();
                }
            }
        }

        public DateTime GetChequeClearanceDay(DateTime depositDate)
        {
            DateTime chequeClearanceDay = new DateTime();
            var businessDayStartTime = DateTime.Parse("01/01/1976 09:00:00 AM");
            var businessDayEndTime = DateTime.Parse("01/01/1976 05:00:00 PM");
            if (DayOfWeek.Monday <= depositDate.DayOfWeek && depositDate.DayOfWeek <= DayOfWeek.Thursday) //Deposit between Monday to Thursday
            {
                if (depositDate.TimeOfDay <= businessDayEndTime.TimeOfDay) //Deposit before business end time (05:00 PM)
                {
                    chequeClearanceDay = depositDate.AddDays(1); //return Tuesday for Monday and so on
                }
                else if (depositDate.DayOfWeek == DayOfWeek.Thursday &&
                         depositDate.TimeOfDay > businessDayEndTime.TimeOfDay) //Deposit on Thursday after business time (after 05:00 PM)
                {
                    chequeClearanceDay = depositDate.AddDays(4); //return Monday
                }
                else if (depositDate.TimeOfDay > businessDayEndTime.TimeOfDay) //Deposit after business end time (05:00 PM) Monday to Wednesday
                {
                    chequeClearanceDay = depositDate.AddDays(2); //return Wednesday for Monday and so on
                }
            }
            else if (depositDate.DayOfWeek == DayOfWeek.Friday &&
                     depositDate.TimeOfDay <= businessDayEndTime.TimeOfDay) //Deposit on Friday before business end time (05:00 PM)
            {
                chequeClearanceDay = depositDate.AddDays(3); //return Monday
            }
            else if ((depositDate.DayOfWeek == DayOfWeek.Friday &&
                      depositDate.TimeOfDay > businessDayEndTime.TimeOfDay) ||
                     depositDate.DayOfWeek == DayOfWeek.Saturday || depositDate.DayOfWeek == DayOfWeek.Sunday)
            {
                if (depositDate.DayOfWeek == DayOfWeek.Friday)
                    chequeClearanceDay = depositDate.AddDays(4); //return Tuesday
                else if (depositDate.DayOfWeek == DayOfWeek.Saturday)
                    chequeClearanceDay = depositDate.AddDays(3); //return Tuesday
                else if (depositDate.DayOfWeek == DayOfWeek.Sunday)
                    chequeClearanceDay = depositDate.AddDays(2); //return Tuesday
            }

            return chequeClearanceDay;
        }

        public void DepositCash(decimal fund)
        {
            if(fund <= 0)
                throw new ArgumentException("Invalid fund value");

            var cashDeposited = new CashDeposited(fund);
            _allAccountEvents.Add(cashDeposited);
            ApplyAccountEvents(cashDeposited);

            if (_state == AccountState.Blocked)
            {
                UnblockAccount();
            }
        }

        public void WithdrawCash(decimal fundToWithdraw)
        {
            if (fundToWithdraw <= 0)
                throw new ArgumentException("Invalid fund value");

            MakeFundAvailableForDepositedCheque(DateTime.Now);

            if (_state == AccountState.Blocked)
                throw new OperationCanceledException("Account is blocked");

            if ((_availableFund + _overdraftLimit - fundToWithdraw) < 0)
            {
                BlockAccount(BlockAccountReasonType.OverdraftLimitBreach);
                throw new OperationCanceledException("Account is blocked");
            }
            else
            {
                var cashWithdrew = new CashWithdrew(fundToWithdraw);
                _allAccountEvents.Add(cashWithdrew);
                ApplyAccountEvents(cashWithdrew);
            }
        }

        public void WireTransfer(decimal fundToWithdraw)
        {
            if (fundToWithdraw <= 0)
                throw new ArgumentException("Invalid fund value");

            MakeFundAvailableForDepositedCheque(DateTime.Now);

            if (_state == AccountState.Blocked)
                throw new OperationCanceledException("Account is blocked");

            if (((_dailyWireTransferLimit - _dailyWireTransferLimitUtilization) - fundToWithdraw) < 0)
            {
                BlockAccount(BlockAccountReasonType.DailyWireTransferLimitBreach);
                throw new OperationCanceledException("Account is blocked");
            }
            else if ((_availableFund + _overdraftLimit - fundToWithdraw) < 0)
            {
                BlockAccount(BlockAccountReasonType.OverdraftLimitBreach);
                throw new OperationCanceledException("Account is blocked");
            }
            else
            {
                var wireTransferred = new WireTransferred(fundToWithdraw);
                _allAccountEvents.Add(wireTransferred);
                ApplyAccountEvents(wireTransferred);
            }
        }

        private void BlockAccount(BlockAccountReasonType blockReasonType)
        {
            AccountBlocked accountBlocked = new AccountBlocked();
            if (blockReasonType == BlockAccountReasonType.OverdraftLimitBreach)
            {
                accountBlocked = new AccountBlockedOverdraftLimitBreach();
            }
            else if (blockReasonType == BlockAccountReasonType.DailyWireTransferLimitBreach)
            {
                accountBlocked = new AccountBlockedDailyWireTransferLimitBreach();
            }

            _allAccountEvents.Add(accountBlocked);
            ApplyAccountEvents(accountBlocked);
        }

        private void UnblockAccount()
        {
            var accountUnblocked = new AccountUnblocked();
            _allAccountEvents.Add(accountUnblocked);
            ApplyAccountEvents(accountUnblocked);
        }

        private void ApplyAccountEvents(IBaseAccountEvent e)
        {
            if (e.GetType() == typeof(AccountCreated))
            {
                _availableFund = 0;
            }
            else if (e.GetType() == typeof(OverdraftLimitApplied))
            {
                _overdraftLimit = ((OverdraftLimitApplied) e).OverdraftLimit;
            }
            else if (e.GetType() == typeof(DailyWireTransferLimitApplied))
            {
                _dailyWireTransferLimit = ((DailyWireTransferLimitApplied) e).DailyWireTransferLimit;
            }
            else if (e.GetType() == typeof(ChequeFundMadeAvailable))
            {
                _availableFund = _availableFund + ((ChequeFundMadeAvailable) e).Fund;
            }
            else if (e.GetType() == typeof(CashDeposited))
            {
                _availableFund = _availableFund + ((CashDeposited) e).Fund;
            }
            else if (e.GetType() == typeof(CashWithdrew))
            {
                _availableFund = _availableFund - ((CashWithdrew) e).Fund;
            }
            else if (e.GetType() == typeof(WireTransferred))
            {
                _availableFund = _availableFund - ((WireTransferred) e).Fund;
                _dailyWireTransferLimitUtilization = _dailyWireTransferLimitUtilization + ((WireTransferred) e).Fund;
            }
            else if (e.GetType() == typeof(AccountBlockedDailyWireTransferLimitBreach) ||
                     e.GetType() == typeof(AccountBlockedOverdraftLimitBreach))
            {
                _state = AccountState.Blocked;
            }
            else if (e.GetType() == typeof(AccountUnblocked))
            {
                _state = AccountState.Unblocked;
            }
        }

        public List<IBaseAccountEvent> GetEvents()
        {
            return _allAccountEvents;
        }
    }

    public enum BlockAccountReasonType
    {
        OverdraftLimitBreach = 0,
        DailyWireTransferLimitBreach = 1
    }

    public enum AccountState
    {
        Unblocked = 0,
        Blocked = 1
    }
}
