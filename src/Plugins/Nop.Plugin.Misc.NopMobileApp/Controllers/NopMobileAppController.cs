using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.NopMobileApp.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class NopMobileAppController : BasePluginController
{
    #region Fields

    private readonly IPermissionService _permissionService;

    #endregion

    #region Ctor 

    public NopMobileAppController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual IActionResult Configure()
    {
        return View("~/Plugins/Misc.NopMobileApp/Views/Configure.cshtml");
    }

    #endregion
}