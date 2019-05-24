using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBalanceDomain.Commands;
using AccountBalanceDomain.EventStore;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;

namespace AccountBalanceDomain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EventStoreWrapper eventStoreWrapper = new EventStoreWrapper();
            SendCommand();
        }

        public static void SendCommand()
        {
            // Build command bus
            var commandBus = new Dispatcher(
                name: "Command Bus",
                watchSlowMsg: false,
                slowMsgThreshold: TimeSpan.FromSeconds(100),
                slowCmdThreshold: TimeSpan.FromSeconds(100));

            // Register domain command handlers within the command bus
            new AccountCommandHandler(commandBus, EventStoreWrapper.eventStoreRepository);

            // create a command message
            var cmd = new CreateAccount(
                accountId: Guid.NewGuid(),
                accountHolderName: "Rupesh",
                source: CorrelatedMessage.NewRoot());


            // send command message
            commandBus.Send(cmd);

        }
    }
}
