using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Payments.AmazonPay.Models;
using Nop.Plugin.Payments.AmazonPay.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.AmazonPay.Components;

/// <summary>
/// Represents the view component to display change button
/// </summary>
public class ChangeButtonViewComponent : NopViewComponent
{
    #region Fields

    private readonly AmazonPayApiService _amazonPayApiService;
    private readonly AmazonPayCheckoutService _amazonPayCheckoutService;

    #endregion

    #region Ctor

    public ChangeButtonViewComponent(AmazonPayApiService amazonPayApiService,
        AmazonPayCheckoutService amazonPayCheckoutService)
    {
        _amazonPayApiService = amazonPayApiService;
        _amazonPayCheckoutService = amazonPayCheckoutService;
    }

    #endregion

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
        ChangeLinkModel model = null;

        if (widgetZone.Equals(PublicWidgetZones.OrderSummaryPaymentMethodInfo))
            model = new ChangeLinkModel { ChangeAction = "changePayment" };

        if (widgetZone.Equals(PublicWidgetZones.OrderSummaryShippingAddress))
            model = new ChangeLinkModel { ChangeAction = "changeAddress" };

        if (model == null)
            return Content(string.Empty);

        //ensure that plugin is active and configured
        if (!await _amazonPayApiService.IsActiveAndConfiguredAsync())
            return Content(string.Empty);

        var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
        if (routeName == null || routeName != AmazonPayDefaults.ConfirmRouteName)
            return Content(string.Empty);

        model.CheckoutSessionId = await _amazonPayCheckoutService.GetCheckoutSessionIdAsync();

        return View("~/Plugins/Payments.AmazonPay/Views/ChangeLink.cshtml", model);
    }

    #endregion
}
