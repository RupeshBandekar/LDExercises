using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;

namespace AccountBalanceDomain.ReadModel
{
    public class AccountRM { }
    //public class AccountRM : ReadModelBase, IHandle<AccountCreated>
    //{
    //    public Account Account;

    //    public AccountRM(Func<IListener> getListener, Guid id) : base("AccountListener", getListener)
    //    {
    //        EventStream.Subscribe<AccountCreated>(this);

    //        Start<Aggregate.Account>(
    //            id: id,
    //            blockUntilLive: true);
    //    }

    //    public void Handle(AccountCreated evt)
    //    {
    //        Account = new Account
    //        {
    //            AccountId = evt.AccountId,
    //            AccountHolderName = evt.AccountHolderName
    //        };
    //    }
    //}

    //public class Account
    //{
    //    public Guid AccountId { get; set; }
    //    public string AccountHolderName { get; set; }
    //}
}
