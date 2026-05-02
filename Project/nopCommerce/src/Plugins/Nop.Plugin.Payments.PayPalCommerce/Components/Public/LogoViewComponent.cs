using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.PayPalCommerce.Components.Public;

/// <summary>
/// Represents the view component to display PayPal logo in the public store
/// </summary>
public class LogoViewComponent : NopViewComponent
{
    #region Fields

    private readonly PayPalCommerceSettings _settings;
    private readonly PayPalCommerceServiceManager _serviceManager;

    #endregion

    #region Ctor

    public LogoViewComponent(PayPalCommerceSettings settings,
        PayPalCommerceServiceManager serviceManager)
    {
        _settings = settings;
        _serviceManager = serviceManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var (active, _) = await _serviceManager.IsActiveAsync(_settings);
        if (!active)
            return Content(string.Empty);

        var script = widgetZone.Equals(PublicWidgetZones.HeaderLinksBefore) && _settings.DisplayLogoInHeaderLinks
            ? _settings.LogoInHeaderLinks
            : widgetZone.Equals(PublicWidgetZones.Footer) && _settings.DisplayLogoInFooter
            ? _settings.LogoInFooter
            : null;

        return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));
    }

    #endregion
}