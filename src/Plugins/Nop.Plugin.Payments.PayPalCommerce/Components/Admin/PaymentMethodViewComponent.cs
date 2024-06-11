using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.PayPalCommerce.Components.Admin;

/// <summary>
/// Represents the view component to display PayPal on the payment methods page in the admin area
/// </summary>
public class PaymentMethodViewComponent : NopViewComponent
{
    #region Fields

    private readonly PayPalCommerceSettings _settings;
    private readonly PayPalCommerceServiceManager _serviceManager;

    #endregion

    #region Ctor

    public PaymentMethodViewComponent(PayPalCommerceSettings settings,
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
        if (!widgetZone.Equals(AdminWidgetZones.PaymentMethodListTop))
            return Content(string.Empty);

        var (active, _) = await _serviceManager.IsActiveAsync(_settings);

        return View("~/Plugins/Payments.PayPalCommerce/Views/Admin/_PaymentMethod.cshtml", active);
    }

    #endregion
}