using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Widgets.AbcPickupInStore;
using Nop.Plugin.Widgets.AbcPickupInStore.Models;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain;
using SevenSpikes.Nop.Plugins.StoreLocator.Services;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.AbcPickupInStore.Factories;
using Nop.Plugin.Misc.AbcCore.Infrastructure;

namespace Nop.Plugin.Widgets.AbcPickupInStore.Components
{
    [ViewComponent(Name = "AbcPickupInStore")]
    public class AbcPickupInStoreViewComponent : NopViewComponent
    {
        private readonly string DELIVERY_SELECTION_WIDGET_ZONE =
            CustomPublicWidgetZones.ProductDetailsBeforeAddToCart;
        private readonly string PICKUP_INFO_WIDGET_ZONE =
            "productdetails_before_tabs";

        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IPickStoreModelFactory _pickStoreModelFactory;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ICustomerShopService _customerShopService;
        private readonly IShopService _shopService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly PickupInStoreSettings _pickupInStoreSettings;
        private readonly StoreLocatorSettings _storeLocatorSettings;

        public AbcPickupInStoreViewComponent(
            ILogger logger,
            IStoreContext storeContext,
            IWorkContext workContext,
            IPickStoreModelFactory pickStoreModelFactory,
            IProductService productService,
            IProductAttributeService productAttributeService,
            ICustomerShopService customerShopService,
            IShopService shopService,
            IShoppingCartService shoppingCartService,
            PickupInStoreSettings pickupInStoreSettings,
            StoreLocatorSettings storeLocatorSettings
        )
        {
            _logger = logger;
            _storeContext = storeContext;
            _workContext = workContext;
            _pickStoreModelFactory = pickStoreModelFactory;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _customerShopService = customerShopService;
            _shopService = shopService;
            _shoppingCartService = shoppingCartService;
            _pickupInStoreSettings = pickupInStoreSettings;
            _storeLocatorSettings = storeLocatorSettings;
        }
        public async Task<IViewComponentResult> InvokeAsync(
            string widgetZone,
            object additionalData = null
        )
        {
            int productId = -1;
            if (additionalData != null && additionalData is ProductDetailsModel)
            {
                productId = (additionalData as ProductDetailsModel).Id;
            }
            else if (additionalData != null && additionalData is int)
            {
                productId = (int)additionalData;
            }

            Product product = await _productService.GetProductByIdAsync(productId);
            var storeUrl = (await _storeContext.GetCurrentStoreAsync()).Url;
            var isClearanceStore = storeUrl.Contains("clearance");
            if (await product.HasDeliveryOptionsAsync() && !isClearanceStore)
            {
                return Content("");
            }
            // clearance store specific
            else if (isClearanceStore)
            {
                if (widgetZone == PICKUP_INFO_WIDGET_ZONE)
                {
                    return View(
                        "~/Plugins/Widgets.AbcPickupInStore/Views/ClearanceStoreStockContainer.cshtml",
                        productId
                    );
                }
                else
                {
                    return Content("");
                }
            }
            // normal abc store
            else
            {
                // if the buy button is disabled, do not show any UI about
                // products
                if (productId > 0 && (product.DisableBuyButton || !product.Published))
                {
                    return Content("");
                }

                PickStoreModel model = await _pickStoreModelFactory.InitializePickStoreModelAsync();
                model.ProductId = productId;

                if (string.IsNullOrWhiteSpace(model.GoogleMapsAPIKey))
                {
                    await _logger.WarningAsync(
                        "Google Maps API Key not included in Store Locator " +
                        "settings, will have issues with ABC Pickup in Store");
                }

                // if item is pickup in store, then display the UI for pickup
                // in store
                var pams = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                var pamsWithFedex = await pams.WhereAwait(
                    async pam => (await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId)).Name == "FedEx"
                ).ToListAsync();
                var pamsWithPickup = await pams.WhereAwait(
                    async pam => (await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId)).Name == "Pickup"
                ).ToListAsync();

                if (pamsWithFedex.Any())
                {
                    model.HasFedExAttribute = true;
                }

                if (pamsWithPickup.Any())
                {
                    string url = "";
                    if (widgetZone == PICKUP_INFO_WIDGET_ZONE)
                    {
                        url = "~/Plugins/Widgets.AbcPickupInStore/Views/PickupInStoreContainer.cshtml";
                    }
                    return View(url, model);
                }
                else
                {
                    return Content("");
                }
            }
        }
    }
}
