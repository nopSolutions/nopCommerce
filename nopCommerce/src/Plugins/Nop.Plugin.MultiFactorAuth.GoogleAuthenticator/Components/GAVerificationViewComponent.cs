using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Components;

/// <summary>
/// Represents view component for verification GoogleAuthenticator
/// </summary>
public class GAVerificationViewComponent : NopViewComponent
{
    #region Ctor

    public GAVerificationViewComponent()
    {

    }

    #endregion

    #region Methods

    /// <summary>
    ///  Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>View component result</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var model = new TokenModel();
        return await ViewAsync("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/Customer/GAVefification.cshtml", model);
    }

    #endregion
}