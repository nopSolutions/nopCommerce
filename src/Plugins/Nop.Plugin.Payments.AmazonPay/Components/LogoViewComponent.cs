using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Plugin.Payments.AmazonPay.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.AmazonPay.Components;

/// <summary>
/// Represents the view component to display logo
/// </summary>
public class LogoViewComponent(AmazonPayApiService amazonPayApiService,
        AmazonPaySettings amazonPaySettings) : NopViewComponent
{
    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="_">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object _)
    {
        if (!widgetZone.Equals(PublicWidgetZones.Footer))
            return Content(string.Empty);

        if (!await amazonPayApiService.IsActiveAndConfiguredAsync())
            return Content(string.Empty);

        return new HtmlContentViewComponentResult(new HtmlString(amazonPaySettings.LogoInFooter ?? string.Empty));
    }

    #endregion
}