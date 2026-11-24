using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.PowerBI.Admin.Controllers;

public class PowerBIController : BasePluginAdminController
{
    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Configure()
    {
        return View("~/Plugins/Misc.PowerBI/Views/Configure.cshtml");
    }

    #endregion
}
