using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBalance.Reactive.Tests
{
    using System.Data;
    using AccountBalance.Reactive.Commands;
    using AccountBalance.Reactive.Events;
    using AccountBalance.Reactive.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit;
    using Xunit.ScenarioReporting;
    using Xunit.Sdk;

    [Collection("AggregateTest")]
    public class WireTransferTests : IDisposable
    {
        readonly Guid _accountId;
        readonly string _accountHolderName;
        readonly EventStoreScenarioRunner<Account> _runner;

        public WireTransferTests(EventStoreFixture fixture)
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
        [InlineData(500.00, 1000.00, 500.00)]
        public async Task Can_wire_transfer(decimal availableFund, decimal wireTransferLimit, decimal fundToWireTransfer)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
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

            var wireTransferred = new WireTransferred(wireTransfer)
            {
                AccountId = wireTransfer.AccountId,
                Fund = wireTransfer.Fund,
                WireTransferDate = wireTransfer.WireTransferDate
            };

            var givens = new Event[] { accountCreated, dailyWireTransferLimitApplied, cashDeposited };

            await _runner.Run(def => def.Given(givens).When(wireTransfer).Then(wireTransferred));
        }

        [Theory]
        [InlineData(1000.00)]
        public async Task Can_throw_exception_on_wire_transfer_from_zero_balance(decimal fundToWireTransfer)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var dailyWireTransferLimitApplied = new DailyWireTransferLimitApplied(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                DailyWireTransferLimit = 1000
            };

            var wireTransfer = new WireTransfer()
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today
            };

            var givens = new Event[] { accountCreated, dailyWireTransferLimitApplied };

            await _runner.Run(def => def.Given(givens).When(wireTransfer).Throws(new OperationCanceledException("Insufficient fund")));
        }

        [Theory]
        [InlineData(500.00, 1000.00, 500.00)]
        public async Task Can_remove_fund_immediately_on_wire_transferring(decimal availableFund,
            decimal wireTransferLimit, decimal fundToWireTransfer)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
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

            var wireTransferred = new WireTransferred(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today
            };

            var wireTransferSecondTime = new WireTransfer()
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today
            };

            var givens = new Event[] { accountCreated, dailyWireTransferLimitApplied, cashDeposited, wireTransferred };

            await _runner.Run(def => def.Given(givens).When(wireTransferSecondTime).Throws(new OperationCanceledException("Insufficient fund")));
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        public async Task Can_throw_exception_on_wire_transferring_zero_negative_fund(decimal fundToWireTransfer)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var wireTransfer = new WireTransfer()
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today
            };

            await _runner.Run(def => def.Given(accountCreated).When(wireTransfer).Throws(new ArgumentException("Invalid fund value")));
        }

        [Theory]
        [InlineData(1000.00, 500.00, 500.00)]
        public async Task Can_block_account_on_wire_transferring_more_than_wire_transfer_limit(decimal availableFund,
            decimal wireTransferLimit, decimal fundToWireTransfer)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
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

            var wireTransferred = new WireTransferred(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today
            };

            var wireTransferSecondTime = new WireTransfer()
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today
            };

            var blockAccount = new BlockAccount()
            {
                AccountId = _accountId,
                ReasonForAccountBlock = "Daily wire transfer limit breached"
            };

            var accountBlocked = new AccountBlocked(blockAccount)
            {
                AccountId = blockAccount.AccountId,
                ReasonForAccountBlock = blockAccount.ReasonForAccountBlock
            };

            var givens = new Event[] { accountCreated, dailyWireTransferLimitApplied, cashDeposited, wireTransferred };

            await _runner.Run(def => def.Given(givens).When(wireTransferSecondTime).Then(accountBlocked));
        }

        [Theory]
        [InlineData(1000.00, 500.00, 700.00)]
        public async Task Can_throw_exception_on_wire_transfer_from_blocked_account(decimal availableFund,
            decimal wireTransferLimit, decimal fundToWireTransfer)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
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

            var wireTransferred = new WireTransferred(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today
            };

            var accountBlocked = new AccountBlocked(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                ReasonForAccountBlock = "Daily wire transfer limit breached"
            };

            var wireTransferSecondTime = new WireTransfer()
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today
            };

            var givens = new Event[] { accountCreated, dailyWireTransferLimitApplied, cashDeposited, wireTransferred, accountBlocked };

            await _runner.Run(def => def.Given(givens).When(wireTransferSecondTime).Throws(new OperationCanceledException("Account is blocked")));
        }
    }
}
