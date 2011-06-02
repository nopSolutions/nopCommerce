using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeController : BaseNopController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
