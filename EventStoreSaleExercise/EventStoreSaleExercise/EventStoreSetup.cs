using System;
using System.Collections.Generic;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace EventStoreSaleExercise
{
    public static class EventStoreSetup
    {
        public static readonly UserCredentials AdminCredentials = new UserCredentials("admin", "changeit");
        public const string StreamName = "Sales";
        public const string SaleAddedEvent = "SaleAdded";
        //public const string InventoryManagerGroup = "InventoryManagerGroup";
        //public const string DirectorGroup = "DirectorGroup";
        public static IEventStoreConnection conn;

        public static IEventStoreConnection CreateConnection()
        {
            conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"),
                "InputFromFileConsoleApp");
            conn.ConnectAsync().Wait();
            return conn;
        }

        public static void CreatePersistentSubscription(IEventStoreConnection conn, string streamName, string group)
        {
            PersistentSubscriptionSettings settings = PersistentSubscriptionSettings.Create()
                .DoNotResolveLinkTos()
                //.StartFromCurrent();
                .StartFromBeginning();

            try
            {
                conn.CreatePersistentSubscriptionAsync(streamName, group, settings, AdminCredentials).Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException.GetType() != typeof(InvalidOperationException)
                    && ex.InnerException?.Message != $"Subscription group {group} on stream {streamName} already exists")
                {
                    throw;
                }
            }
        }
    }
}
