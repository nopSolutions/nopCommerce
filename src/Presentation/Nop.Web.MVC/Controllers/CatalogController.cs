using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.MVC.Controllers
{
    public class CatalogController : Controller
    {
        //
        // GET: /Catalog/

        public ActionResult Index()
        {
            return Content("Catalog Index", "text/html");
        }

        public ActionResult Test()
        {
            return View();
        }

    }
}
