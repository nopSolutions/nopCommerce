using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Plugin.Misc.AbcCore.Models;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using Nop.Plugin.Misc.AbcCore.Nop;
using SevenSpikes.Nop.Plugins.StoreLocator.Services;
using Nop.Core.Domain.Orders;
using Newtonsoft.Json;
using Nop.Plugin.Misc.AbcCore.Mattresses;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.AbcCore.Controllers
{
    public class CartSlideoutController : BasePluginController
    {
        private readonly IAbcMattressModelService _abcMattressModelService;
        private readonly IAbcProductAttributeService _abcProductAttributeService;
        private readonly IBackendStockService _backendStockService;
        private readonly IDeliveryService _deliveryService;
        private readonly IGeocodeService _geocodeService;
        private readonly INopDataProvider _nopDataProvider;
        private readonly IProductService _productService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShopService _shopService;
        private readonly IWorkContext _workContext;

        public CartSlideoutController(
            IAbcMattressModelService abcMattressModelService,
            IAbcProductAttributeService abcProductAttributeService,
            IBackendStockService backendStockService,
            IDeliveryService deliveryService,
            IGeocodeService geocodeService,
            INopDataProvider nopDataProvider,
            IProductService productService,
            IProductAttributeParser productAttributeParser,
            IShoppingCartService shoppingCartService,
            IShopService shopService,
            IWorkContext workContext
        ) {
            _abcMattressModelService = abcMattressModelService;
            _abcProductAttributeService = abcProductAttributeService;
            _backendStockService = backendStockService;
            _deliveryService = deliveryService;
            _geocodeService = geocodeService;
            _nopDataProvider = nopDataProvider;
            _productService = productService;
            _productAttributeParser = productAttributeParser;
            _shoppingCartService = shoppingCartService;
            _shopService = shopService;
            _workContext = workContext;
        }

        public async Task<IActionResult> GetDeliveryOptions(int? productId, int? zip)
        {
            if (zip == null || zip.ToString().Length != 5)
            {
                return BadRequest("Zip code must be a 5 digit number provided as a query parameter 'zip'.");
            }

            if (productId == null || productId == 0)
            {
                return BadRequest("Product ID must be provided.");
            }

            // skip gathering everything if mattress
            //var mattressModel = _abcMattressModelService.GetAbcMattressModelByProductId(productId.Value);

            // pickup in store options
            StockResponse stockResponse = await _backendStockService.GetApiStockAsync(productId.Value);
                
            // get 5 closest based on zip code
            var coords = _geocodeService.GeocodeZip(zip.Value);
            if (stockResponse == null)
            {
                stockResponse = new StockResponse();
                stockResponse.ProductStocks = new List<ProductStock>();
            }
            else
            {
                stockResponse.ProductStocks = stockResponse.ProductStocks
                    .Select(s => s)
                    .OrderBy(s => Distance(Double.Parse(s.Shop.Latitude), Double.Parse(s.Shop.Longitude), coords.lat, coords.lng))
                    .Take(5).ToList();
            }
            
            return Json(new {
                isDeliveryAvailable = await _deliveryService.CheckZipcodeAsync(zip.Value),
                pickupInStoreHtml = await RenderViewComponentToStringAsync(
                    "CartSlideoutPickupInStore",
                    new {
                        productStock = stockResponse.ProductStocks
                    })
            });
        }

        [HttpPost]
        public async Task<IActionResult> SelectPickupStore([FromBody]SelectPickupStoreModel model)
        {
            if (model.ShoppingCartItemId == 0)
            {
                return BadRequest("Shopping Cart Item ID is required.");
            }
            if (model.ShopId == 0)
            {
                return BadRequest("Shop ID is required.");
            }

            var customer = await _workContext.GetCurrentCustomerAsync();
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(customer);
            var shoppingCartItem = shoppingCart.FirstOrDefault(sci => sci.Id == model.ShoppingCartItemId);
            if (shoppingCartItem == null)
            {
                return BadRequest($"Unable to find shopping cart item with id {model.ShoppingCartItemId}");
            }

            var pickupPa = await _abcProductAttributeService.GetProductAttributeByNameAsync("Pickup");
            var pickupPam = (await _abcProductAttributeService.GetProductAttributeMappingsByProductIdAsync(
                shoppingCartItem.ProductId)).First(pam => pam.ProductAttributeId == pickupPa.Id);
            var shop = await _shopService.GetShopByIdAsync(model.ShopId);

            shoppingCartItem.AttributesXml = _productAttributeParser.AddProductAttribute(
                shoppingCartItem.AttributesXml,
                pickupPam,
                // will need to add pickupMsg in here
                shop.Name + "\nAvailable: 1 to 3 days"// + pickupMsg
            );
            
            await _shoppingCartService.UpdateShoppingCartItemAsync(
                    customer,
                    shoppingCartItem.Id,
                    shoppingCartItem.AttributesXml,
                    shoppingCartItem.CustomerEnteredPrice,
                    shoppingCartItem.RentalStartDateUtc,
                    shoppingCartItem.RentalEndDateUtc,
                    shoppingCartItem.Quantity);

            return Ok();
        }

        public async Task<IActionResult> GetEditCartItemInfo(int? shoppingCartItemId, int? zip)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(customer);
            var shoppingCartItem = shoppingCart.FirstOrDefault(sci => sci.Id == shoppingCartItemId);
            var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

            var slideoutInfo = await GetSlideoutInfoAsync(product, shoppingCartItem);

            return Json(new
            {
                slideoutInfo
            }, new JsonSerializerSettings() 
            { 
                NullValueHandling = NullValueHandling.Ignore 
            });
        }

        private double Distance(double lat1, double lng1, double lat2, double lng2)
        {
            return Math.Pow(Math.Pow(lat1 - lat2, 2) + Math.Pow(lng1 - lng2, 2), 0.5);
        }

        private async Task<CartSlideoutInfo> GetSlideoutInfoAsync(
            Product product,
            ShoppingCartItem sci)
        {
            return new CartSlideoutInfo() {
                ProductInfoHtml = await RenderViewComponentToStringAsync("CartSlideoutProductInfo", new { productId = product.Id } ),
                SubtotalHtml = await RenderViewComponentToStringAsync("CartSlideoutSubtotal", new { sci = sci } ),
                DeliveryOptionsHtml = await RenderViewComponentToStringAsync(
                    "CartSlideoutProductAttributes",
                    new {
                        product = product,
                        includedAttributeNames = new string[]
                        {
                            AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName,
                            AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName,
                            AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName
                        },
                        updateCartItem = sci
                    }),
                WarrantyHtml = await RenderViewComponentToStringAsync(
                    "CartSlideoutProductAttributes",
                    new {
                        product = product,
                        includedAttributeNames = new string[] { "Warranty" }
                    }),
                ShoppingCartItemId = sci.Id,
                ProductId = sci.ProductId
            };
        }
    }
}
