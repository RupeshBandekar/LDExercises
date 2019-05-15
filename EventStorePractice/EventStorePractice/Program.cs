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
            var events = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filePath));
            var eventData = new List<EventData>();

            foreach (var _event in events)
            {
                var id = _event.eventId.ToString();
                var eventType = _event.eventType.ToString();
                eventData.Add(new EventData(Guid.Parse(id), eventType, true,
                    Encoding.UTF8.GetBytes(_event.data.ToString()), null));
            }

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

        static void ReadFirstEvent()
        {
            var conn = CreateConnection();
            var streamName = Globals.streamName;

            var readEvents = conn.ReadStreamEventsForwardAsync(streamName, 0, 10, true).Result;
            foreach (var _event in readEvents.Events)
            {
                Console.WriteLine(Encoding.UTF8.GetString(_event.Event.Data));
            }

            //var readResult = conn.ReadEventAsync(streamName, 0, true).Result;
            //Console.WriteLine(Encoding.UTF8.GetString(readResult.Event.Value.Event.Data));
        }

        static void ConnectToPersistentSubscriptions()
        {
            //var conn = CreateConnection();
            //var streamName = Globals.streamName;
            //var adminCredentials = Globals.AdminCredentials;

            var conn = CreateConnection();
            var adminCredentials = Globals.AdminCredentials;
            var projection = Globals.Projection;
            var streamName = string.Empty;

            foreach (var _file in Directory.GetFiles("../", "shoppingCart-*"))
            {
                streamName = Path.GetFileNameWithoutExtension(_file);
                var eventDataRaw = ProcessEvents(_file);
                var eventData = eventDataRaw.ToArray();
                conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventData).Wait();
            }

            PersistentSubscriptionSettings settings = PersistentSubscriptionSettings.Create()
                .DoNotResolveLinkTos()
                .StartFromCurrent();

            conn.CreatePersistentSubscriptionAsync(streamName, "examplegroup", settings, adminCredentials).Wait();

            conn.ConnectToPersistentSubscription(streamName, "examplegroup", (_, x) =>
            {
                var data = Encoding.ASCII.GetString(x.Event.Data);
                Console.WriteLine("Received: " + x.Event.EventStreamId + ":" + x.Event.EventNumber);
                Console.WriteLine(data);
            }, (sub, reason, ex) => { }, adminCredentials);

            Console.WriteLine("waiting for events. press enter to exit");
            Console.ReadLine();
        }

        static void FirstProjection()
        {
            var conn = CreateConnection();
            var adminCredentials = Globals.AdminCredentials;
            var projection = Globals.Projection;

            foreach (var _file in Directory.GetFiles("../","shoppingCart-*"))
            {
                var streamName = Path.GetFileNameWithoutExtension(_file);
                var eventDataRaw = ProcessEvents(_file);
                var eventData = eventDataRaw.ToArray();
                conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventData).Wait();
            }

            string countItemsProjection = @"
                    fromAll().when({
                    $init: function(){
                        return {
                            count: 0
                        }
                    },
                    ItemAdded: function(s,e){
                        if(e.body.Description.indexOf('Xbox One S') >= 0){
                            s.count += 1;
                        }
                    }
                })";

            projection.CreateContinuousAsync("xbox-one-s-counter", countItemsProjection, adminCredentials).Wait();

            var projectionState = projection.GetStateAsync("xbox-one-s-counter", adminCredentials);
            Console.Write(projectionState.Result);
        }

        static void CreateCatchupSubscription()
        {
            var conn = CreateConnection();
            var adminCredentials = Globals.AdminCredentials;
            var streamName = string.Empty;

            foreach (var _file in Directory.GetFiles("../", "shoppingCart-*"))
            {
                streamName = Path.GetFileNameWithoutExtension(_file);

                var sub = conn.SubscribeToStreamFrom(streamName, null, true,
                    (_, x) =>
                    {
                        string data = Encoding.ASCII.GetString(x.Event.Data);
                        Console.WriteLine("Received: " + x.Event.EventStreamId + ":" + x.Event.EventNumber);
                        Console.WriteLine(data);
                    });
            }

            Console.WriteLine("waiting for events. press enter to exit");
            Console.ReadLine();

        }

        static void WriteData()
        {
            var conn = CreateConnection();
            var adminCredentials = Globals.AdminCredentials;
            var streamName = string.Empty;

            foreach (var _file in Directory.GetFiles("../", "shoppingCart-*"))
            {
                streamName = Path.GetFileNameWithoutExtension(_file);
                var eventDataRaw = ProcessEvents(_file);
                var eventData = eventDataRaw.ToArray();
                conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventData).Wait();
            }
            Console.WriteLine("events written successfully! press enter to exit");
            Console.ReadLine();
        }

        public EventData AddSalesData(int productId, string productName, int quantity, decimal price)
        {
            return new EventData(Guid.NewGuid(), "salesdata", true,
                Encoding.UTF8.GetBytes("{id:" + productId + ",name:" + productName + ",quantity:" + quantity +
                                       ",price:" + price + "}"), null);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("1 - Connect to persistent subscription");
            Console.WriteLine("2 - Writing first event");
            Console.WriteLine("3 - First projection");
            Console.WriteLine("4 - Create Catchup Subscription");
            Console.WriteLine("5 - Write Data");
            Console.WriteLine("Enter your option:");
            string input = Console.ReadLine();
            if (input == "1")
            {
                FirstEvent();
                ReadFirstEvent();
            }
            else if(input == "2")
            {
                ConnectToPersistentSubscriptions();
            }
            else if(input == "3")
            {
                FirstProjection();
            }
            else if(input == "4")
            {
                CreateCatchupSubscription();
            }
            else if(input == "5")
            {
                WriteData();
            }
            
        }

    }
}
