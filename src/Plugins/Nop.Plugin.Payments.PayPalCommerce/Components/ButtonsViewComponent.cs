using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Services.Catalog;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Payments.PayPalCommerce.Components
{
    /// <summary>
    /// Represents the view component to display buttons
    /// </summary>
    public class ButtonsViewComponent : NopViewComponent
    {
        #region Fields

        protected readonly IPaymentPluginManager _paymentPluginManager;
        protected readonly IPriceCalculationService _priceCalculationService;
        protected readonly IProductService _productServise;
        protected readonly IStoreContext _storeContext;
        protected readonly IWorkContext _workContext;
        protected readonly PayPalCommerceSettings _settings;

        #endregion

        #region Ctor

        public ButtonsViewComponent(IPaymentPluginManager paymentPluginManager,
            IPriceCalculationService priceCalculationService,
            IProductService productServise,
            IStoreContext storeContext,
            IWorkContext workContext,
            PayPalCommerceSettings settings)
        {
            _paymentPluginManager = paymentPluginManager;
            _priceCalculationService = priceCalculationService;
            _productServise = productServise;
            _storeContext = storeContext;
            _workContext = workContext;
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
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(PayPalCommerceDefaults.SystemName, customer, store?.Id ?? 0))
                return Content(string.Empty);

            if (!ServiceManager.IsConfigured(_settings))
                return Content(string.Empty);

            if (!widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo) && !widgetZone.Equals(PublicWidgetZones.OrderSummaryContentAfter))
                return Content(string.Empty);

            if (widgetZone.Equals(PublicWidgetZones.OrderSummaryContentAfter))
            {
                if (!_settings.DisplayButtonsOnShoppingCart)
                    return Content(string.Empty);

                var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
                if (routeName != PayPalCommerceDefaults.ShoppingCartRouteName)
                    return Content(string.Empty);
            }

            if (widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo) && !_settings.DisplayButtonsOnProductDetails)
                return Content(string.Empty);

            var productId = additionalData is ProductDetailsModel.AddToCartModel model ? model.ProductId : 0;
            var productCost = "0.00";
            if (productId > 0)
            {
                var product = await _productServise.GetProductByIdAsync(productId);
                var finalPrice = (await _priceCalculationService.GetFinalPriceAsync(product, customer, store)).finalPrice;
                productCost = finalPrice.ToString("0.00", CultureInfo.InvariantCulture);
            }
            return View("~/Plugins/Payments.PayPalCommerce/Views/Buttons.cshtml", (widgetZone, productId, productCost));
        }

        #endregion
    }
}