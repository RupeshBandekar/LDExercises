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
    public class DepositCashTests : IDisposable
    {
        readonly Guid _accountId;
        readonly string _accountHolderName;
        readonly EventStoreScenarioRunner<Account> _runner;

        public DepositCashTests(EventStoreFixture fixture)
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
        public async Task Can_deposit_cash(decimal depositFund)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var depositCash = new DepositCash()
            {
                AccountId = _accountId,
                Fund = depositFund
            };

            var cashDeposited = new CashDeposited(depositCash)
            {
                AccountId = depositCash.AccountId,
                Fund = depositCash.Fund
            };

            await _runner.Run(def => def.Given(accountCreated).When(depositCash).Then(cashDeposited));
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        public async Task Can_throw_exception_on_depositing_invalid_cash(decimal depositFund)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var depositCash = new DepositCash()
            {
                AccountId = _accountId,
                Fund = depositFund
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(depositCash).Throws(new ArgumentException("Invalid fund value")));
        }
    }
}
