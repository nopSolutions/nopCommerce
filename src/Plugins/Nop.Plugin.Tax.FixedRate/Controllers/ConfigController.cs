using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Plugin.Tax.FixedRate.Models;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Tax.FixedRate.Controllers
{
    [AdminAuthorize]
    public class ConfigController : Controller
    {
        public ConfigController(ILogger logger)
        {
            //TODO don't pass logger as parameter (did it to test injection for plugin controllers)
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new TestModel()
            {
                TestProperty1 = "TestProperty1 value"
            };
            return View(model);
        }


        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(TestModel model)
        {
            return View(model);
        }
    }
}
