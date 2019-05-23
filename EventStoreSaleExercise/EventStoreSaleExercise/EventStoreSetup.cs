namespace EventStoreSaleExercise
{
    using System;
    using EventStore.ClientAPI;
    using EventStore.ClientAPI.SystemData;

    public static class EventStoreSetup
    {
        public static readonly UserCredentials AdminCredentials = new UserCredentials("admin", "changeit");
        public const string StreamName = "Sales";
        public const string SaleAddedEvent = "SaleAdded";
        public static IEventStoreConnection conn;

        public static IEventStoreConnection CreateConnection()
        {
            conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"),
                "InputFromFileConsoleApp");
            conn.ConnectAsync().Wait();
            return conn;
        }
    }
}
