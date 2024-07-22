using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Plugin.Payments.AmazonPay.Services;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Payments.AmazonPay.Components;

/// <summary>
/// Represents the view component to display payment button
/// </summary>
public class PaymentButtonViewComponent : NopViewComponent
{
    #region Fields

    private readonly AmazonPayApiService _amazonPayApiService;
    private readonly AmazonPayCheckoutService _amazonPayCheckoutService;
    private readonly AmazonPaySettings _amazonPaySettings;
    private readonly DisallowedProducts _disallowedProducts;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly OrderSettings _orderSettings;

    #endregion

    #region Ctor

    public PaymentButtonViewComponent(AmazonPayApiService amazonPayApiService,
        AmazonPayCheckoutService amazonPayCheckoutService,
        AmazonPaySettings amazonPaySettings,
        DisallowedProducts disallowedProducts,
        IHttpContextAccessor httpContextAccessor,
        IShoppingCartService shoppingCartService,
        OrderSettings orderSettings)
    {
        _amazonPayApiService = amazonPayApiService;
        _amazonPayCheckoutService = amazonPayCheckoutService;
        _amazonPaySettings = amazonPaySettings;
        _disallowedProducts = disallowedProducts;
        _httpContextAccessor = httpContextAccessor;
        _shoppingCartService = shoppingCartService;
        _orderSettings = orderSettings;
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
        //ensure that plugin is active and configured
        if (!await _amazonPayApiService.IsActiveAndConfiguredAsync())
            return Content(string.Empty);

        if (widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo))
        {
            if (!_amazonPaySettings.ButtonPlacement.Contains(ButtonPlacement.Product))
                return Content(string.Empty);

            var productId = additionalData is ProductDetailsModel.AddToCartModel data ? (int?)data.ProductId : null;
            var model = await _amazonPayCheckoutService.GetPaymentInfoModelAsync(ButtonPlacement.Product, productId);
            if (model is null)
                return Content(string.Empty);

            model.IsCartContainsNoAllowedProducts = model.IsCartContainsNoAllowedProducts || await _disallowedProducts.IsProductDisallowAsync(model.ProductId ?? 0);

            return View("~/Plugins/Payments.AmazonPay/Views/PaymentInfo.cshtml", model);
        }

        if (widgetZone.Equals(PublicWidgetZones.BodyStartHtmlTagAfter))
        {
            if (!_amazonPaySettings.ButtonPlacement.Contains(ButtonPlacement.MiniCart))
                return Content(string.Empty);

            var model = await _amazonPayCheckoutService.GetPaymentInfoModelAsync(ButtonPlacement.MiniCart);
            if (model is null)
                return Content(string.Empty);

            return View("~/Plugins/Payments.AmazonPay/Views/MiniCart.cshtml", model);
        }

        if (!new List<string> { PublicWidgetZones.CheckoutProgressBefore, PublicWidgetZones.OpcContentBefore }.Contains(widgetZone))
            return Content(string.Empty);

        if (!_amazonPaySettings.ButtonPlacement.Contains(ButtonPlacement.Checkout))
            return Content(string.Empty);

        var values = _httpContextAccessor.HttpContext?.Request.RouteValues;
        if (values == null)
            return Content(string.Empty);

        var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
        if (routeName == null)
            return Content(string.Empty);

        if (_orderSettings.OnePageCheckoutEnabled)
        {
            if (!routeName.Equals(AmazonPayDefaults.OnePageCheckoutRouteName))
                return Content(string.Empty);

            var onePageModel = await _amazonPayCheckoutService.GetPaymentInfoModelAsync(ButtonPlacement.Checkout);
            if (onePageModel is null)
                return Content(string.Empty);

            return View("~/Plugins/Payments.AmazonPay/Views/PaymentInfo.cshtml", onePageModel);
        }

        if (!_orderSettings.DisableBillingAddressCheckoutStep
            && !routeName.Equals("CheckoutBillingAddress", StringComparison.InvariantCultureIgnoreCase))
            return Content(string.Empty);

        if (_orderSettings.DisableBillingAddressCheckoutStep
            && await _shoppingCartService.ShoppingCartRequiresShippingAsync(await _amazonPayCheckoutService.GetCartAsync())
            && !routeName.Equals("CheckoutShippingAddress", StringComparison.InvariantCultureIgnoreCase))
            return Content(string.Empty);

        if (_orderSettings.DisableBillingAddressCheckoutStep
            && !await _shoppingCartService.ShoppingCartRequiresShippingAsync(await _amazonPayCheckoutService.GetCartAsync())
            && !routeName.Equals("CheckoutShippingMethod", StringComparison.InvariantCultureIgnoreCase))
            return Content(string.Empty);

        var checkoutModel = await _amazonPayCheckoutService.GetPaymentInfoModelAsync(ButtonPlacement.Checkout);
        if (checkoutModel is null)
            return Content(string.Empty);

        return View("~/Plugins/Payments.AmazonPay/Views/PaymentInfo.cshtml", checkoutModel);
    }

    #endregion
}