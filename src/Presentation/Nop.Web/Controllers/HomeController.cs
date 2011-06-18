using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class HomeController : BaseNopController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
