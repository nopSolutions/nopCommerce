using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers;

public partial class HomeController : BasePublicController
{
    [SaveLastContinueShoppingPage]
    public virtual IActionResult Index()
    {
        return View();
    }
}