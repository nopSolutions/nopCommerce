using System.Web.Mvc;

namespace Nop.Web.MVC.Controllers
{
    public class CatalogController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("List", "Category", new { area = "Admin" });
            return RedirectToAction("List", "Home", new { area = "Public" });
            //return Content("Catalog Index", "text/html");
        }
    }
}
