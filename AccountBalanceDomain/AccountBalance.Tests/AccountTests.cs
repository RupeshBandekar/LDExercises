using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AccountBalanceDomain;

namespace AccountBalanceTests
{
    public class AccountTests
    {
        [Theory]
        [InlineData("Account_Holder1")]
        [Trait("Collection","1_Account_Creation")]
        public void should_create_account(string accountHolderName)
        {
            var accountId = Guid.NewGuid();
            var account = Account.Create(accountId, accountHolderName);
            var accountCreatedEvent = account.GetEvents();
            var accountCreatedType = Assert.Single(accountCreatedEvent);
            Assert.IsType<AccountCreated>(accountCreatedType);
            Assert.Equal(accountHolderName, ((AccountCreated)accountCreatedEvent[0]).AccountHolderName);
        }

        [Theory]
        [InlineData("Account_Holder1")]
        [Trait("Collection", "1_Account_Creation")]
        public void should_throw_exception_when_create_account_with_empty_id(string accountHolderName)
        {
            Assert.Throws<ArgumentException>(() => Account.Create(Guid.Empty, accountHolderName));
        }

        [Fact]
        [Trait("Collection", "1_Account_Creation")]
        public void should_throw_exception_when_create_account_with_empty_name()
        {
            Assert.Throws<ArgumentException>(() => Account.Create(Guid.NewGuid(), string.Empty));
        }
        [Theory]
        [InlineData(1000.00)]
        [Trait("Collection", "2_Set_Overdraft_Limit")]
        public void should_set_overdraft_limit(decimal overdraftLimit)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.SetOverdraftLimit(overdraftLimit);
            var overdraftLimitEvent = account.GetEvents();
            Assert.Equal(2, overdraftLimitEvent.Count);
            Assert.IsType<AccountCreated>(overdraftLimitEvent[0]);
            Assert.IsType<OverdraftLimitApplied>(overdraftLimitEvent[1]);
        }

