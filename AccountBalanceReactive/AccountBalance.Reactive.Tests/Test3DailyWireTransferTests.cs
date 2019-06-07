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
    public class Test3DailyWireTransferTests : IDisposable
    {
        readonly Guid _accountId;
        readonly string _accountHolderName;
        readonly EventStoreScenarioRunner<Account> _runner;

        public Test3DailyWireTransferTests(EventStoreFixture fixture)
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
        [InlineData(1000.00)]
        public async Task Can_set_daily_wire_transfer_limit(decimal dailyWireTransferLimit)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var setDailyWireTransferLimit = new SetDailyWireTransferLimit()
            {
                AccountId = _accountId,
                DailyWireTransferLimit = dailyWireTransferLimit
            };

            var dailyWireTransferLimitApplied = new DailyWireTransferLimitApplied(setDailyWireTransferLimit)
            {
                AccountId = setDailyWireTransferLimit.AccountId,
                DailyWireTransferLimit = setDailyWireTransferLimit.DailyWireTransferLimit
            };

            await _runner.Run(def => def.Given(accountCreated).When(setDailyWireTransferLimit).Then(dailyWireTransferLimitApplied));
        }

        [Theory]
        [InlineData(2000.00, 500.00, 500.00)]
        public async Task Can_consider_fresh_daily_wire_transfer_utilization(decimal availableFund, decimal dailyWireTransferLimit,
            decimal fundToWireTransfer)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var dailyWireTransferLimitApplied = new DailyWireTransferLimitApplied(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                DailyWireTransferLimit = dailyWireTransferLimit
            };

            var cashDeposited = new CashDeposited(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = availableFund
            };

            var wireTransferredYesterday = new WireTransferred(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today.AddDays(-1)
            };

            var wireTransferToday = new WireTransfer()
            {
                AccountId = _accountId,
                Fund = fundToWireTransfer,
                WireTransferDate = DateTime.Today
            };

            var wireTransferredToday = new WireTransferred(wireTransferToday)
            {
                AccountId = wireTransferToday.AccountId,
                Fund = wireTransferToday.Fund,
                WireTransferDate = wireTransferToday.WireTransferDate
            };

            var givens = new Event[] { accountCreated, dailyWireTransferLimitApplied, cashDeposited, wireTransferredYesterday };

            await _runner.Run(def => def.Given(givens).When(wireTransferToday).Then(wireTransferredToday));
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        public async Task Can_throw_exception_on_setting_invalid_daily_wire_transfer_limit(decimal dailyWireTransferLimit)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var setDailyWireTransferLimit = new SetDailyWireTransferLimit()
            {
                AccountId = _accountId,
                DailyWireTransferLimit = dailyWireTransferLimit
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(setDailyWireTransferLimit)
                    .Throws(new ArgumentException("Invalid Daily Wire Transfer Limit")));
        }
    }
}
