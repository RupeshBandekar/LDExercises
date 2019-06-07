namespace AccountBalance.Reactive.Tests.Common
{
    using Xunit;

    [CollectionDefinition("AccountBalanceTest")]
    public sealed class AccountBalanceTestCollection : ICollectionFixture<EventStoreFixture>
    {
    }
}
