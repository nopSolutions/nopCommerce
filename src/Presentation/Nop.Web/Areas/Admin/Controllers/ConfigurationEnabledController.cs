using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Areas.Admin.Controllers;

public class ConfigurationEnabledController : BaseAdminController
{
    public IActionResult Index()
    {
        return View();
    }

}