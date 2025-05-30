using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Factories;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.PayPalCommerce.Components.Public;

/// <summary>
/// Represents the view component to display Pay Later messages in the public store
/// </summary>
public class MessagesViewComponent : NopViewComponent
{
    #region Fields

    private readonly PayPalCommerceModelFactory _modelFactory;
    private readonly PayPalCommerceServiceManager _serviceManager;
    private readonly PayPalCommerceSettings _settings;

    #endregion

    #region Ctor

    public MessagesViewComponent(PayPalCommerceModelFactory modelFactory,
        PayPalCommerceServiceManager serviceManager,
        PayPalCommerceSettings settings)
    {
        _modelFactory = modelFactory;
        _serviceManager = serviceManager;
        _settings = settings;
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

        if (!widgetZone.Equals(PublicWidgetZones.OrderSummaryTotals))
            return Content(string.Empty);

        if (!_settings.UseSandbox && !_settings.ConfiguratorSupported)
            return Content(string.Empty);

        //get messages placement
        var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
        var isCartPage = routeName == PayPalCommerceDefaults.Route.ShoppingCart;
        var isPaymentMethodPage = routeName == PayPalCommerceDefaults.Route.PaymentInfo;
        var isCheckoutPage = (HttpContext.Request.RouteValues?.TryGetValue("controller", out var controller) ?? false) &&
            string.Equals(controller.ToString(), "Checkout", StringComparison.InvariantCultureIgnoreCase);
        if (!isCartPage && !isPaymentMethodPage && !isCheckoutPage)
            return Content(string.Empty);

        //load script only on checkout pages (excluding payment method page) to avoid double loading
        var loadScript = !isCartPage && !isPaymentMethodPage;
        var placement = isCartPage ? ButtonPlacement.Cart : ButtonPlacement.PaymentMethod;
        var model = await _modelFactory.PrepareMessagesModelAsync(placement, loadScript);

        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/_Messages.cshtml", model);
    }

    #endregion
}