using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBalance.Reactive.Tests
{
    using AccountBalance.Reactive.Commands;
    using AccountBalance.Reactive.Events;
    using AccountBalance.Reactive.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public class BlockAccountTests : IDisposable
    {
        readonly Guid _accountId;
        readonly string _accountHolderName;
        readonly EventStoreScenarioRunner<Account> _runner;

        public BlockAccountTests(EventStoreFixture fixture)
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
        [InlineData(100.00, 500.00, 700.00)]
        public async Task Can_block_account_on_cash_withdrawing_more_than_overdraft_limit(decimal availableFund,
            decimal overdraftLimit, decimal fundToWithdraw)
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

            var accountBlocked = new AccountBlocked(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                ReasonForAccountBlock = "Overdraft limit breached"
            };

            var givens = new Event[] { accountCreated, overdraftLimitApplied, cashDeposited };

            await _runner.Run(def => def.Given(givens).When(withdrawCash).Then(accountBlocked));
        }

        [Theory]
        [InlineData(100.00, 1000.00, 500.00, 700.00)]
        public async Task Can_block_account_on_wire_transferring_more_than_overdraft_limit(decimal availableFund,
            decimal wireTransferLimit, decimal overdraftLimit, decimal fundToWireTransfer)
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

            var dailyWireTransferLimitApplied = new DailyWireTransferLimitApplied(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                DailyWireTransferLimit = wireTransferLimit
            };

            var cashDeposited = new CashDeposited(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = availableFund
            };

            var wireTransfer = new WireTransfer()
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today
            };

            var accountBlocked = new AccountBlocked(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                ReasonForAccountBlock = "Overdraft limit breached"
            };

            var givens = new Event[] { accountCreated, overdraftLimitApplied, dailyWireTransferLimitApplied, cashDeposited };

            await _runner.Run(def => def.Given(givens).When(wireTransfer).Then(accountBlocked));
        }
    }
}
