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
    }
}