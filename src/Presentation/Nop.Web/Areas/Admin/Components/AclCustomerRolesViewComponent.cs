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

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(object additionalData)
    {
        if (additionalData is not IAclSupportedModel model)
            return Content(string.Empty);
        
        return await ViewAsync(model);
    }

    #endregion
}