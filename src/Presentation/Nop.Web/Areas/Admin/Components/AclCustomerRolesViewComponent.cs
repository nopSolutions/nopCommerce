using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents view component to display field to select customer roles
/// </summary>
public partial class AclCustomerRolesViewComponent : NopViewComponent
{
    #region Methods

    public IViewComponentResult Invoke(object additionalData)
    {
        if (additionalData is not IAclSupportedModel model)
            return Content(string.Empty);
        
        return View(model);
    }

    #endregion
}