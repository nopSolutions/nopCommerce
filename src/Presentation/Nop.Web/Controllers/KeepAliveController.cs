using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Controllers;

//do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
//they can create guest account(s), etc
public partial class KeepAliveController : Controller
{
    public virtual IActionResult Index()
    {
        return Content("I am alive!");
    }
}