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
using Microsoft.AspNetCore.Http;

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

        public async Task<IActionResult> GetDeliveryOptions(int? productId, string zip)
        {
            if (zip == null || zip.Length != 5)
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
            var coords = _geocodeService.GeocodeZip(zip);
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
                isDeliveryAvailable = await _deliveryService.CheckZipcodeAsync(zip),
                isFedExAvailable = await HasFedExProductAttributeAsync(productId.Value),
                pickupInStoreHtml = await RenderViewComponentToStringAsync(
                    "CartSlideoutPickupInStore",
                    new {
                        productStock = stockResponse.ProductStocks
                    })
            });
        }

        public async Task<IActionResult> GetEditCartItemInfo(int? shoppingCartItemId)
        {
            if (shoppingCartItemId == null || shoppingCartItemId == 0)
            {
                return BadRequest("Shopping cart item ID must be provided.");
            }

            var customer = await _workContext.GetCurrentCustomerAsync();
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(customer);
            var shoppingCartItem = shoppingCart.FirstOrDefault(sci => sci.Id == shoppingCartItemId);
            var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

            var slideoutInfo = await GetSlideoutInfoAsync(product, shoppingCartItem, 0.0M);

            return Json(new
            {
                slideoutInfo
            }, new JsonSerializerSettings() 
            { 
                NullValueHandling = NullValueHandling.Ignore 
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetAddCartItemInfo(int productId, IFormCollection form)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var product = await _productService.GetProductByIdAsync(productId);

            ShoppingCartItem sci = new ShoppingCartItem()
            {
                CustomerId = customer.Id,
                ProductId = product.Id
            };

            decimal customerEnteredPrice = 0.0M;
            foreach (var formKey in form.Keys)
            {
                if (formKey.Equals($"addtocart_{productId}.CustomerEnteredPrice", StringComparison.InvariantCultureIgnoreCase))
                {
                    decimal.TryParse(form[formKey], out customerEnteredPrice);
                    break;
                }
            }

            var slideoutInfo = await GetSlideoutInfoAsync(product, sci, customerEnteredPrice);

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
            ShoppingCartItem sci,
            decimal customerEnteredPrice)
        {
            var productId = product.Id;

            return new CartSlideoutInfo() {
                ProductInfoHtml = await RenderViewComponentToStringAsync("CartSlideoutProductInfo", new { productId = productId, customerEnteredPrice = customerEnteredPrice } ),
                DeliveryOptionsHtml = await RenderViewComponentToStringAsync(
                    "CartSlideoutProductAttributes",
                    new {
                        product = product,
                        includedAttributeNames = new string[]
                        {
                            AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName,
                            AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName,
                            AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName,
                            "Warranty",
                            AbcDeliveryConsts.DeliveryAccessoriesProductAttributeName,
                            AbcDeliveryConsts.DeliveryInstallAccessoriesProductAttributeName,
                            AbcDeliveryConsts.PickupAccessoriesProductAttributeName,
                        },
                        updateCartItem = sci
                    }),
                ShoppingCartItemId = sci.Id,
                ProductId = productId
            };
        }

        private async Task<bool> HasFedExProductAttributeAsync(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            var pams = await _abcProductAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
            foreach (var pam in pams)
            {
                var pavs = await _abcProductAttributeService.GetProductAttributeValuesAsync(pam.Id);
                foreach (var pav in pavs)
                {
                    if (pav.Name == "FedEx")
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}
