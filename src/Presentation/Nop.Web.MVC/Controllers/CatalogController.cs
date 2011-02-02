using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.MVC.Controllers
{
    public class CatalogController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("List", "CategoryAdmin", new { area = "Categories" });
            //return Content("Catalog Index", "text/html");
        }
    }
}
