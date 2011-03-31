using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Tax.Free.Controllers
{
    [AdminAuthorize]
    public class ConfigController : Controller
    {
        public ActionResult Configure2()
        {
            return View();
        }
    }
}
