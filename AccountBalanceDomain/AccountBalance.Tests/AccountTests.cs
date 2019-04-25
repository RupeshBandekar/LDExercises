namespace AccountBalanceTests
{
    using System;
    using Xunit;
    using AccountBalanceDomain;

    public class AccountTests
    {
        [Theory]
        [InlineData("Account_Holder1")]
        [Trait("Account_Balance", "1_Account_Creation")]
        public void should_create_account(string accountHolderName)
        {
            var accountId = Guid.NewGuid();
            var account = Account.CreateAccount(accountId, accountHolderName);
            var accountCreatedEvent = account.GetEvents();
            var accountCreatedType = Assert.Single(accountCreatedEvent);
            Assert.IsType<AccountCreated>(accountCreatedType);
            Assert.Equal(accountHolderName, ((AccountCreated) accountCreatedEvent[0]).AccountHolderName);
        }

        [Theory]
        [InlineData("Account_Holder1")]
        [Trait("Account_Balance", "1_Account_Creation")]
        public void should_throw_exception_on_creating_account_with_empty_id(string accountHolderName)
        {
            Assert.Throws<ArgumentException>(() => Account.CreateAccount(Guid.Empty, accountHolderName));
        }

        [Fact]
        [Trait("Account_Balance", "1_Account_Creation")]
        public void should_throw_exception_on_creating_account_with_empty_name()
        {
            Assert.Throws<ArgumentException>(() => Account.CreateAccount(Guid.NewGuid(), string.Empty));
        }

        [Theory]
        [InlineData(1000.00)]
        [Trait("Account_Balance", "2_Set_Overdraft_Limit")]
        public void should_set_overdraft_limit(decimal overdraftLimit)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.SetOverdraftLimit(overdraftLimit);
            var overdraftLimitEvent = account.GetEvents();
            Assert.Equal(2, overdraftLimitEvent.Count);
            Assert.IsType<AccountCreated>(overdraftLimitEvent[0]);
            Assert.IsType<OverdraftLimitApplied>(overdraftLimitEvent[1]);
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        [Trait("Account_Balance", "2_Set_Overdraft_Limit")]
        public void should_throw_exception_on_setting_invalid_overdraft_limit(decimal overdraftLimit)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.SetOverdraftLimit(overdraftLimit));
        }

        [Theory]
        [InlineData(1000.00)]
        [Trait("Account_Balance", "3_Set_Daily_Wire_Transfer_Limit")]
        public void should_set_daily_wire_transfer_limit(decimal dailyWireTransferLimit)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.SetDailyWireTransferLimit(dailyWireTransferLimit);
            var dailyWireTransferLimitEvent = account.GetEvents();
            Assert.Equal(2, dailyWireTransferLimitEvent.Count);
            Assert.IsType<AccountCreated>(dailyWireTransferLimitEvent[0]);
            Assert.IsType<DailyWireTransferLimitApplied>(dailyWireTransferLimitEvent[1]);
            Assert.Equal(dailyWireTransferLimit,
                ((DailyWireTransferLimitApplied) dailyWireTransferLimitEvent[1]).DailyWireTransferLimit);
        }

        [Theory]
        [InlineData(2000.00, 500.00, 500.00)]
        [Trait("Account_Balance", "3_Set_Daily_Wire_Transfer_Limit")]
        public void should_reset_daily_wire_transfer_utilization(decimal fundAvailable, decimal dailyWireTransferLimit,
            decimal fundToWireTransfer)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.SetDailyWireTransferLimit(dailyWireTransferLimit);
            account.DepositCash(fundAvailable);
            account.WireTransfer(fundToWireTransfer, DateTime.Today.AddDays(-1));
            account.WireTransfer(fundToWireTransfer, DateTime.Today);
            var dailyWireTransferLimitEvent = account.GetEvents();
            Assert.Equal(5, dailyWireTransferLimitEvent.Count);
            Assert.IsType<AccountCreated>(dailyWireTransferLimitEvent[0]);
            Assert.IsType<DailyWireTransferLimitApplied>(dailyWireTransferLimitEvent[1]);
            Assert.IsType<CashDeposited>(dailyWireTransferLimitEvent[2]);
            Assert.IsType<WireTransferred>(dailyWireTransferLimitEvent[3]);
            Assert.IsType<WireTransferred>(dailyWireTransferLimitEvent[4]);
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        [Trait("Account_Balance", "3_Set_Daily_Wire_Transfer_Limit")]
        public void should_throw_exception_on_setting_invalid_daily_wire_transfer_limit(decimal dailyWireTransferLimit)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.SetDailyWireTransferLimit(dailyWireTransferLimit));
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.GetDepositChequeData), MemberType = typeof(TestDataGenerator))]
        [Trait("Account_Balance", "4_Deposit_Cheque")]
        public void should_deposit_cheque(decimal depositFund, DateTime depositDate, DateTime clearanceBusinessDay)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.DepositCheque(depositFund, depositDate);
            var chequeDepositedEvent = account.GetEvents();
            Assert.Equal(2, chequeDepositedEvent.Count);
            Assert.IsType<AccountCreated>(chequeDepositedEvent[0]);
            Assert.IsType<ChequeDeposited>(chequeDepositedEvent[1]);
            Assert.Equal(clearanceBusinessDay, ((ChequeDeposited) chequeDepositedEvent[1]).ClearanceBusinessDay);
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        [Trait("Account_Balance", "4_Deposit_Cheque")]
        public void should_throw_exception_on_invalid_amount_cheque_deposit(decimal depositFund)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.DepositCheque(depositFund, DateTime.Today));
        }

        [Fact]
        [Trait("Account_Balance", "4_Deposit_Cheque")]
        public void should_throw_exception_if_cash_withdrawn_zero_balance_on_cheque_deposit_day()
        {
            decimal fund = 1000.00M;
            DateTime depositDate = DateTime.Today;
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.DepositCheque(fund, depositDate);
            var exception = Assert.Throws<OperationCanceledException>(() => account.WithdrawCash(fund));
            var checkDepositedEvent = account.GetEvents();
            Assert.Equal(2, checkDepositedEvent.Count);
            Assert.IsType<AccountCreated>(checkDepositedEvent[0]);
            Assert.IsType<ChequeDeposited>(checkDepositedEvent[1]);
            Assert.Equal("Fund insufficient", exception.Message);
        }

        [Fact]
        [Trait("Account_Balance", "4_Deposit_Cheque")]
        public void should_withdraw_available_fund_from_cleared_cheque()
        {
            decimal fund = 1000.00M;
            DateTime depositDate = DateTime.Today.AddDays(-1);
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.DepositCheque(fund, depositDate);
            account.WithdrawCash(fund);
            var fundAvailableEvent = account.GetEvents();
            Assert.Equal(3, fundAvailableEvent.Count);
            Assert.IsType<AccountCreated>(fundAvailableEvent[0]);
            Assert.IsType<ChequeDeposited>(fundAvailableEvent[1]);
            Assert.IsType<CashWithdrew>(fundAvailableEvent[2]);
            Assert.Equal(fund, ((CashWithdrew) fundAvailableEvent[2]).Fund);
        }

        [Theory]
        [InlineData(1000.00)]
        [Trait("Account_Balance", "5_Deposit_Cash")]
        public void should_deposit_cash(decimal fund)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.DepositCash(fund);
            var cashDepositedEvent = account.GetEvents();
            Assert.Equal(2, cashDepositedEvent.Count);
            Assert.IsType<AccountCreated>(cashDepositedEvent[0]);
            Assert.IsType<CashDeposited>(cashDepositedEvent[1]);
            Assert.Equal(fund, ((CashDeposited) cashDepositedEvent[1]).Fund);
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        [Trait("Account_Balance", "5_Deposit_Cash")]
        public void should_throw_exception_on_depositing_invalid_cash(decimal fund)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.DepositCash(fund));
        }

        [Theory]
        [InlineData(1000.00, 500.00)]
        [Trait("Account_Balance", "6_Withdraw_Cash")]
        public void should_withdraw_cash(decimal availableFund, decimal fundToWithdraw)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.DepositCash(availableFund);
            account.WithdrawCash(fundToWithdraw);
            var cashWithdrewEvent = account.GetEvents();
            Assert.Equal(3, cashWithdrewEvent.Count);
            Assert.IsType<AccountCreated>(cashWithdrewEvent[0]);
            Assert.IsType<CashDeposited>(cashWithdrewEvent[1]);
            Assert.IsType<CashWithdrew>(cashWithdrewEvent[2]);
            Assert.Equal(fundToWithdraw, ((CashWithdrew) cashWithdrewEvent[2]).Fund);
        }

        [Theory]
        [InlineData(1000.00)]
        [Trait("Account_Balance", "6_Withdraw_Cash")]
        public void should_throw_exception_on_withdraw_from_zero_balance(decimal fundToWithdraw)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            var exception = Assert.Throws<OperationCanceledException>(() => account.WithdrawCash(fundToWithdraw));
            var cashWithdrewEvent = account.GetEvents();
            Assert.Single(cashWithdrewEvent);
            Assert.IsType<AccountCreated>(cashWithdrewEvent[0]);
            Assert.Equal("Fund insufficient", exception.Message);
        }

        [Theory]
        [InlineData(500.00, 500.00)]
        [Trait("Account_Balance", "6_Withdraw_Cash")]
        public void should_remove_fund_immediately_on_withdrawing_cash(decimal availableFund, decimal fundToWithdraw)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.DepositCash(availableFund);
            account.WithdrawCash(fundToWithdraw);
            var exception = Assert.Throws<OperationCanceledException>(() => account.WithdrawCash(fundToWithdraw));
            var cashWithdrewEvent = account.GetEvents();
            Assert.Equal(3, cashWithdrewEvent.Count);
            Assert.IsType<AccountCreated>(cashWithdrewEvent[0]);
            Assert.IsType<CashDeposited>(cashWithdrewEvent[1]);
            Assert.IsType<CashWithdrew>(cashWithdrewEvent[2]);
            Assert.Equal(fundToWithdraw, ((CashWithdrew) cashWithdrewEvent[2]).Fund);
            Assert.Equal("Fund insufficient", exception.Message);
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        [Trait("Account_Balance", "6_Withdraw_Cash")]
        public void should_throw_exception_on_withdrawing_invalid_cash(decimal fundToWithdraw)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.WithdrawCash(fundToWithdraw));
        }

        [Theory]
        [InlineData(500.00, 1000.00, 500.00)]
        [Trait("Account_Balance", "7_Wire_Transfer")]
        public void should_wire_transfer(decimal availableFund, decimal wireTransferLimit, decimal fundToWireTransfer)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.SetDailyWireTransferLimit(wireTransferLimit);
            account.DepositCash(availableFund);
            account.WireTransfer(fundToWireTransfer, DateTime.Today);
            var wireTransferredEvent = account.GetEvents();
            Assert.Equal(4, wireTransferredEvent.Count);
            Assert.IsType<AccountCreated>(wireTransferredEvent[0]);
            Assert.IsType<DailyWireTransferLimitApplied>(wireTransferredEvent[1]);
            Assert.IsType<CashDeposited>(wireTransferredEvent[2]);
            Assert.IsType<WireTransferred>(wireTransferredEvent[3]);
            Assert.Equal(fundToWireTransfer, ((WireTransferred) wireTransferredEvent[3]).Fund);
        }

        [Theory]
        [InlineData(1000.00)]
        [Trait("Account_Balance", "7_Wire_Transfer")]
        public void should_throw_exception_on_wire_transfer_from_zero_balance(decimal fundToWireTransfer)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.SetDailyWireTransferLimit(1000.00M);
            var exception =
                Assert.Throws<OperationCanceledException>(
                    () => account.WireTransfer(fundToWireTransfer, DateTime.Today));
            var wireTransferredEvent = account.GetEvents();
            Assert.Equal(2, wireTransferredEvent.Count);
            Assert.IsType<AccountCreated>(wireTransferredEvent[0]);
            Assert.IsType<DailyWireTransferLimitApplied>(wireTransferredEvent[1]);
            Assert.Equal("Fund insufficient", exception.Message);
        }

        [Theory]
        [InlineData(500.00, 1000.00, 500.00)]
        [Trait("Account_Balance", "7_Wire_Transfer")]
        public void should_remove_fund_immediately_on_wire_transferring(decimal availableFund,
            decimal wireTransferLimit, decimal fundToWireTransfer)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.SetDailyWireTransferLimit(wireTransferLimit);
            account.DepositCash(availableFund);
            account.WireTransfer(fundToWireTransfer, DateTime.Today);
            var exception =
                Assert.Throws<OperationCanceledException>(
                    () => account.WireTransfer(fundToWireTransfer, DateTime.Today));
            var wireTransferredEvent = account.GetEvents();
            Assert.Equal(4, wireTransferredEvent.Count);
            Assert.IsType<AccountCreated>(wireTransferredEvent[0]);
            Assert.IsType<DailyWireTransferLimitApplied>(wireTransferredEvent[1]);
            Assert.IsType<CashDeposited>(wireTransferredEvent[2]);
            Assert.IsType<WireTransferred>(wireTransferredEvent[3]);
            Assert.Equal(fundToWireTransfer, ((WireTransferred) wireTransferredEvent[3]).Fund);
            Assert.Equal("Fund insufficient", exception.Message);
        }

        [Theory]
        [InlineData(-1000.00)]
        [InlineData(0)]
        [Trait("Account_Balance", "7_Wire_Transfer")]
        public void should_throw_exception_on_wire_transferring_zero_negative_fund(decimal fundToWithdraw)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            Assert.Throws<ArgumentException>(() => account.WireTransfer(fundToWithdraw, DateTime.Today));
        }

        [Theory]
        [InlineData(1000.00, 500.00, 500.00)]
        [Trait("Account_Balance", "7_Wire_Transfer")]
        public void should_block_account_on_wire_transferring_more_than_wire_transfer_limit(decimal availableFund,
            decimal wireTransferLimit, decimal fundToWireTransfer)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.SetDailyWireTransferLimit(wireTransferLimit);
            account.DepositCash(availableFund);
            account.WireTransfer(fundToWireTransfer, DateTime.Today);
            Assert.Throws<OperationCanceledException>(() => account.WireTransfer(fundToWireTransfer, DateTime.Today));
            var accountBlockedEvent = account.GetEvents();
            Assert.Equal(5, accountBlockedEvent.Count);
            Assert.IsType<AccountCreated>(accountBlockedEvent[0]);
            Assert.IsType<DailyWireTransferLimitApplied>(accountBlockedEvent[1]);
            Assert.IsType<CashDeposited>(accountBlockedEvent[2]);
            Assert.IsType<WireTransferred>(accountBlockedEvent[3]);
            Assert.IsType<AccountBlockedDailyWireTransferLimitBreach>(accountBlockedEvent[4]);
        }

        [Theory]
        [InlineData(100.00, 500.00, 700.00)]
        [Trait("Account_Balance", "8_Block_Account_On_Overdraft_Limit_Breach")]
        public void should_block_account_on_cash_withdrawing_more_than_overdraft_limit(decimal availableFund,
            decimal overdraftLimit, decimal fundToWithdraw)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.SetOverdraftLimit(overdraftLimit);
            account.DepositCash(availableFund);
            Assert.Throws<OperationCanceledException>(() => account.WithdrawCash(fundToWithdraw));
            var accountBlockedEvent = account.GetEvents();
            Assert.Equal(4, accountBlockedEvent.Count);
            Assert.IsType<AccountCreated>(accountBlockedEvent[0]);
            Assert.IsType<OverdraftLimitApplied>(accountBlockedEvent[1]);
            Assert.IsType<CashDeposited>(accountBlockedEvent[2]);
            Assert.IsType<AccountBlockedOverdraftLimitBreach>(accountBlockedEvent[3]);
        }

        [Theory]
        [InlineData(100.00, 1000.00, 500.00, 700.00)]
        [Trait("Account_Balance", "8_Block_Account_On_Overdraft_Limit_Breach")]
        public void should_block_account_on_wire_transferring_more_than_overdraft_limit(decimal availableFund,
            decimal wireTransferLimit, decimal overdraftLimit, decimal fundToWireTransfer)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.SetOverdraftLimit(overdraftLimit);
            account.SetDailyWireTransferLimit(wireTransferLimit);
            account.DepositCash(availableFund);
            Assert.Throws<OperationCanceledException>(() => account.WireTransfer(fundToWireTransfer, DateTime.Today));
            var accountBlockedEvent = account.GetEvents();
            Assert.Equal(5, accountBlockedEvent.Count);
            Assert.IsType<AccountCreated>(accountBlockedEvent[0]);
            Assert.IsType<OverdraftLimitApplied>(accountBlockedEvent[1]);
            Assert.IsType<DailyWireTransferLimitApplied>(accountBlockedEvent[2]);
            Assert.IsType<CashDeposited>(accountBlockedEvent[3]);
            Assert.IsType<AccountBlockedOverdraftLimitBreach>(accountBlockedEvent[4]);
        }

        [Theory]
        [InlineData(100.00, 500.00, 700.00, 50.00)]
        [Trait("Account_Balance", "9_Unblock_Account_On_Available_Fund")]
        public void should_unblock_account_on_cash_deposit(decimal availableFund, decimal overdraftLimit,
            decimal fundToWithdraw, decimal depositFund)
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
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
        }

        [Fact]
        [Trait("Account_Balance", "9_Unblock_Account_On_Available_Fund")]
        public void should_unblock_account_on_cheque_fund_made_available()
        {
            var account = Account.CreateAccount(Guid.NewGuid(), "Account_Holder1");
            account.SetOverdraftLimit(500.00M);
            account.DepositCash(1000.00M);
            Assert.Throws<OperationCanceledException>(() => account.WithdrawCash(2000.00M));
            account.DepositCheque(1000.00M, DateTime.Now.AddDays(-1));
            account.WithdrawCash(500M);
            var accountUnblockedEvent = account.GetEvents();
            Assert.Equal(7, accountUnblockedEvent.Count);
            Assert.IsType<AccountCreated>(accountUnblockedEvent[0]);
            Assert.IsType<OverdraftLimitApplied>(accountUnblockedEvent[1]);
            Assert.IsType<CashDeposited>(accountUnblockedEvent[2]);
            Assert.IsType<AccountBlockedOverdraftLimitBreach>(accountUnblockedEvent[3]);
            Assert.IsType<ChequeDeposited>(accountUnblockedEvent[4]);
            Assert.IsType<AccountUnblocked>(accountUnblockedEvent[5]);
            Assert.IsType<CashWithdrew>(accountUnblockedEvent[6]);
        }
    }
}
