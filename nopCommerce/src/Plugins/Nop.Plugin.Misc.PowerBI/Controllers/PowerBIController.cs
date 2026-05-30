using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.PowerBI.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class PowerBIController : BasePluginController
{
    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Configure()
    {
        return View("~/Plugins/Misc.PowerBI/Views/Configure.cshtml");
    }

    #endregion
}
