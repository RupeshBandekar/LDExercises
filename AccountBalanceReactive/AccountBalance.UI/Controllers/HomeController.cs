using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AccountBalance.UI.Controllers
{
    using System.IO;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using AccountBalance.Reactive;
    using AccountBalance.Reactive.Commands;
    using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
    using Newtonsoft.Json;
    using ReactiveDomain;
    using ReactiveDomain.Messaging.Bus;
    using Account = AccountBalance.UI.Account;

    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        EventStoreFixture _fixture;

        public HomeController(EventStoreFixture fixture)
        {
            _fixture = fixture;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[action]")]
        public IEnumerable<Account> GetAccounts()
        {
            return _fixture.accountRM.lstAccount.AsEnumerable();
        }

        [HttpPost("[action]")]
        public async Task<string> CreateAccount()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                var reqData = await reader.ReadToEndAsync();
                var account = new List<Account> {JsonConvert.DeserializeObject<Account>(reqData)};

                var commandBus = new Dispatcher(
                    name: "Command Bus",
                    watchSlowMsg: false,
                    slowMsgThreshold: TimeSpan.FromSeconds(100),
                    slowCmdThreshold:TimeSpan.FromSeconds(100));

                new AccountCommandHandler(_fixture.Repository, commandBus);

                var accountId = Guid.NewGuid();
                var cmd = new CreateAccount()
                {
                    AccountId = accountId,
                    AccountHolderName = account[0].AccountHolderName
                };

                commandBus.Fire(cmd);

                return JsonConvert.SerializeObject($"Account id '{accountId}' created successfully");
            }
        }

        [HttpPost("[action]")]
        public async Task<string> SetOverdraftLimit()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                var reqData = await reader.ReadToEndAsync();
                var account = new List<Account> { JsonConvert.DeserializeObject<Account>(reqData) };

                var commandBus = new Dispatcher(
                    name: "Command Bus",
                    watchSlowMsg: false,
                    slowMsgThreshold: TimeSpan.FromSeconds(100),
                    slowCmdThreshold: TimeSpan.FromSeconds(100));

                new AccountCommandHandler(_fixture.Repository, commandBus);

                var cmd = new SetOverdraftLimit()
                {
                    AccountId = account[0].AccountId,
                    OverdraftLimit = account[0].OverdraftLimit
                };

                commandBus.Fire(cmd);

                return JsonConvert.SerializeObject($"Overdraft limit set successfully");
            }
        }
        [HttpPost("[action]")]
        public async Task<string> SetDailyWireTransferLimit()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                var reqData = await reader.ReadToEndAsync();
                var account = new List<Account> { JsonConvert.DeserializeObject<Account>(reqData) };

                var commandBus = new Dispatcher(
                    name: "Command Bus",
                    watchSlowMsg: false,
                    slowMsgThreshold: TimeSpan.FromSeconds(100),
                    slowCmdThreshold: TimeSpan.FromSeconds(100));

                new AccountCommandHandler(_fixture.Repository, commandBus);

                var cmd = new SetDailyWireTransferLimit()
                {
                    AccountId = account[0].AccountId,
                    DailyWireTransferLimit = account[0].DailyWireTransferLimit
                };

                commandBus.Fire(cmd);

                return JsonConvert.SerializeObject($"Daily wire transfer limit set successfully");
            }
        }
        [HttpPost("[action]")]
        public async Task<string> DepositCheque()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                var reqData = await reader.ReadToEndAsync();
                var account = new List<Account> { JsonConvert.DeserializeObject<Account>(reqData) };

                var commandBus = new Dispatcher(
                    name: "Command Bus",
                    watchSlowMsg: false,
                    slowMsgThreshold: TimeSpan.FromSeconds(100),
                    slowCmdThreshold: TimeSpan.FromSeconds(100));

                new AccountCommandHandler(_fixture.Repository, commandBus);

                var cmd = new DepositCheque()
                {
                    AccountId = account[0].AccountId,
                    DepositDate = account[0].DepositDate,
                    Fund = account[0].Fund
                };

                commandBus.Fire(cmd);

                return JsonConvert.SerializeObject($"Cheque deposited successfully");
            }
        }
        [HttpPost("[action]")]
        public async Task<string> DepositCash()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                var reqData = await reader.ReadToEndAsync();
                var account = new List<Account> { JsonConvert.DeserializeObject<Account>(reqData) };

                var commandBus = new Dispatcher(
                    name: "Command Bus",
                    watchSlowMsg: false,
                    slowMsgThreshold: TimeSpan.FromSeconds(100),
                    slowCmdThreshold: TimeSpan.FromSeconds(100));

                new AccountCommandHandler(_fixture.Repository, commandBus);

                var cmd = new DepositCash()
                {
                    AccountId = account[0].AccountId,
                    Fund = account[0].Fund
                };

                commandBus.Fire(cmd);

                return JsonConvert.SerializeObject($"Cash deposited successfully");
            }
        }
        [HttpPost("[action]")]
        public async Task<string> WithdrawCash()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                var reqData = await reader.ReadToEndAsync();
                var account = new List<Account> { JsonConvert.DeserializeObject<Account>(reqData) };

                var commandBus = new Dispatcher(
                    name: "Command Bus",
                    watchSlowMsg: false,
                    slowMsgThreshold: TimeSpan.FromSeconds(100),
                    slowCmdThreshold: TimeSpan.FromSeconds(100));

                new AccountCommandHandler(_fixture.Repository, commandBus);

                var cmd = new WithdrawCash()
                {
                    AccountId = account[0].AccountId,
                    Fund = account[0].Fund
                };

                commandBus.Fire(cmd);

                return JsonConvert.SerializeObject($"Cash withdrawn successfully");
            }
        }
        [HttpPost("[action]")]
        public async Task<string> WireTransfer()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                var reqData = await reader.ReadToEndAsync();
                var account = new List<Account> { JsonConvert.DeserializeObject<Account>(reqData) };

                var commandBus = new Dispatcher(
                    name: "Command Bus",
                    watchSlowMsg: false,
                    slowMsgThreshold: TimeSpan.FromSeconds(100),
                    slowCmdThreshold: TimeSpan.FromSeconds(100));

                new AccountCommandHandler(_fixture.Repository, commandBus);

                var cmd = new WireTransfer()
                {
                    AccountId = account[0].AccountId,
                    WireTransferDate = account[0].WireTransferDate,
                    Fund = account[0].Fund
                };

                commandBus.Fire(cmd);

                return JsonConvert.SerializeObject($"Wire transferred successfully");
            }
        }
    }
}