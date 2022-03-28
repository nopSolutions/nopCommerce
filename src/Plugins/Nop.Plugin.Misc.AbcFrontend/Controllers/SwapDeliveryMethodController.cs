using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Web.Framework.Controllers;
using System.Linq;
using SevenSpikes.Nop.Plugins.StoreLocator.Services;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Models;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcFrontend.Controllers
{
    public class SwapDeliveryMethodController : BasePluginController
    {
        IShoppingCartService _shoppingCartService;
        ICustomerShopService _customerShopService;
        IProductService _productService;
        IBackendStockService _backendStockService;
        IShopService _shopService;
        IRepository<ShoppingCartItem> _shoppingCartItemRepository;
        IRepository<ProductHomeDelivery> _productHomeDeliveryRepository;
        IAttributeUtilities _attributeUtilities;
        IWorkContext _workContext;
        IUrlRecordService _urlRecordService;
        IStoreContext _storeContext;

        public SwapDeliveryMethodController(
            IShoppingCartService shoppingCartService,
            ICustomerShopService customerShopService,
            IProductService productService,
            IBackendStockService backendStockService,
            IShopService shopService,
            IRepository<ShoppingCartItem> shoppingCartItemRepository,
            IRepository<ProductHomeDelivery> productHomeDeliveryRepository,
            IAttributeUtilities attributeUtilities,
            IWorkContext workContext,
            IUrlRecordService urlRecordService,
            IStoreContext storeContext
        )
        {
            _shoppingCartService = shoppingCartService;
            _customerShopService = customerShopService;
            _productService = productService;
            _backendStockService = backendStockService;
            _shopService = shopService;
            _shoppingCartItemRepository = shoppingCartItemRepository;
            _productHomeDeliveryRepository = productHomeDeliveryRepository;
            _attributeUtilities = attributeUtilities;
            _workContext = workContext;
            _urlRecordService = urlRecordService;
            _storeContext = storeContext;
        }

        [HttpPost]
        public async Task<IActionResult> Change(int shoppingCartItemId)
        {
            ShoppingCartItem item = _shoppingCartItemRepository.Table
                .Where(i => i.Id == shoppingCartItemId)
                .Select(i => i).FirstOrDefault();

            CustomerShopMapping csm = _customerShopService.GetCurrentCustomerShopMapping((await _workContext.GetCurrentCustomerAsync()).Id);

            // no store selected yet
            if (csm == null)
            {
                // kick back to product page
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                var seName = await _urlRecordService.GetSeNameAsync<Product>(product);
                string url = Url.RouteUrl("Product", new { SeName = seName });
                url += "?updatecartitemid=" + item.Id;
                return Json(new FailResponse
                {
                    RedirectUrl = url
                });
            }

            Shop currentShop = await _shopService.GetShopByIdAsync(csm.ShopId);

            bool isHomeDeliveryProduct = _productHomeDeliveryRepository.Table
                .Where(hdp => hdp.Product_Id == item.ProductId)
                .Select(hdp => hdp).Any();

            string attributes = item.AttributesXml;

            // get xml and check if it's home delivery or pickup in store
            var pickupAttributeMapping = await _attributeUtilities.GetPickupAttributeMappingAsync(attributes);

            // is currently pickup in store 
            if (pickupAttributeMapping != null)
            {
                if (isHomeDeliveryProduct)
                {
                    // replace pickup with home delivery if the item is a home delivery product
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    attributes = await _attributeUtilities.InsertHomeDeliveryAttributeAsync(product, attributes);
                }
                else
                {
                    // remove pickup attributemapping
                    attributes = await _attributeUtilities.RemovePickupAttributesAsync(attributes);
                }
            }
            // is currently home delivery
            else
            {
                // check if this item can be picked up at the current store
                StockResponse response = await _backendStockService.GetApiStockAsync(item.ProductId);
                bool itemAvailableAtStore = response.ProductStocks
                    .Where(p => p.Shop.Id == csm.ShopId && p.Available).Any();

                if (itemAvailableAtStore)
                {
                    // replace home delivery/shipping with pickup
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    attributes = await _attributeUtilities.InsertPickupAttributeAsync(product, response, attributes);
                }
                else
                {
                    // item is not available at the store. do not change, then display a message to customer
                    return Json(new FailResponse
                    {
                        ErrorMessageRecipient = "#change_method_error_" + shoppingCartItemId,
                        ErrorMessage = "This item is not avaialble for pickup at " + currentShop.Name
                    });
                }
            }

            var cart = await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);
            ShoppingCartItem sameCartItem = cart
                .Where(sci => sci.Id != item.Id && sci.ProductId == item.ProductId && sci.AttributesXml == attributes)
                .Select(sci => sci).FirstOrDefault();
            if (sameCartItem == null)
            {
                await _shoppingCartService.UpdateShoppingCartItemAsync(await _workContext.GetCurrentCustomerAsync(), item.Id, attributes, item.CustomerEnteredPrice, null, null, item.Quantity, true);
            }
            else
            {
                await _shoppingCartService.UpdateShoppingCartItemAsync(await _workContext.GetCurrentCustomerAsync(), sameCartItem.Id,
                    sameCartItem.AttributesXml, sameCartItem.CustomerEnteredPrice, null, null, item.Quantity + sameCartItem.Quantity, true);
                await _shoppingCartService.DeleteShoppingCartItemAsync(item);
            }

            return new EmptyResult();
        }

        private class FailResponse
        {
            public string RedirectUrl { get; set; }
            public string ErrorMessageRecipient { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
