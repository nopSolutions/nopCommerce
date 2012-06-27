using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public partial class KeepAliveController : Controller
    {
        public ActionResult Index()
        {
            return Content("I am alive!");
        }
    }
}
