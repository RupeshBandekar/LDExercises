using System;
using System.Net;
using AccountBalanceDomain.Commands;
using AccountBalanceDomain.EventStore;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using ReactiveDomain.EventStore;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;

namespace AccountBalanceReactive.Tests
{
    public class EventStoreFixture : IDisposable
    {
        private IRepository eventStoreRepository;
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

            var userCredentialsProjectionManager = new UserCredentials(
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

            //projectionManager.EnableAsync("$by_category", userCredentialsProjectionManager).Wait();

            // Define the stream name builder
            IStreamNameBuilder streamNameBuilder = new PrefixedCamelCaseStreamNameBuilder(domainPrefix);

            // Build event store repository
            eventStoreRepository = new StreamStoreRepository(
                streamNameBuilder: streamNameBuilder,
                eventStoreConnection: eventStoreLoader.Connection,
                eventSerializer: new JsonMessageSerializer());

            // Build event store stream listener factory
            Func<string, IListener> getListener = name => new StreamListener(
                listenerName: name,
                eventStoreConnection: eventStoreLoader.Connection,
                streamNameBuilder: streamNameBuilder,
                serializer: new JsonMessageSerializer());
        }

        public IRepository EventStoreRepository => eventStoreRepository;

        public void Dispose()
        {
            
        }
    }
}
