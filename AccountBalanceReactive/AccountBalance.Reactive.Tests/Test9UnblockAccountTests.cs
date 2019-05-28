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
        [InlineData(500.00, 100.00)]
        public async Task Can_unblock_account_on_cash_deposit(decimal depositFund, decimal fundToWithraw)
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

            var cashDeposited = new CashDeposited(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = depositFund
            };

            var withdrawCash = new WithdrawCash()
            {
                AccountId = _accountId,
                Fund = fundToWithraw
            };

            var cashWithdrawn = new CashWithdrawn(withdrawCash)
            {
                AccountId = withdrawCash.AccountId,
                Fund = withdrawCash.Fund
            };

            var givens = new Event[] { accountCreated, accountBlocked, cashDeposited };

            await _runner.Run(def => def.Given(givens).When(withdrawCash).Then(cashWithdrawn));
        }

        [Theory]
        [InlineData(500.00, 100.00)]
        public async Task Can_unblock_account_on_cheque_fund_made_available(decimal depositFund, decimal fundToWithraw)
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

            var chequeDeposited = new ChequeDeposited(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = depositFund,
                DepositDate = new DateTime(2019, 04, 15, 10, 00, 00),
                ClearanceBusinessDay = new DateTime(2019, 04, 16, 10, 00, 00)
            };

            var withdrawCash = new WithdrawCash()
            {
                AccountId = _accountId,
                Fund = fundToWithraw
            };

            var cashWithdrawn = new CashWithdrawn(withdrawCash)
            {
                AccountId = withdrawCash.AccountId,
                Fund = withdrawCash.Fund
            };

            var givens = new Event[] { accountCreated, accountBlocked, chequeDeposited };

            await _runner.Run(def => def.Given(givens).When(withdrawCash).Then(cashWithdrawn));
        }
    }
}
