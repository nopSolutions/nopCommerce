using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Tax.FixedRate.Controllers
{
    [AdminAuthorize]
    public class FixedRateController : Controller
    {
        public ActionResult Configure()
        {
            return View();
        }
    }
}
