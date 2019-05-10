using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace EventStoreSaleExercise
{
    public static class Globals
    {
        public static readonly UserCredentials AdminCredentials = new UserCredentials("admin", "changeit");
        public const string SaleAddedEvent = "SaleAdded";

        public static IEventStoreConnection CreateConnection()
        {
            var conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"),
                "InputFromFileConsoleApp");
            conn.ConnectAsync().Wait();
            return conn;
        }
    }
}
