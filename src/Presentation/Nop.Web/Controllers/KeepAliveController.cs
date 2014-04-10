using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra acion filters will be called
    //they can create guest account(s), etc
    public partial class KeepAliveController : Controller
    {
        public ActionResult Index()
        {
            return Content("I am alive!");
        }
    }
}
