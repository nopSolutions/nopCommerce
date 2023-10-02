using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using AbcWarehouse.Plugin.Widgets.UniFi.Models;
using Nop.Plugin.Misc.AbcCore.Infrastructure;
using Nop.Plugin.Misc.AbcCore.Services;

namespace AbcWarehouse.Plugin.Widgets.UniFi.Components
{
    public class UniFiViewComponent : NopViewComponent
    {
        private readonly ILogger _logger;
        private readonly IProductAbcDescriptionService _productAbcDescriptionService;
        private readonly IProductAbcFinanceService _productAbcFinanceService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly UniFiSettings _settings;

        public UniFiViewComponent(
            ILogger logger,
            IProductAbcDescriptionService productAbcDescriptionService,
            IProductAbcFinanceService productAbcFinanceService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext,
            UniFiSettings settings)
        {
            _logger = logger;
            _productAbcDescriptionService = productAbcDescriptionService;
            _productAbcFinanceService = productAbcFinanceService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _settings = settings;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            if (widgetZone == PublicWidgetZones.BodyEndHtmlTagBefore)
            {
                var partnerId = _settings.PartnerId;
                if (string.IsNullOrWhiteSpace(partnerId))
                {
                    await _logger.ErrorAsync("Widgets.UniFi: Partner ID not defined, add in Settings.");
                    return Content(string.Empty);
                }

                var model = new UniFiModel
                {
                    PartnerId = partnerId
                };

                if (IsProductDetailsPage())
                {
                    model.FlowType = "PDP";

                    var routeData = Url.ActionContext.RouteData;
                    var productIdAsString = routeData.Values["productId"].ToString();
                    var productId = int.Parse(productIdAsString);

                    if (productId != 0)
                    {
                        var productAbcDescription =
                            await _productAbcDescriptionService.GetProductAbcDescriptionByProductIdAsync(productId);
                        if (productAbcDescription != null)
                        {
                            var productAbcFinance =
                                await _productAbcFinanceService.GetProductAbcFinanceByAbcItemNumberAsync(productAbcDescription.AbcItemNumber);
                            if (productAbcFinance != null)
                            {
                                model.Tags = productAbcFinance.TransPromo;
                            }
                        }
                    }
                }

                if (IsCheckout())
                {
                    model.FlowType = "CHECKOUT";

                    var customer = await _workContext.GetCurrentCustomerAsync();
                    var cart = await _shoppingCartService.GetShoppingCartAsync(
                        customer,
                        ShoppingCartType.ShoppingCart,
                        (await _storeContext.GetCurrentStoreAsync()).Id);

                    foreach (var item in cart)
                    {
                        var productAbcDescription =
                            await _productAbcDescriptionService.GetProductAbcDescriptionByProductIdAsync(item.ProductId);
                        if (productAbcDescription != null)
                        {
                            var productAbcFinance =
                                await _productAbcFinanceService.GetProductAbcFinanceByAbcItemNumberAsync(productAbcDescription.AbcItemNumber);
                            if (productAbcFinance != null)
                            {
                                if (string.IsNullOrWhiteSpace(model.Tags))
                                {
                                    model.Tags = productAbcFinance.TransPromo;
                                }
                                else
                                {
                                    var existingTransPromos = model.Tags.Split(',');
                                    if (!existingTransPromos.Contains(productAbcFinance.TransPromo))
                                    {
                                        model.Tags += $",{productAbcFinance.TransPromo}";
                                    }
                                }
                            }
                        }
                    }
                }

                return View("~/Plugins/Widgets.UniFi/Views/Framework.cshtml", model);
            }

            if (widgetZone == CustomPublicWidgetZones.ProductDetailsAfterPrice)
            {
                return View("~/Plugins/Widgets.UniFi/Views/SyncPrice.cshtml");
            }

            return Content(string.Empty);
        }

        private bool IsProductDetailsPage()
        {
            var routeData = Url.ActionContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            return controller.ToString() == "Product" && action.ToString() == "ProductDetails";
        }

        private bool IsCheckout()
        {
            var routeData = Url.ActionContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            return controller.ToString() == "CustomCheckout" && action.ToString() == "PaymentInfo";
        }
    }
}