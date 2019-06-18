namespace AccountBalance.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AccountBalance.Reactive;
    using AccountBalance.Reactive.Events;
    using ReactiveDomain.Foundation;
    using ReactiveDomain.Foundation.EventStore;
    using ReactiveDomain.Messaging.Bus;

    public class AccountRM :
        ReadModelBase,
        IHandle<AccountCreated>,
        IHandle<OverdraftLimitApplied>,
        IHandle<DailyWireTransferLimitApplied>,
        IHandle<ChequeDeposited>,
        IHandle<CashDeposited>,
        IHandle<CashWithdrawn>,
        IHandle<WireTransferred>,
        IHandle<AccountBlocked>
    {
        public Account Account;
        public List<Account> lstAccount;

        public AccountRM(Func<IListener> getListener, IStreamNameBuilder streamNameBuilder)
            : base(getListener)
        {
            lstAccount = new List<Account>();

            EventStream.Subscribe<AccountCreated>(this);
            EventStream.Subscribe<OverdraftLimitApplied>(this);
            EventStream.Subscribe<DailyWireTransferLimitApplied>(this);
            EventStream.Subscribe<ChequeDeposited>(this);
            EventStream.Subscribe<CashDeposited>(this);
            EventStream.Subscribe<CashWithdrawn>(this);
            EventStream.Subscribe<WireTransferred>(this);
            EventStream.Subscribe<AccountBlocked>(this);

            var streamName = streamNameBuilder.GenerateForCategory(typeof(AccountBalance.Reactive.Account));
            Start(streamName, null, true);
            //Start<AccountBalance.Reactive.Account>(
            //    id: id,
            //    blockUntilLive: true);
        }

        public void Handle(AccountCreated message)
        {
            Account = new Account
            {
               AccountId = message.AccountId,
               AccountHolderName = message.AccountHolderName
            };

            lstAccount.Add(Account);
        }

        public void Handle(OverdraftLimitApplied message)
        {
            Account = lstAccount.FirstOrDefault(x => x.AccountId == message.AccountId);
            if (Account == null)
            {
                throw new ArgumentException($"Account id({message.AccountId}) not found.");
            }
            Account.OverdraftLimit = message.OverdraftLimit;
            //UpdateAccountList(Account);
        }

        public void Handle(DailyWireTransferLimitApplied message)
        {
            Account = lstAccount.FirstOrDefault(x => x.AccountId == message.AccountId);
            if (Account == null)
            {
                throw new ArgumentException($"Account id({message.AccountId}) not found.");
            }
            Account.DailyWireTransferLimit = message.DailyWireTransferLimit;
            //UpdateAccountList(Account);
        }

        public void Handle(ChequeDeposited message)
        {
            if (Account == null)
            {
                throw new ArgumentException($"Account id({message.AccountId}) not found.");
            }

            if (message.ClearanceBusinessDay.Date < DateTime.Today.Date)
            {
                Account.AvailableFund = Account.AvailableFund + message.Fund;
                if (Account.AccountState == AccountState.Blocked) Account.AccountState = AccountState.Unblocked;
            }
            else if (message.ClearanceBusinessDay.Date == DateTime.Today.Date &&
                     DateTime.Now.TimeOfDay >= Convert.ToDateTime("09:00:00 AM").TimeOfDay)
            {
                Account.AvailableFund = Account.AvailableFund + message.Fund;
                if (Account.AccountState == AccountState.Blocked) Account.AccountState = AccountState.Unblocked;
            }
            //UpdateAccountList(Account);
        }

        public void Handle(CashDeposited message)
        {
            if (Account == null)
            {
                throw new ArgumentException($"Account id({message.AccountId}) not found.");
            }

            Account.AvailableFund = Account.AvailableFund + message.Fund;
            if (Account.AccountState == AccountState.Blocked) Account.AccountState = AccountState.Unblocked;

            //UpdateAccountList(Account);
        }

        public void Handle(CashWithdrawn message)
        {
            if (Account == null)
            {
                throw new ArgumentException($"Account id({message.AccountId}) not found.");
            }

            Account.AvailableFund = Account.AvailableFund - message.Fund;

            //UpdateAccountList(Account);
        }

        public void Handle(WireTransferred message)
        {
            if (Account == null)
            {
                throw new ArgumentException($"Account id({message.AccountId}) not found.");
            }

            Account.AvailableFund = Account.AvailableFund - message.Fund;

            //UpdateAccountList(Account);
        }

        public void Handle(AccountBlocked message)
        {
            if (Account == null)
            {
                throw new ArgumentException($"Account id({message.AccountId}) not found.");
            }

            Account.AccountState = AccountState.Blocked;
            Account.ReasonForAccountBlock = message.ReasonForAccountBlock;

            //UpdateAccountList(Account);
        }
        public void UpdateAccountList(Account account)
        {
            var index = lstAccount.FindIndex(x => x.AccountId == account.AccountId);
            lstAccount[index] = Account;
        }
    }

    public class Account
    {
        public Guid AccountId { get; set; }
        public string AccountHolderName { get; set; }
        public decimal OverdraftLimit { get; set; }
        public decimal DailyWireTransferLimit { get; set; }
        public decimal AvailableFund { get; set; }
        //public decimal ChequeDepositFund { get; set; }
        //public DateTime DepositDate { get; set; }
        //public DateTime ClearanceBusinessDay { get; set; }
        //public decimal CashDepositFund { get; set; }
        //public decimal CashWithdrawnFund { get; set; }
        //public decimal WireTransferFund { get; set; }
        //public DateTime WireTransferDate { get; set; }
        public string ReasonForAccountBlock { get; set; }
        public AccountState AccountState { get; set; }
    }
}
