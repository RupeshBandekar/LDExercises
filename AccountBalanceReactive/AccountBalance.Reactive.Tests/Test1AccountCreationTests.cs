namespace AccountBalance.Reactive.Tests
{
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using AccountBalance.Reactive.Tests.Common;
    using Xunit.ScenarioReporting;

    [Collection("AccountBalanceTest")]
    public class Test1AccountCreationTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public Test1AccountCreationTests(EventStoreFixture fixture)
        {
            _accountId = Guid.NewGuid();
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
        [InlineData("Account_Holder1")]
        public async Task Can_create_account(string accountHolderName)
        {
            var createAccount = new CreateAccount
            {
                AccountId = _accountId,
                AccountHolderName = accountHolderName
            };

            var accountCreated = new AccountCreated(createAccount)
            {
                AccountId = createAccount.AccountId,
                AccountHolderName = createAccount.AccountHolderName
            };

            await _runner.Run(
                def => def.Given().When(createAccount).Then(accountCreated)
            );
        }

        [Theory]
        [InlineData("Account_Holder1")]
        public async Task Can_throw_exception_on_creating_account_with_empty_id(string accountHolderName)
        {
            var createAccount = new CreateAccount
            {
                AccountId = Guid.Empty,
                AccountHolderName = accountHolderName
            };

            await _runner.Run(def => def.Given().When(createAccount).Throws(new ArgumentException("Invalid Account ID")));
        }

        [Fact]
        public async Task Can_throw_exception_on_creating_account_with_empty_name()
        {
            var createAccount = new CreateAccount
            {
                AccountId = Guid.NewGuid(),
                AccountHolderName = string.Empty
            };

            await _runner.Run(def => def.Given().When(createAccount).Throws(new ArgumentException("Invalid Account Holder Name")));
        }
    }
}