        [Theory]
        [InlineData(-1000.00)]
        [Trait("Collection", "2_Set_Overdraft_Limit")]
        public void should_throw_exception_when_set_negative_overdraft_limit(decimal overdraftLimit)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.SetOverdraftLimit(overdraftLimit));
        }

        [Theory]
        [InlineData(1000.00)]
        [Trait("Collection", "3_Set_Daily_Wire_Transfer_Limit")]
        public void should_set_daily_wire_transfer_limit(decimal dailyWireTransferLimit)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.SetDailyWireTransferLimit(dailyWireTransferLimit);
            var dailyWireTransferLimitEvent = account.GetEvents();
            Assert.Equal(2, dailyWireTransferLimitEvent.Count);
            Assert.IsType<AccountCreated>(dailyWireTransferLimitEvent[0]);
            Assert.IsType<DailyWireTransferLimitApplied>(dailyWireTransferLimitEvent[1]);
            Assert.Equal(dailyWireTransferLimit, ((DailyWireTransferLimitApplied)dailyWireTransferLimitEvent[1]).DailyWireTransferLimit);
        }

        [Theory]
        [InlineData(-1000.00)]
        [Trait("Collection", "3_Set_Daily_Wire_Transfer_Limit")]
        public void should_throw_exception_when_set_negative_daily_wire_transfer_limit(decimal dailyWireTransferLimit)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.SetDailyWireTransferLimit(dailyWireTransferLimit));
        }

        
        [Theory]
        [MemberData(nameof(TestDataGenerator.GetDepositChequeData), MemberType = typeof(TestDataGenerator))]
        [Trait("Collection", "4_Deposit_Cheque")]
        public void should_deposit_cheque(decimal depositFund, DateTime depositDate, int chequeNumber, DateTime clearanceBusinessDay)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.DepositCheque(depositFund, depositDate, chequeNumber);
            var chequeDepositedEvent = account.GetEvents();
            Assert.Equal(2, chequeDepositedEvent.Count);
            Assert.IsType<AccountCreated>(chequeDepositedEvent[0]);
            Assert.IsType<ChequeDeposited>(chequeDepositedEvent[1]);
            Assert.Equal(clearanceBusinessDay, ((ChequeDeposited)chequeDepositedEvent[1]).ClearanceBussinessDay);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.GetDepositChequeData), MemberType = typeof(TestDataGenerator))]
        [Trait("Collection", "4_Deposit_Cheque")]
        public void should_make_fund_available_of_deposited_cheque_on_cash_withdrawal(decimal depositFund, DateTime depositDate, int chequeNumber, DateTime clearanceBusinessDay)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.DepositCheque(depositFund, depositDate, chequeNumber);
            account.MakeFundAvailableForDepositedCheque(clearanceBusinessDay);
            var makeFundAvailableEvent = account.GetEvents();
            Assert.Equal(3, makeFundAvailableEvent.Count);
            Assert.IsType<AccountCreated>(makeFundAvailableEvent[0]);
            Assert.IsType<ChequeDeposited>(makeFundAvailableEvent[1]);
            Assert.IsType<ChequeFundMadeAvailable>(makeFundAvailableEvent[2]);
            Assert.Equal(depositFund, ((ChequeFundMadeAvailable)makeFundAvailableEvent[2]).Fund);
        }

        [Theory]
        [InlineData(1000.00)]
        [Trait("Collection", "5_Deposit_Cash")]
        public void should_deposit_cash(decimal fund)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.DepositCash(fund);
            var cashDepositedEvent = account.GetEvents();
            Assert.Equal(2, cashDepositedEvent.Count);
            Assert.IsType<AccountCreated>(cashDepositedEvent[0]);
            Assert.IsType<CashDeposited>(cashDepositedEvent[1]);
            Assert.Equal(fund, ((CashDeposited)cashDepositedEvent[1]).Fund);
        }

        [Theory]
        [InlineData(-1000.00)]
        [Trait("Collection", "5_Deposit_Cash")]
        public void should_throw_exception_when_deposit_negative_cash(decimal fund)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.DepositCash(fund));
        }

        [Theory]
        [InlineData(1000.00, 500.00)]
        [Trait("Collection", "6_Withdraw_Cash")]
        public void should_withdraw_cash(decimal availableFund, decimal fundToWithdraw)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.DepositCash(availableFund);
            account.WithdrawCash(fundToWithdraw);
            account.WithdrawCash(fundToWithdraw);
            var cashWithdrewEvent = account.GetEvents();
            Assert.Equal(4, cashWithdrewEvent.Count);
            Assert.IsType<AccountCreated>(cashWithdrewEvent[0]);
            Assert.IsType<CashDeposited>(cashWithdrewEvent[1]);
            Assert.IsType<CashWithdrew>(cashWithdrewEvent[2]);
            Assert.IsType<CashWithdrew>(cashWithdrewEvent[3]);
            Assert.Equal(fundToWithdraw, ((CashWithdrew)cashWithdrewEvent[2]).Fund);
            Assert.Equal(fundToWithdraw, ((CashWithdrew)cashWithdrewEvent[3]).Fund);
        }

        [Theory]
        [InlineData(-1000.00)]
        [Trait("Collection", "6_Withdraw_Cash")]
        public void should_throw_exception_when_withdraw_negative_cash(decimal fundToWithdraw)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.WithdrawCash(fundToWithdraw));
        }

        [Theory]
        [InlineData(1000.00, 500.00, 250.00)]
        [Trait("Collection", "7_Wire_Transfer")]
        public void should_wire_transfer_fund(decimal availableFund, decimal wireTransferLimit, decimal fundToWithdraw)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.SetDailyWireTransferLimit(wireTransferLimit);
            account.DepositCash(availableFund);
            account.WireTransfer(fundToWithdraw);
            account.WireTransfer(fundToWithdraw);
            var wireTransferredEvent = account.GetEvents();
            Assert.Equal(5, wireTransferredEvent.Count);
            Assert.IsType<AccountCreated>(wireTransferredEvent[0]);
            Assert.IsType<DailyWireTransferLimitApplied>(wireTransferredEvent[1]);
            Assert.IsType<CashDeposited>(wireTransferredEvent[2]);
            Assert.IsType<WireTransferred>(wireTransferredEvent[3]);
            Assert.IsType<WireTransferred>(wireTransferredEvent[4]);
            Assert.Equal(fundToWithdraw, ((WireTransferred)wireTransferredEvent[4]).Fund);
        }
        [Theory]
        [InlineData(-1000.00)]
        [Trait("Collection", "7_Wire_Transfer")]
        public void should_throw_exception_when_wire_transfer_negative_fund(decimal fundToWithdraw)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.WireTransfer(fundToWithdraw));
        }
        [Fact]
        [Trait("Collection", "7_Wire_Transfer")]
        public void should_block_account_when_wire_transfer_more_than_WTlimit()
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.SetDailyWireTransferLimit(500.00M);
            account.DepositCash(1000.00M);
            account.WireTransfer(250.00M);
            account.WireTransfer(250.00M);
            Assert.Throws<OperationCanceledException>(() => account.WireTransfer(250.00M));
            var accountBlockedEvent = account.GetEvents();
            Assert.Equal(6, accountBlockedEvent.Count);
            Assert.IsType<AccountCreated>(accountBlockedEvent[0]);
            Assert.IsType<DailyWireTransferLimitApplied>(accountBlockedEvent[1]);
            Assert.IsType<CashDeposited>(accountBlockedEvent[2]);
            Assert.IsType<WireTransferred>(accountBlockedEvent[3]);
            Assert.IsType<WireTransferred>(accountBlockedEvent[4]);
            Assert.IsType<AccountBlockedDailyWireTransferLimitBreach>(accountBlockedEvent[5]);
            Assert.Equal(AccountState.Blocked, ((AccountBlocked)accountBlockedEvent[5]).AccountState);
        }

        [Theory]
        [InlineData(100.00, 500.00, 250.00)]
        [Trait("Collection", "8_Block_Account_On_Overdraft_Limit_Breach")]
        public void should_block_account_when_withdraw_more_than_overdraft_limit(decimal availableFund,
            decimal overdraftLimit, decimal fundToWithdraw)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.SetOverdraftLimit(overdraftLimit);
            account.DepositCash(availableFund);
            account.WithdrawCash(fundToWithdraw);
            account.WithdrawCash(fundToWithdraw);
            Assert.Throws<OperationCanceledException>(() => account.WithdrawCash(fundToWithdraw));
            var accountBlockedEvent = account.GetEvents();
            Assert.Equal(6, accountBlockedEvent.Count);
            Assert.IsType<AccountCreated>(accountBlockedEvent[0]);
            Assert.IsType<OverdraftLimitApplied>(accountBlockedEvent[1]);
            Assert.IsType<CashDeposited>(accountBlockedEvent[2]);
            Assert.IsType<CashWithdrew>(accountBlockedEvent[3]);
            Assert.IsType<CashWithdrew>(accountBlockedEvent[4]);
            Assert.IsType<AccountBlockedOverdraftLimitBreach>(accountBlockedEvent[5]);
            Assert.Equal(AccountState.Blocked, ((AccountBlocked) accountBlockedEvent[5]).AccountState);
        }

        [Theory]
        [InlineData(100.00, 1000.00, 500.00, 250.00)]
        [Trait("Collection", "8_Block_Account_On_Overdraft_Limit_Breach")]
        public void should_block_account_when_wire_transfer_more_than_overdraft_limit(decimal availableFund,
            decimal wireTransferLimit, decimal overdraftLimit, decimal fundToWithdraw)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.SetOverdraftLimit(overdraftLimit);
            account.SetDailyWireTransferLimit(wireTransferLimit);
            account.DepositCash(availableFund);
            account.WithdrawCash(fundToWithdraw);
            account.WithdrawCash(fundToWithdraw);
            Assert.Throws<OperationCanceledException>(() => account.WireTransfer(fundToWithdraw));
            var accountBlockedEvent = account.GetEvents();
            Assert.Equal(7, accountBlockedEvent.Count);
            Assert.IsType<AccountCreated>(accountBlockedEvent[0]);
            Assert.IsType<OverdraftLimitApplied>(accountBlockedEvent[1]);
            Assert.IsType<DailyWireTransferLimitApplied>(accountBlockedEvent[2]);
            Assert.IsType<CashDeposited>(accountBlockedEvent[3]);
            Assert.IsType<CashWithdrew>(accountBlockedEvent[4]);
            Assert.IsType<CashWithdrew>(accountBlockedEvent[5]);
            Assert.IsType<AccountBlockedOverdraftLimitBreach>(accountBlockedEvent[6]);
            Assert.Equal(AccountState.Blocked, ((AccountBlocked) accountBlockedEvent[6]).AccountState);
        }

        [Theory]
        [InlineData(100.00, 500.00, 700.00, 50.00)]
        [Trait("Collection", "9_Unblock_Account_On_Available_Fund")]
        public void should_unblock_account_on_cash_deposit(decimal availableFund, decimal overdraftLimit,
            decimal fundToWithdraw, decimal depositFund)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.SetOverdraftLimit(overdraftLimit);
            account.DepositCash(availableFund);
            Assert.Throws<OperationCanceledException>(() => account.WithdrawCash(fundToWithdraw));
            account.DepositCash(depositFund);
            var accountUnblockedEvent = account.GetEvents();
            Assert.Equal(6, accountUnblockedEvent.Count);
            Assert.IsType<AccountCreated>(accountUnblockedEvent[0]);
            Assert.IsType<OverdraftLimitApplied>(accountUnblockedEvent[1]);
            Assert.IsType<CashDeposited>(accountUnblockedEvent[2]);
            Assert.IsType<AccountBlockedOverdraftLimitBreach>(accountUnblockedEvent[3]);
            Assert.IsType<CashDeposited>(accountUnblockedEvent[4]);
            Assert.IsType<AccountUnblocked>(accountUnblockedEvent[5]);
            Assert.Equal(AccountState.Unblocked, ((AccountUnblocked) accountUnblockedEvent[5]).AccountState);
        }

        [Fact]
        //[MemberData(nameof(TestDataGenerator.GetDepositChequeUnblockData), MemberType = typeof(TestDataGenerator))]
        [Trait("Collection", "9_Unblock_Account_On_Available_Fund")]
        public void should_unblock_account_on_cheque_fund_made_available()
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            account.SetOverdraftLimit(500.00M);
            account.DepositCash(1000.00M);
            Assert.Throws<OperationCanceledException>(() => account.WithdrawCash(2000.00M));
            account.DepositCheque(1000.00M, DateTime.Now.AddDays(-1), 12345);
            account.DepositCheque(1000.00M, DateTime.Now.AddDays(-1), 67890);
            account.WithdrawCash(500M);
            account.WithdrawCash(500M);
            var accountUnblockedEvent = account.GetEvents();
            Assert.Equal(11, accountUnblockedEvent.Count);
            Assert.IsType<AccountCreated>(accountUnblockedEvent[0]);
            Assert.IsType<OverdraftLimitApplied>(accountUnblockedEvent[1]);
            Assert.IsType<CashDeposited>(accountUnblockedEvent[2]);
            Assert.IsType<AccountBlockedOverdraftLimitBreach>(accountUnblockedEvent[3]);
            Assert.IsType<ChequeDeposited>(accountUnblockedEvent[4]);
            Assert.IsType<ChequeDeposited>(accountUnblockedEvent[5]);
            Assert.IsType<ChequeFundMadeAvailable>(accountUnblockedEvent[6]);
            Assert.IsType<AccountUnblocked>(accountUnblockedEvent[7]);
            Assert.IsType<ChequeFundMadeAvailable>(accountUnblockedEvent[8]);
            Assert.IsType<CashWithdrew>(accountUnblockedEvent[9]);
            Assert.IsType<CashWithdrew>(accountUnblockedEvent[10]);
            Assert.Equal(AccountState.Unblocked, ((AccountUnblocked)accountUnblockedEvent[7]).AccountState);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.GetDateTime), MemberType = typeof(TestDataGenerator))]
        [Trait("Collection", "99_Get_Next_Business_Day")]
        public void should_return_next_business_day(DateTime depositDay, DateTime expectedNextBusinessDay)
        {
            var account = Account.Create(Guid.NewGuid(), "Account_Holder1");
            Assert.Equal(expectedNextBusinessDay, account.GetChequeClearanceDay(depositDay));
        }
    }
}
