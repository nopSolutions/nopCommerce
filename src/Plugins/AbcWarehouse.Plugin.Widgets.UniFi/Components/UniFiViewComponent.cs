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
using Nop.Plugin.Misc.AbcCore.Mattresses;

namespace AbcWarehouse.Plugin.Widgets.UniFi.Components
{
    public class UniFiViewComponent : NopViewComponent
    {
        private readonly IAbcMattressEntryService _abcMattressEntryService;
        private readonly IAbcMattressModelService _abcMattressModelService;
        private readonly IAbcMattressProductService _abcMattressProductService;
        private readonly ILogger _logger;
        private readonly IProductAbcDescriptionService _productAbcDescriptionService;
        private readonly IProductAbcFinanceService _productAbcFinanceService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly UniFiSettings _settings;

        public UniFiViewComponent(
            IAbcMattressEntryService abcMattressEntryService,
            IAbcMattressModelService abcMattressModelService,
            IAbcMattressProductService abcMattressProductService,
            ILogger logger,
            IProductAbcDescriptionService productAbcDescriptionService,
            IProductAbcFinanceService productAbcFinanceService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext,
            UniFiSettings settings)
        {
            _abcMattressEntryService = abcMattressEntryService;
            _abcMattressModelService = abcMattressModelService;
            _abcMattressProductService = abcMattressProductService;
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
                    PartnerId = partnerId,
                    Url = _settings.UseIntegration ?
                        "https://spdpone.syfpos.com/mpp/UniFi.js" :
                        "https://pdpone.syfpayments.com/mpp/UniFi.js"
                };

                if (IsProductDetailsPage())
                {
                    model.FlowType = "PDP";

                    var routeData = Url.ActionContext.RouteData;
                    var productIdAsString = routeData.Values["productId"].ToString();
                    var productId = int.Parse(productIdAsString);

                    if (productId != 0)
                    {
                        string abcItemNumber = null;

                        // need to check if this is a mattress
                        var isMattress = _abcMattressProductService.IsMattressProduct(productId);
                        if (isMattress)
                        {
                            var abcMattressModel = _abcMattressModelService.GetAbcMattressModelByProductId(productId);
                            // just to keep it simple, grab the first entry
                            var abcMattressEntry = _abcMattressEntryService.GetAbcMattressEntriesByModelId(abcMattressModel.Id).First();
                            
                            abcItemNumber = abcMattressEntry?.ItemNo;
                        }
                        else
                        {
                            var productAbcDescription = await _productAbcDescriptionService.GetProductAbcDescriptionByProductIdAsync(productId);
                            abcItemNumber = productAbcDescription?.AbcItemNumber;
                        }

                        if (abcItemNumber != null)
                        {
                            var productAbcFinance =
                                await _productAbcFinanceService.GetProductAbcFinanceByAbcItemNumberAsync(abcItemNumber);
                            model.AbcItemNumber = abcItemNumber;
                            if (productAbcFinance != null)
                            {
                                model.Tags = productAbcFinance.TransPromo;
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
    }
}