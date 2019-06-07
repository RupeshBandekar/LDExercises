namespace AccountBalance.Reactive.Tests
{
    using System;
    using System.Threading.Tasks;
    using Xunit;
    using AccountBalance.Reactive.Commands;
    using AccountBalance.Reactive.Events;
    using AccountBalance.Reactive.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit.ScenarioReporting;

    [Collection("AccountBalanceTest")]
    public class Test2OverdraftLimitTests : IDisposable
    {
        readonly Guid _accountId;
        readonly string _accountHolderName;
        readonly EventStoreScenarioRunner<Account> _runner;

        public Test2OverdraftLimitTests(EventStoreFixture fixture)
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
        public async Task Can_set_overdraft_limit(decimal overdraftLimit)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var setOverdraftLimit = new SetOverdraftLimit()
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            var overdraftLimitApplied = new OverdraftLimitApplied(setOverdraftLimit)
            {
                AccountId = setOverdraftLimit.AccountId,
                OverdraftLimit = setOverdraftLimit.OverdraftLimit
            };

            await _runner.Run(def => def.Given(accountCreated).When(setOverdraftLimit).Then(overdraftLimitApplied));
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        public async Task Can_throw_exception_on_setting_invalid_overdraft_limit(decimal overdraftLimit)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var setOverdraftLimit = new SetOverdraftLimit()
            {
                AccountId = _accountId,
                OverdraftLimit = overdraftLimit
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(setOverdraftLimit)
                    .Throws(new ArgumentException("Invalid Overdraft Limit")));
        }
    }
}
