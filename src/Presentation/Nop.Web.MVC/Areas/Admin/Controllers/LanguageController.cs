using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.MVC.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class LanguageController : Controller
    {
        public ActionResult Index()
        {
            return View("List");
        }

    }
}
