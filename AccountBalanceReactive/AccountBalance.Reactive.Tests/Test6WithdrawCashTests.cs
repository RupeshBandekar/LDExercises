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
    public class Test6WithdrawCashTests
    {

        readonly Guid _accountId;
        readonly string _accountHolderName;
        readonly EventStoreScenarioRunner<Account> _runner;

        public Test6WithdrawCashTests(EventStoreFixture fixture)
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
        [InlineData(1000.00, 500.00)]
        public async Task Can_withdraw_cash(decimal availableFund, decimal fundToWithdraw)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var cashDeposited = new CashDeposited(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = availableFund
            };

            var withdrawCash = new WithdrawCash()
            {
                AccountId = _accountId,
                Fund = fundToWithdraw
            };

            var cashWithdrawn = new CashWithdrawn(withdrawCash)
            {
                AccountId = withdrawCash.AccountId,
                Fund = withdrawCash.Fund
            };

            var givens = new Event[]{accountCreated, cashDeposited};

            await _runner.Run(def => def.Given(givens).When(withdrawCash).Then(cashWithdrawn));
        }

        [Theory]
        [InlineData(1000.00)]
        public async Task Can_throw_exception_on_withdraw_from_zero_balance(decimal fundToWithdraw)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var withdrawCash = new WithdrawCash()
            {
                AccountId = _accountId,
                Fund = fundToWithdraw
            };

            await _runner.Run(def => def.Given(accountCreated).When(withdrawCash).Throws(new OperationCanceledException("Insufficient fund")));
        }

        [Theory]
        [InlineData(500.00, 500.00)]
        public async Task Can_remove_fund_immediately_on_withdrawing_cash(decimal availableFund, decimal fundToWithdraw)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var cashDeposited = new CashDeposited(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = availableFund
            };

            var cashWithdrawnFirstTime = new CashWithdrawn(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = fundToWithdraw
            };

            var withdrawCash = new WithdrawCash()
            {
                AccountId = _accountId,
                Fund = fundToWithdraw
            };

            var cashWithdrawnSecondTime = new CashWithdrawn(withdrawCash)
            {
                AccountId = withdrawCash.AccountId,
                Fund = withdrawCash.Fund
            };

            var givens = new Event[] { accountCreated, cashDeposited, cashWithdrawnFirstTime };

            await _runner.Run(def => def.Given(givens).When(withdrawCash).Throws(new OperationCanceledException("Insufficient fund")));
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        public async Task Can_throw_exception_on_withdrawing_invalid_cash(decimal fundToWithdraw)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var withdrawCash = new WithdrawCash()
            {
                AccountId = _accountId,
                Fund = fundToWithdraw
            };

            await _runner.Run(def => def.Given(accountCreated).When(withdrawCash).Throws(new ArgumentException("Invalid fund value")));
        }

        [Theory]
        [InlineData(500.00, 1000.00)]
        public async Task Can_throw_exception_on_withdraw_from_blocked_account(decimal overdraftLimit, decimal fundToWithdraw)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var overdraftLimitApplied = new OverdraftLimitApplied(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            var withdrawCash = new WithdrawCash()
            {
                AccountId = _accountId,
                Fund = fundToWithdraw
            };

            var accountBlocked = new AccountBlocked(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                ReasonForAccountBlock = "Overdraft limit breached"
            };

            var withdrawCashSecondTime = new WithdrawCash()
            {
                AccountId = _accountId,
                Fund = fundToWithdraw
            };

            var givens = new Event[] { accountCreated, overdraftLimitApplied,  accountBlocked};

            await _runner.Run(def => def.Given(givens).When(withdrawCashSecondTime).Throws(new OperationCanceledException("Account is blocked")));
        }
    }
}
