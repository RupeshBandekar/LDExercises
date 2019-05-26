namespace AccountBalance.Reactive.Tests.Common
{
    using Xunit;

    [CollectionDefinition("AggregateTest")]
    public sealed class AggregateTestCollection : ICollectionFixture<EventStoreFixture>
    {
    }
}
