namespace AccountBalanceDomain.Commands
{
    using System;
    using ReactiveDomain.Messaging;
    using ReactiveDomain.Messaging.Bus;
    using ReactiveDomain.Foundation;
    using Aggregate;
    public class AccountCommandHandler : 
        IHandleCommand<CreateAccount>,
        IHandleCommand<SetOverdraftLimit>
    {
        private readonly IRepository _repo;
        private readonly ICommandBus _inputBus;

        public AccountCommandHandler(ICommandBus inputBus, IRepository repo)
        {
            _repo = repo;
            _inputBus = inputBus;

            _inputBus.Subscribe<CreateAccount>(this);
            _inputBus.Subscribe<SetOverdraftLimit>(this);
        }
        public CommandResponse Handle(CreateAccount command)
        {
            if (_repo.TryGetById<Account>(command.AccountId, out Account account) && account != null)
            {
                throw new InvalidOperationException($"Account Id({command.AccountId}) already exists.");
            }

            account = Account.CreateAccount(
                accountId: command.AccountId,
                accountHolderName: command.AccountHolderName,
                source: command);

            _repo.Save(account);
            return command.Succeed();
        }

        public CommandResponse Handle(SetOverdraftLimit command)
        {
            if (_repo.TryGetById<Account>(command.AccountId, out Account account) && account != null)
            {
                account.SetOverdraftLimit(
                    accountId: command.AccountId,
                    overdraftLimit: command.OverdraftLimit,
                    source: command);

                _repo.Save(account);
                return command.Succeed();
            }

            return command.Fail();
        }
    }
}
