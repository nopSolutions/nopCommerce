using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Plugin.Payments.PeachPayments.Services;
using Nop.Services.Catalog;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Payments.PeachPayments.Components
{
    [ViewComponent(Name = PeachPaymentsDefaults.BUTTONS_VIEW_COMPONENT_NAME)]
    public class ButtonsViewComponent:NopViewComponent
    {

        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductService _productServise;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly PeachPaymentsSettings _settings;

        public ButtonsViewComponent(IPaymentPluginManager paymentPluginManager,
           IPriceCalculationService priceCalculationService,
           IProductService productServise,
           IStoreContext storeContext,
           IWorkContext workContext,
           PeachPaymentsSettings settings)
        {
            _paymentPluginManager = paymentPluginManager;
            _priceCalculationService = priceCalculationService;
            _productServise = productServise;
            _storeContext = storeContext;
            _workContext = workContext;
            _settings = settings;
        }
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(PeachPaymentsDefaults.SystemName, customer, store?.Id ?? 0))
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
                if (routeName != PeachPaymentsDefaults.ShoppingCartRouteName)
                    return Content(string.Empty);
            }

            if (widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo) && !_settings.DisplayButtonsOnProductDetails)
                return Content(string.Empty);

            var productId = additionalData is ProductDetailsModel.AddToCartModel model ? model.ProductId : 0;
            var productCost = "0.00";
            if (productId > 0)
            {
                var product = await _productServise.GetProductByIdAsync(productId);
                var finalPrice = (await _priceCalculationService.GetFinalPriceAsync(product, customer,store)).finalPrice;
                productCost = finalPrice.ToString("0.00", CultureInfo.InvariantCulture);
            }
            return View("~/Plugins/Payments.PeachPayments/Views/Buttons.cshtml", (widgetZone, productId, productCost));
        }
    }
}
