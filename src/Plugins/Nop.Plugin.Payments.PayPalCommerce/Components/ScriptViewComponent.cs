using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.PayPalCommerce.Components;

/// <summary>
/// Represents the view component to add script to pages
/// </summary>
public class ScriptViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly PayPalCommerceSettings _settings;
    protected readonly ServiceManager _serviceManager;

    #endregion

    #region Ctor

    public ScriptViewComponent(IPaymentPluginManager paymentPluginManager,
        IStoreContext storeContext,
        IWorkContext workContext,
        PayPalCommerceSettings settings,
        ServiceManager serviceManager)
    {
        _paymentPluginManager = paymentPluginManager;
        _storeContext = storeContext;
        _workContext = workContext;
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
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        if (!await _paymentPluginManager.IsPluginActiveAsync(PayPalCommerceDefaults.SystemName, customer, store?.Id ?? 0))
            return Content(string.Empty);

        if (!ServiceManager.IsConfigured(_settings))
            return Content(string.Empty);

        if (!widgetZone.Equals(PublicWidgetZones.CheckoutPaymentInfoTop) &&
            !widgetZone.Equals(PublicWidgetZones.OpcContentBefore) &&
            !widgetZone.Equals(PublicWidgetZones.ProductDetailsTop) &&
            !widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore))
        {
            return Content(string.Empty);
        }

        if (widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore))
        {
            if (!_settings.DisplayButtonsOnShoppingCart)
                return Content(string.Empty);

            var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
            if (routeName != PayPalCommerceDefaults.ShoppingCartRouteName)
                return Content(string.Empty);
        }

        if (widgetZone.Equals(PublicWidgetZones.ProductDetailsTop) && !_settings.DisplayButtonsOnProductDetails)
            return Content(string.Empty);

        var (script, _) = await _serviceManager.GetScriptAsync(_settings, widgetZone);
        return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));
    }

    #endregion
}