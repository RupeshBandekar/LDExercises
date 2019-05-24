namespace AccountBalanceReactive.Tests
{
    using Xunit;

    [CollectionDefinition("AccountTest")]
    public class AggregateTestCollection : ICollectionFixture<EventStoreFixture>
    {
    }
}
