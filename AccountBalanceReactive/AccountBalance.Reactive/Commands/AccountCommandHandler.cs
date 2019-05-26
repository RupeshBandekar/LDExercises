namespace AccountBalance.Reactive
{
    using System;
    using AccountBalance.Reactive.Commands;
    using ReactiveDomain.Foundation;
    using ReactiveDomain.Messaging;
    using ReactiveDomain.Messaging.Bus;

    public sealed class AccountCommandHandler
        : IHandleCommand<CreateAccount>,
            IHandleCommand<SetOverdraftLimit>,
            IHandleCommand<SetDailyWireTransferLimit>,
            IHandleCommand<DepositeCheque>,
            IHandleCommand<DepositCash>,
            IHandleCommand<WithdrawCash>,
            IDisposable
    {
        readonly IRepository _repository;
        readonly IDisposable _disposable;

        public AccountCommandHandler(IRepository repository, ICommandSubscriber dispatcher)
        {
            _repository = repository;

            _disposable = new CompositeDisposable
            {
                dispatcher.Subscribe<CreateAccount>(this),
                dispatcher.Subscribe<SetOverdraftLimit>(this),
                dispatcher.Subscribe<SetDailyWireTransferLimit>(this),
                dispatcher.Subscribe<DepositeCheque>(this),
                dispatcher.Subscribe<DepositCash>(this),
                dispatcher.Subscribe<WithdrawCash>(this)
            };
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        public CommandResponse Handle(CreateAccount command)
        {
            try
            {
                if (_repository.TryGetById<Account>(command.AccountId, out var _))
                    throw new ValidationException($"An account with ID {command.AccountId} already exists");

                var account = Account.CreateAccount(command.AccountId, command.AccountHolderName, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception ex)
            {
                return command.Fail(ex);
            }
        }

        public CommandResponse Handle(SetOverdraftLimit command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException($"An account with ID {account.Id} does not exists");

                account.SetOverdraftLimit(command.OverdraftLimit, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception ex)
            {
                return command.Fail(ex);
            }
        }

        public CommandResponse Handle(SetDailyWireTransferLimit command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException($"An account with ID {account.Id} does not exists");

                account.SetDailyWireTransferLimit(command.DailyWireTransferLimit, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception ex)
            {
                return command.Fail(ex);
            }
        }

        public CommandResponse Handle(DepositeCheque command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException($"An account with ID {account.Id} does not exists");

                account.DepositCheque(command.Fund, command.DepositDate, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception ex)
            {
                return command.Fail(ex);
            }
        }

        public CommandResponse Handle(DepositCash command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException($"An account with ID {account.Id} does not exists");

                account.DepositCash(command.Fund, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception ex)
            {
                return command.Fail(ex);
            }
        }

        public CommandResponse Handle(WithdrawCash command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException($"An account with ID {account.Id} does not exists");

                account.WithdrawCash(command.Fund, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception ex)
            {
                return command.Fail(ex);
            }
        }
    }
}
