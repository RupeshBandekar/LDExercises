namespace AccountBalance.Reactive.Tests
{
    using System;
    using System.Threading.Tasks;
    using AccountBalance.Reactive.Commands;
    using AccountBalance.Reactive.Events;
    using AccountBalance.Reactive.Tests.Common;
    using AccountBalance.Reactive.Tests.Helper;
    using ReactiveDomain.Messaging;
    using Xunit;
    using Xunit.ScenarioReporting;

    [Collection("AccountBalanceTest")]
    public class Test4DepositChequeTests : IDisposable
    {

        readonly Guid _accountId;
        readonly string _accountHolderName;
        readonly EventStoreScenarioRunner<Account> _runner;

        public Test4DepositChequeTests(EventStoreFixture fixture)
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
        [MemberData(nameof(TestDataGenerator.GetDepositChequeData), MemberType = typeof(TestDataGenerator))]
        public async Task Can_deposit_cheque(decimal depositFund, DateTime depositDate, DateTime clearanceBusinessDay)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var depositCheque = new DepositCheque()
            {
                AccountId = _accountId,
                Fund = depositFund,
                DepositDate = depositDate
            };

            var chequeDeposited = new ChequeDeposited(depositCheque)
            {
                AccountId = depositCheque.AccountId,
                Fund = depositCheque.Fund,
                DepositDate = depositCheque.DepositDate,
                ClearanceBusinessDay = clearanceBusinessDay
            };

            await _runner.Run(def => def.Given(accountCreated).When(depositCheque).Then(chequeDeposited));
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        public async Task Can_throw_exception_on_invalid_amount_cheque_deposit(decimal depositFund)
        {
            var accountCreated = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = _accountHolderName
            };

            var depositCheque = new DepositCheque()
            {
                AccountId = _accountId,
                Fund = depositFund,
                DepositDate = DateTime.Today
            };

            await _runner.Run(
                def => def.Given(accountCreated).When(depositCheque)
                    .Throws(new ArgumentException("Invalid fund value")));
        }

        [Fact]
        public void Can_throw_exception_if_cash_withdrawn_zero_balance_on_cheque_deposit_day()
        {
            decimal fund = 1000.00M;
            DateTime depositDate = DateTime.Today;
        }

        [Fact]
        public void Can_withdraw_available_fund_from_cleared_cheque()
        {
            decimal fund = 1000.00M;
            DateTime depositDate = DateTime.Today.AddDays(-1);
        }

    }
}
