using AccountBalanceDomain.Commands;
using ReactiveDomain.Messaging.Bus;

namespace AccountBalanceReactive.Tests
{
    using ReactiveDomain.Messaging;
    using System;
    using Xunit;
    [Collection("AccountTest")]
    public class AccountAggregateTests : IDisposable
    {
        private Dispatcher commandBus;
        private Guid _accountId;
        private readonly EventStoreFixture _fixture;

        public AccountAggregateTests(EventStoreFixture fixture)
        {
            _accountId = Guid.NewGuid();
            _fixture = fixture;
            // Build command bus
            commandBus = new Dispatcher(
                name: "Command Bus",
                watchSlowMsg: false,
                slowMsgThreshold: TimeSpan.FromSeconds(100),
                slowCmdThreshold: TimeSpan.FromSeconds(100));

            // Register domain command handlers within the command bus
            new AccountCommandHandler(commandBus, _fixture.EventStoreRepository);
        }

        public void Dispose()
        {}

        [Theory]
        [InlineData("Account_Holder1")]
        [Trait("Account_Balance", "1_Account_Creation")]
        public void Can_create_account(string accountHolderName)
        {
            var cmd = new CreateAccount(
                accountId: _accountId,
                accountHolderName: accountHolderName,
                source: CorrelatedMessage.NewRoot());

            commandBus.Send(cmd);
        }

        [Theory]
        [InlineData("Account_Holder1")]
        [Trait("Account_Balance", "1_Account_Creation")]
        public void Can_throw_exception_on_creating_account_with_empty_id(string accountHolderName)
        {
            var cmd = new CreateAccount(
                accountId: Guid.Empty, 
                accountHolderName: accountHolderName,
                source: CorrelatedMessage.NewRoot());

            var exception = Assert.Throws<ReactiveDomain.Messaging.Bus.CommandException>(() => commandBus.Send(cmd));
            Assert.Equal("CreateAccount: Invalid Account ID", exception.Message);
        }

        [Fact]
        [Trait("Account_Balance", "1_Account_Creation")]
        public void Can_throw_exception_on_creating_account_with_empty_name()
        {
            var cmd = new CreateAccount(
                accountId: Guid.NewGuid(), 
                accountHolderName: string.Empty,
                source: CorrelatedMessage.NewRoot());

            var exception = Assert.Throws<ReactiveDomain.Messaging.Bus.CommandException>(() => commandBus.Send(cmd));
            Assert.Equal("CreateAccount: Invalid Account Holder Name", exception.Message);
        }

        [Theory]
        [InlineData("Account_Holder1", 1000.00)]
        [Trait("Account_Balance", "2_Set_Overdraft_Limit")]
        public void should_set_overdraft_limit(string accountHolderName, decimal overdraftLimit)
        {
            var cmd = new CreateAccount(
                accountId: _accountId,
                accountHolderName: accountHolderName,
                source: CorrelatedMessage.NewRoot());

            commandBus.Send(cmd);

            var cmdOverdraftLimit = new SetOverdraftLimit(
                accountId: _accountId, 
                overdraftLimit: overdraftLimit,
                source: CorrelatedMessage.NewRoot());

            commandBus.Send(cmdOverdraftLimit);
        }
    }
}
