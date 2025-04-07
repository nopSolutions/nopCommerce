using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Factories;
using Nop.Plugin.Payments.PayPalCommerce.Models.Public;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Services.Catalog;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Payments.PayPalCommerce.Components.Public;

/// <summary>
/// Represents the view component to display PayPal buttons in the public store
/// </summary>
public class ButtonsViewComponent(IProductService productService,
        PayPalCommerceModelFactory modelFactory,
        PayPalCommerceServiceManager serviceManager,
        PayPalCommerceSettings settings) : NopViewComponent
{
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
        var (active, _) = await serviceManager.IsActiveAsync(settings);
        if (!active)
            return Content(string.Empty);

        PaymentInfoModel model = null;

        if (widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo))
        {
            if (settings.DisplayButtonsOnProductDetails)
            {
                var productId = additionalData is ProductDetailsModel.AddToCartModel product ? (int?)product.ProductId : null;
                if (productId is null || (await productService.GetProductByIdAsync(productId ?? 0))?.ParentGroupedProductId == 0)
                    model = await modelFactory.PreparePaymentInfoModelAsync(ButtonPlacement.Product, productId);
            }
        }
        else if (widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore))
        {
            if (settings.DisplayButtonsOnShoppingCart)
            {
                var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
                if (routeName == PayPalCommerceDefaults.Route.ShoppingCart)
                    model = await modelFactory.PreparePaymentInfoModelAsync(ButtonPlacement.Cart);
            }
        }
        else
        {
            if (settings.DisplayButtonsOnPaymentMethod)
                model = await modelFactory.PreparePaymentInfoModelAsync(ButtonPlacement.PaymentMethod);
        }

        if (model?.Cart.IsRecurring is null)
            return Content(string.Empty);

        if (model.Cart.IsRecurring == true)
            return View("~/Plugins/Payments.PayPalCommerce/Views/Public/_Buttons.Recurring.cshtml", model);

        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/_Buttons.cshtml", model);
    }

    #endregion
}