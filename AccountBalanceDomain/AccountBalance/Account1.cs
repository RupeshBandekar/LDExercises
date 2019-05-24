namespace AccountBalanceDomain
{
    using System;
    using System.Collections.Generic;
    public class Account1
    {
        private readonly List<IBaseAccountEvent> _allAccountEvents;
        private readonly Guid _accountId;
        private AccountState _state;
        private decimal _overdraftLimit;
        private decimal _dailyWireTransferLimit;
        private decimal _dailyWireTransferLimitUtilization;
        private decimal _availableFund;
        

        public Account1(Guid accountId)
        {
            _accountId = accountId;
            _allAccountEvents = new List<IBaseAccountEvent>();
        }

        public static Account1 CreateAccount(Guid accountId, string accountHolderName)
        {
            if(accountId == Guid.Empty)
                throw new ArgumentException("Invalid Account ID");

            if (string.IsNullOrWhiteSpace(accountHolderName))
                throw new ArgumentException("Invalid Account Holder Name");

            Account1 account = new Account1(accountId);
            var accountCreated = new AccountCreated(accountId, accountHolderName);
            account._allAccountEvents.Add(accountCreated);
            account.ApplyAccountEvents(accountCreated);

            return account;
        }

        public void SetOverdraftLimit(decimal overdraftLimit)
        {
            if (overdraftLimit <= 0)
                throw new ArgumentException("Invalid Overdraft Limit");

            var overdraftLimitApplied = new OverdraftLimitApplied(_accountId, overdraftLimit);
            _allAccountEvents.Add(overdraftLimitApplied);
            ApplyAccountEvents(overdraftLimitApplied);
        }

        public void SetDailyWireTransferLimit(decimal dailyWireTransferLimit)
        {
            if (dailyWireTransferLimit <= 0)
                    throw new ArgumentException("Invalid Daily Wire Transfer Limit");

            var dailyWireTransferLimitApplied = new DailyWireTransferLimitApplied(_accountId, dailyWireTransferLimit);
            _allAccountEvents.Add(dailyWireTransferLimitApplied);
            ApplyAccountEvents(dailyWireTransferLimitApplied);
        }

        public void DepositCheque(decimal depositFund, DateTime depositDate)
        {
            if (depositFund <= 0)
                throw new ArgumentException("Invalid fund value");
            
            var clearanceBusinessDay = GetChequeClearanceDay(depositDate);
            var chequeDeposited = new ChequeDeposited(_accountId, depositFund, depositDate, clearanceBusinessDay);
            _allAccountEvents.Add(chequeDeposited);
            ApplyAccountEvents(chequeDeposited);

            if (_state == AccountState.Blocked)
                UnblockAccount();
        }

        private DateTime GetChequeClearanceDay(DateTime depositDate)
        {
            //Scenarios covered as per Deposit Day
            //|Deposit Day	|Deposit Time		|Clearance Day
            //|Monday		|Before 05:00PM		|Tuesday
            //|Monday		|09:00AM - 05:00PM	|Tuesday
            //|Monday		|After 05:00PM		|Wednesday
            //|Thursday		|After 05:00PM		|Monday
            //|Friday		|09:00AM - 05:00PM	|Monday
            //|Friday		|After 05:00PM		|Tuesday
            //|Saturday		|12:00AM-11:59PM	|Tuesday
            //|Sunday		|12:00AM-11:59PM	|Tuesday

            DateTime chequeClearanceDay = new DateTime();
            var businessDayEndTime = DateTime.Parse("01/01/1976 05:00:00 PM");
            //Deposit between Monday to Thursday
            if (DayOfWeek.Monday <= depositDate.DayOfWeek && depositDate.DayOfWeek <= DayOfWeek.Thursday) 
            {
                //Deposit before business end time (05:00 PM)
                if (depositDate.TimeOfDay <= businessDayEndTime.TimeOfDay) 
                {
                    chequeClearanceDay = depositDate.AddDays(1); //return Tuesday for Monday and so on
                }
                //Deposit on Thursday after business time (after 05:00 PM)
                else if (depositDate.DayOfWeek == DayOfWeek.Thursday &&
                         depositDate.TimeOfDay > businessDayEndTime.TimeOfDay) 
                {
                    chequeClearanceDay = depositDate.AddDays(4); //return Monday
                }
                //Deposit after business end time (05:00 PM) Monday to Wednesday
                else if (depositDate.TimeOfDay > businessDayEndTime.TimeOfDay) 
                {
                    chequeClearanceDay = depositDate.AddDays(2); //return Wednesday for Monday and so on
                }
            }
            //Deposit on Friday before business end time (05:00 PM)
            else if (depositDate.DayOfWeek == DayOfWeek.Friday &&
                     depositDate.TimeOfDay <= businessDayEndTime.TimeOfDay) 
            {
                chequeClearanceDay = depositDate.AddDays(3); //return Monday
            }
            //Deposit on Friday after business end time (05:00 PM)
            else if (depositDate.DayOfWeek == DayOfWeek.Friday &&
                     depositDate.TimeOfDay > businessDayEndTime.TimeOfDay)
            {
                chequeClearanceDay = depositDate.AddDays(4); //return Tuesday
            }
            //Deposit on Saturday
            else if (depositDate.DayOfWeek == DayOfWeek.Saturday)
            {
                chequeClearanceDay = depositDate.AddDays(3); //return Tuesday
            }
            //Deposit on Sunday
            else if (depositDate.DayOfWeek == DayOfWeek.Sunday)
            {
                chequeClearanceDay = depositDate.AddDays(2); //return Tuesday
            }

            return chequeClearanceDay;
        }

        public void DepositCash(decimal fund)
        {
            if(fund <= 0)
                throw new ArgumentException("Invalid fund value");

            var cashDeposited = new CashDeposited(_accountId, fund);
            _allAccountEvents.Add(cashDeposited);
            ApplyAccountEvents(cashDeposited);

            if (_state == AccountState.Blocked)
                UnblockAccount();
        }

        public void WithdrawCash(decimal fundToWithdraw)
        {
            if (fundToWithdraw <= 0)
                throw new ArgumentException("Invalid fund value");

            if (_state == AccountState.Blocked)
                throw new OperationCanceledException("Account is blocked");

            if (_overdraftLimit == 0 && _availableFund - fundToWithdraw < 0)
            {
                throw new OperationCanceledException("Fund insufficient");
            }
            else if ((_availableFund + _overdraftLimit - fundToWithdraw) < 0)
            {
                BlockAccount(BlockAccountReasonType.OverdraftLimitBreach);
                throw new OperationCanceledException("Account is blocked");
            }
            else
            {
                var cashWithdrew = new CashWithdrawn(_accountId, fundToWithdraw);
                _allAccountEvents.Add(cashWithdrew);
                ApplyAccountEvents(cashWithdrew);
            }
        }

        public void WireTransfer(decimal fundToWireTransfer, DateTime wireTransferDate)
        {
            if (fundToWireTransfer <= 0)
                throw new ArgumentException("Invalid fund value");

            if (_state == AccountState.Blocked)
                throw new OperationCanceledException("Account is blocked");

            if (_overdraftLimit == 0 && _availableFund - fundToWireTransfer < 0)
            {
                throw new OperationCanceledException("Fund insufficient");
            }

            if ((_availableFund + _overdraftLimit - fundToWireTransfer) < 0)
            {
                BlockAccount(BlockAccountReasonType.OverdraftLimitBreach);
                throw new OperationCanceledException("Account is blocked");
            }

            if (((_dailyWireTransferLimit - _dailyWireTransferLimitUtilization) - fundToWireTransfer) < 0)
            {
                BlockAccount(BlockAccountReasonType.DailyWireTransferLimitBreach);
                throw new OperationCanceledException("Account is blocked");
            }

            var wireTransferred = new WireTransferred(_accountId, fundToWireTransfer, wireTransferDate);
            _allAccountEvents.Add(wireTransferred);
            ApplyAccountEvents(wireTransferred);

        }

        private void BlockAccount(BlockAccountReasonType blockReasonType)
        {
            AccountBlocked accountBlocked = null;
            if (blockReasonType == BlockAccountReasonType.OverdraftLimitBreach)
            {
                accountBlocked = new AccountBlocked(_accountId, "Overdraft limit breached");
            }
            else if (blockReasonType == BlockAccountReasonType.DailyWireTransferLimitBreach)
            {
                accountBlocked = new AccountBlocked(_accountId, "Daily wire transfer limit breached");
            }

            _allAccountEvents.Add(accountBlocked);
            ApplyAccountEvents(accountBlocked);
        }

        private void UnblockAccount()
        {
            var accountUnblocked = new AccountUnblocked(_accountId);
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
                DailyWireTransferLimitApplied tEvent = (DailyWireTransferLimitApplied) e;
                _dailyWireTransferLimit = tEvent.DailyWireTransferLimit;
            }
            else if (e.GetType() == typeof(ChequeDeposited))
            {
                ChequeDeposited tEvent = (ChequeDeposited) e;
                if (tEvent.ClearanceBusinessDay.Date < DateTime.Today.Date)
                {
                    _availableFund = _availableFund + tEvent.Fund;
                }
                else if (tEvent.ClearanceBusinessDay.Date == DateTime.Today.Date &&
                         DateTime.Now.TimeOfDay >= Convert.ToDateTime("09:00:00 AM").TimeOfDay)
                {
                    _availableFund = _availableFund + tEvent.Fund;
                }
            }
            else if (e.GetType() == typeof(CashDeposited))
            {
                _availableFund = _availableFund + ((CashDeposited) e).Fund;
            }
            else if (e.GetType() == typeof(CashWithdrawn))
            {
                _availableFund = _availableFund - ((CashWithdrawn) e).Fund;
            }
            else if (e.GetType() == typeof(WireTransferred))
            {
                WireTransferred tEvent = (WireTransferred) e;
                _availableFund = _availableFund - tEvent.Fund;

                if (tEvent.WireTransferDate == DateTime.Today.Date)
                {
                    _dailyWireTransferLimitUtilization = _dailyWireTransferLimitUtilization + tEvent.Fund;
                }
            }
            else if (e.GetType() == typeof(AccountBlocked))
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
