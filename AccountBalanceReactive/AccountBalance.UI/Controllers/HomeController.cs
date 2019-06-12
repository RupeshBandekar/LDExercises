using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AccountBalance.UI.Controllers
{
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
    }
}