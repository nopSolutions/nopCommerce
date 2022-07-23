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

namespace Nop.Plugin.Misc.AbcCore.Controllers
{
    public class CartSlideoutController : BasePluginController
    {
        private readonly IBackendStockService _backendStockService;
        private readonly IDeliveryService _deliveryService;
        private readonly IGeocodeService _geocodeService;
        private readonly INopDataProvider _nopDataProvider;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;

        public CartSlideoutController(
            IBackendStockService backendStockService,
            IDeliveryService deliveryService,
            IGeocodeService geocodeService,
            INopDataProvider nopDataProvider,
            IProductAttributeParser productAttributeParser,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext
        ) {
            _backendStockService = backendStockService;
            _deliveryService = deliveryService;
            _geocodeService = geocodeService;
            _nopDataProvider = nopDataProvider;
            _productAttributeParser = productAttributeParser;
            _shoppingCartService = shoppingCartService;
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

            // pickup in store options
            StockResponse stockResponse = await _backendStockService.GetApiStockAsync(productId.Value);
            
            // get 5 closest based on zip code
            var coords = _geocodeService.GeocodeZip(zip.Value);
            stockResponse.ProductStocks = stockResponse.ProductStocks
                    .Select(s => s)
                    .OrderBy(s => Distance(Double.Parse(s.Shop.Latitude), Double.Parse(s.Shop.Longitude), coords.lat, coords.lng))
                    .Take(5).ToList();

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
        public async Task<IActionResult> SelectPickupStore(int? shoppingCartItemId, int? shopId)
        {
            // Just for now, let's make the connection
            return Ok();

            if (!shoppingCartItemId.HasValue)
            {
                return BadRequest("Shopping Cart Item ID is required.");
            }
            if (!shopId.HasValue)
            {
                return BadRequest("Shop ID is required.");
            }

            var customer = await _workContext.GetCurrentCustomerAsync();
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(customer);
            var shoppingCartItem = shoppingCart.FirstOrDefault(sci => sci.Id == shoppingCartItemId);
            if (shoppingCartItem == null)
            {
                return BadRequest($"Unable to find shopping cart item with id {shoppingCartItemId}");
            }

            shoppingCartItem.AttributesXml = _productAttributeParser.AddProductAttribute(
                shoppingCartItem.AttributesXml,
                new ProductAttributeMapping(), // need to get the pickup attribute from product
                "West Bloomfield" // Need to get the shop name
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

        private double Distance(double lat1, double lng1, double lat2, double lng2)
        {
            return Math.Pow(Math.Pow(lat1 - lat2, 2) + Math.Pow(lng1 - lng2, 2), 0.5);
        }
    }
}
