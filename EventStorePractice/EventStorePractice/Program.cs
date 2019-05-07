using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventStorePractice
{
    class Program
    {

        public static class Globals
        {
            public const string filePath = "../event.json";
            public const string streamName = "newstream";

            public static readonly UserCredentials AdminCredentials = new UserCredentials("admin", "changeit");

            public static readonly ProjectionsManager Projection = new ProjectionsManager(new ConsoleLogger(),
                new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2113), TimeSpan.FromMilliseconds(5000));
        }

        private static IEventStoreConnection CreateConnection()
        {
            var conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"),
                "InputFromFileConsoleApp");
            conn.ConnectAsync().Wait();
            return conn;
        }

        private static List<EventData> ProcessEvents(string filePath)
        {
            var events = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath));
            var eventData = new List<EventData>();
            
            var id = events.SelectToken("eventId").Value<string>();
            var eventType = events.SelectToken("eventType").Value<string>();
            eventData.Add(new EventData(Guid.Parse(id), eventType, true,
                Encoding.UTF8.GetBytes(events.SelectTokens("data").Value<JToken>().ToString()), null));

            return eventData;
        }

        static void FirstEvent()
        {
            var conn = CreateConnection();
            var streamName = Globals.streamName;
            var firstEventData = ProcessEvents(Globals.filePath);
            var eventData = firstEventData.ToArray();

            conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventData).Wait();
            Console.WriteLine($"Published {firstEventData.Count} events to '{Globals.streamName}'");
        }

        static void Main(string[] args)
        {
            FirstEvent();
        }

    }
}
