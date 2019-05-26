namespace AccountBalance.Reactive
{
    using System;
    using AccountBalance.Reactive.Events;
    using NodaTime;
    using ReactiveDomain;
    using ReactiveDomain.Messaging;

    public sealed class Account : EventDrivenStateMachine
    {
        private AccountState _state;
        private decimal _overdraftLimit;
        private decimal _dailyWireTransferLimit;
        private decimal _dailyWireTransferLimitUtilization;
        private decimal _availableFund;

        Account()
        {
            Register<AccountCreated>(e => { Id = e.AccountId; });
            Register<OverdraftLimitApplied>(e => { _overdraftLimit = e.OverdraftLimit; });
            Register<DailyWireTransferLimitApplied>(e => { _dailyWireTransferLimit = e.DailyWireTransferLimit; });
            Register<ChequeDeposited>(
                e =>
                {
                    if (e.ClearanceBusinessDay.Date < DateTime.Today.Date)
                    {
                        _availableFund = _availableFund + e.Fund;
                    }
                    else if (e.ClearanceBusinessDay.Date == DateTime.Today.Date &&
                             DateTime.Now.TimeOfDay >= Convert.ToDateTime("09:00:00 AM").TimeOfDay)
                    {
                        _availableFund = _availableFund + e.Fund;
                    }
                });
            Register<AccountBlocked>(e => { _state = AccountState.Blocked; });
            Register<AccountUnblocked>(e => { _state = AccountState.Unblocked; });
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

        public void SetOverdraftLimit(decimal overdraftLimit, CorrelatedMessage source)
        {
            if (overdraftLimit <= 0)
                throw new ArgumentException("Invalid Overdraft Limit");

            Raise(
                new OverdraftLimitApplied(source)
                {
                    AccountId = Id,
                    OverdraftLimit = overdraftLimit
                });
        }

        public void SetDailyWireTransferLimit(decimal dailyWireTransferLimit, CorrelatedMessage source)
        {
            if (dailyWireTransferLimit <= 0)
                throw new ArgumentException("Invalid Daily Wire Transfer Limit");

            Raise(
                new DailyWireTransferLimitApplied(source)
                {
                    AccountId = Id,
                    DailyWireTransferLimit = dailyWireTransferLimit
                });
        }

        public void DepositCheque(decimal depositFund, DateTime depositDate, CorrelatedMessage source)
        {
            if (depositFund <= 0)
                throw new ArgumentException("Invalid fund value");

            Raise(
                new ChequeDeposited(source)
                {
                    AccountId = Id,
                    Fund = depositFund,
                    DepositDate = depositDate,
                    ClearanceBusinessDay = GetChequeClearanceDay(depositDate)
                });

            if (_state == AccountState.Blocked)
            {
                Raise(
                    new AccountUnblocked(source)
                    {
                        AccountId = Id
                    });
            }
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
    }
    public enum AccountState
    {
        Unblocked = 0,
        Blocked = 1
    }
    public enum BlockAccountReasonType
    {
        OverdraftLimitBreach = 0,
        DailyWireTransferLimitBreach = 1
    }
}
