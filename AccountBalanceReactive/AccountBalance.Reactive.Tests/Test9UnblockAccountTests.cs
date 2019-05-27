namespace AccountBalance.Reactive.Tests
{
    using System;
    using System.Threading.Tasks;
    using AccountBalance.Reactive.Commands;
    using AccountBalance.Reactive.Events;
    using AccountBalance.Reactive.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit;
    using Xunit.ScenarioReporting;

    [Collection("AccountBalanceTest")]
    public class Test9UnblockAccountTests : IDisposable
    {
        readonly Guid _accountId;
        readonly string _accountHolderName;
        readonly EventStoreScenarioRunner<Account> _runner;

        public Test9UnblockAccountTests(EventStoreFixture fixture)
        {
            _accountId = Guid.NewGuid();
            _accountHolderName = "Test_Account_Holder";
            _runner = new EventStoreScenarioRunner<Account>(
                _accountId,
                fixture,
                (repository, dispatcher) => new AccountCommandHandler(repository, dispatcher));
        }

        public void Dispose()
        {
            _runner.Dispose();
        }

        [Theory]
        [InlineData(50.00)]
        public async Task Can_unblock_account_on_cash_deposit(decimal depositFund)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var accountBlocked = new AccountBlocked(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                ReasonForAccountBlock = "Overdraft limit breached"
            };

            var depositCash = new DepositCash()
            {
                AccountId = _accountId,
                Fund = depositFund
            };

            var cashDeposited = new CashDeposited(depositCash)
            {
                AccountId = depositCash.AccountId,
                Fund = depositFund
            };

            var accountUnblocked = new AccountUnblocked(depositCash)
            {
                AccountId = depositCash.AccountId
            };

            var givens = new Event[] { accountCreated, accountBlocked };
            var thens = new Event[] {cashDeposited, accountUnblocked};

            await _runner.Run(def => def.Given(givens).When(depositCash).Then(thens));
        }

        [Fact]
        public async Task Can_unblock_account_on_cheque_fund_made_available()
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var accountBlocked = new AccountBlocked(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                ReasonForAccountBlock = "Overdraft limit breached"
            };

            var depositCheque = new DepositCheque()
            {
                AccountId = _accountId,
                Fund = 1000,
                DepositDate = new DateTime(2019, 04, 15, 10, 00, 00)
            };

            var chequeDeposited = new ChequeDeposited(depositCheque)
            {
                AccountId = depositCheque.AccountId,
                Fund = depositCheque.Fund,
                DepositDate = depositCheque.DepositDate,
                ClearanceBusinessDay = new DateTime(2019, 04, 16, 10, 00, 00)
            };

            var accountUnblocked = new AccountUnblocked(chequeDeposited)
            {
                AccountId = chequeDeposited.AccountId
            };

            var givens = new Event[] { accountCreated, accountBlocked};
            var thens = new Event[] { chequeDeposited, accountUnblocked };

            await _runner.Run(def => def.Given(givens).When(depositCheque).Then(thens));
        }
    }
}
