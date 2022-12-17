using Nop.Plugin.Widgets.AbcPickupInStore.Models;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using System.Collections.Generic;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Services.Orders;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Localization;
using SevenSpikes.Nop.Plugins.StoreLocator.Services;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain;
using Nop.Core.Domain.Catalog;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.AbcCore.Models;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Services.Messages;
using Nop.Core.Domain.Orders;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AbcCore;

namespace Nop.Plugin.Widgets.AbcPickupInStore.Views.PickupInStore
{
    public class PickStoreController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShopService _shopService;
        private readonly IProductService _productService;
        private readonly ICustomerShopService _customerShopService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IBackendStockService _backendStockService;
        private readonly IAttributeUtilities _attributeUtilities;
        private readonly PickupInStoreSettings _pickupInStoreSettings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly StoreLocatorSettings _storeLocatorSettings;
        private readonly INotificationService _notificationService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly CoreSettings _coreSettings;

        public PickStoreController(
            IWorkContext workContext,
            IStoreContext storeContext,
            IShopService shopService,
            IProductService productService,
            ICustomerShopService customerShopService,
            IProductAttributeParser productAttributeParser,
            IShoppingCartService shoppingCartService,
            IBackendStockService backendStockService,
            IAttributeUtilities attributeUtilities,
            PickupInStoreSettings pickUpInStoreSettings,
            ISettingService settingService,
            ILocalizationService localizationService,
            ILogger logger,
            StoreLocatorSettings storeLocatorSettings,
            INotificationService notificationService,
            IProductAttributeService productAttributeService,
            CoreSettings coreSettings
        )
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _shopService = shopService;
            _productService = productService;
            _customerShopService = customerShopService;
            _productAttributeParser = productAttributeParser;
            _shoppingCartService = shoppingCartService;
            _backendStockService = backendStockService;
            _attributeUtilities = attributeUtilities;
            _pickupInStoreSettings = pickUpInStoreSettings;
            _settingService = settingService;
            _localizationService = localizationService;
            _logger = logger;
            _storeLocatorSettings = storeLocatorSettings;
            _notificationService = notificationService;
            _productAttributeService = productAttributeService;
            _coreSettings = coreSettings;
        }
        public IActionResult Configure()
        {
            return View("~/Plugins/Widgets.AbcPickupInStore/Views/Configure.cshtml", _pickupInStoreSettings.ToModel());
        }

        [HttpPost]
        public async Task<IActionResult> Configure(PickupInStoreModel model)
        {
            await _settingService.SaveSettingAsync(PickupInStoreSettings.FromModel(model));

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost]
        public async Task<IActionResult> StoreSelected(string shopId)
        {
            Shop shop = await _shopService.GetShopByIdAsync(Int32.Parse(shopId));
            _customerShopService.InsertOrUpdateCustomerShop((await _workContext.GetCurrentCustomerAsync()).Id, Int32.Parse(shopId));
            var shoppingCartItems = (await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart))
                .Select(sci => sci);
            // update all attribute definitions
            foreach (var cartItem in shoppingCartItems)
            {
                // update attribute definitions to include the proper name
                ProductAttributeMapping pickupAttribute = await _attributeUtilities.GetPickupAttributeMappingAsync(cartItem.AttributesXml);

                if (pickupAttribute != null)
                {
                    // check if product is available at the selected store
                    StockResponse stockResponse = await _backendStockService.GetApiStockAsync(cartItem.ProductId);
                    bool available = false;
                    if (stockResponse != null)
                    {
                        available = stockResponse.ProductStocks.Where(ps => ps.Available && ps.Shop.Id == shop.Id).Any();
                    }

                    if (available)
                    {
                        string removedAttr = _productAttributeParser.RemoveProductAttribute(cartItem.AttributesXml, pickupAttribute);

                        cartItem.AttributesXml = await _attributeUtilities.InsertPickupAttributeAsync(
                            await _productService.GetProductByIdAsync(cartItem.ProductId),
                            stockResponse,
                            removedAttr);
                    }
                    else
                    {
                        cartItem.AttributesXml = await _attributeUtilities.InsertHomeDeliveryAttributeAsync(
                            await _productService.GetProductByIdAsync(cartItem.ProductId),
                            cartItem.AttributesXml);
                    }
                    await _shoppingCartService.UpdateShoppingCartItemAsync(await _workContext.GetCurrentCustomerAsync(), cartItem.Id,
                        cartItem.AttributesXml, cartItem.CustomerEnteredPrice, null, null, cartItem.Quantity, false);

                }
            }
            return Json(await _shopService.GetShopByIdAsync(int.Parse(shopId)));
        }

        [HttpPost]
        public async Task<IActionResult> FilterClosestStores(double lat, double lng, int productId)
        {
            StockResponse stockResponse = await _backendStockService.GetApiStockAsync(productId);
            if (stockResponse != null)
            {
                stockResponse.ProductStocks = stockResponse.ProductStocks
                    .Select(s => s)
                    .OrderBy(s => Distance(Double.Parse(s.Shop.Latitude), Double.Parse(s.Shop.Longitude), lat, lng))
                    .Take(5).ToList();
            }
            else
            {
                stockResponse = new StockResponse
                {
                    ProductStocks = new List<ProductStock>(),
                    ProductId = productId
                };
            }

            ProductAttribute fedExProductAttribute = null;
            var productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
            foreach (var pam in productAttributeMappings)
            {
                var pa = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
                if (pa.Name == "FedEx")
                {
                    fedExProductAttribute = pa;
                    break;
                }
            }

            stockResponse.IsFedExMode = _coreSettings.IsFedExMode && fedExProductAttribute == null;

            return PartialView("~/Plugins/Widgets.AbcPickupInStore/Views/SelectStoreForPickup.cshtml", stockResponse);
        }

        [HttpPost]
        public async Task<IActionResult> DisplayClearanceStock(int productId)
        {
            StockResponse stockResponse = await _backendStockService.GetApiStockAsync(productId);
            if (stockResponse != null)
            {
                stockResponse.ProductStocks = stockResponse.ProductStocks
                    .Where(ps => ps.Quantity > 0)
                    .Select(ps => ps).ToList();
            }
            return PartialView("~/Plugins/Widgets.AbcPickupInStore/Views/_ClearanceStoreStock.cshtml", stockResponse);
        }

        private double Distance(double lat1, double lng1, double lat2, double lng2)
        {
            return Math.Pow(Math.Pow(lat1 - lat2, 2) + Math.Pow(lng1 - lng2, 2), 0.5);
        }
    }
}
