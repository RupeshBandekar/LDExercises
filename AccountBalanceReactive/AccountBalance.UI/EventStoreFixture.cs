namespace AccountBalance.UI
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using EventStore.ClientAPI.Projections;
    using ReactiveDomain;
    using ReactiveDomain.EventStore;
    using ReactiveDomain.Foundation;
    using ReactiveDomain.Foundation.EventStore;

    public class EventStoreFixture : IDisposable
    {
        readonly StreamStoreRepository _repo;

        //readonly ClusterVNode _node;

        public EventStoreFixture()
        {
            var domainPrefix = "AccountBalance";
            var eventStoreUser = "admin";
            var eventStorePwd = "changeit";
            var eventStoreServer = "127.0.0.1";
            var eventStoreTcpPort = 1113;
            var eventStoreHttpPort = 2113;

            // Build event store connection
            var userCredentials = new ReactiveDomain.UserCredentials(
                username: eventStoreUser,
                password: eventStorePwd);

            var userCredentialsProjectionManager = new EventStore.ClientAPI.SystemData.UserCredentials(
                username: eventStoreUser,
                password: eventStorePwd);

            var eventStoreLoader = new EventStoreLoader();

            // connect to the event store
            eventStoreLoader.Connect(
                credentials: userCredentials,
                server: IPAddress.Parse(eventStoreServer),
                tcpPort: eventStoreTcpPort);

            // Start needed EventStore Projections
            var projectionManager = new ProjectionsManager(
                new ReactiveDomain.EventStore.NullLogger(),
                new IPEndPoint(IPAddress.Parse(eventStoreServer), eventStoreHttpPort),
                TimeSpan.FromSeconds(5));

            projectionManager.EnableAsync("$by_category", userCredentialsProjectionManager).Wait();

            // Define the stream name builder
            StreamNameBuilder = new PrefixedCamelCaseStreamNameBuilder(domainPrefix);

            EventSerializer = new JsonMessageSerializer();

            StreamStoreConnection = eventStoreLoader.Connection;

            // Build event store repository
            _repo = new StreamStoreRepository(
                streamNameBuilder: StreamNameBuilder,
                eventStoreConnection: StreamStoreConnection,
                eventSerializer: EventSerializer);

            // Build event store stream listener factory
            Func<string, IListener> getListener = name => new StreamListener(
                listenerName: name,
                eventStoreConnection: StreamStoreConnection,
                streamNameBuilder: StreamNameBuilder,
                serializer: EventSerializer);

            accountRM = new AccountRM(() => getListener(nameof(AccountRM)), StreamNameBuilder);
        }

        public void Dispose()
        {
            StreamStoreConnection.Close();
            StreamStoreConnection.Dispose();
        }

        public IRepository Repository => _repo;

        public IStreamStoreConnection StreamStoreConnection { get; }

        public IStreamNameBuilder StreamNameBuilder { get; }

        public IEventSerializer EventSerializer { get; }

        public AccountRM accountRM { get; }
    }
}
