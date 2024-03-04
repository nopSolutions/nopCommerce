using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.GA4.Models;
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
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Checkout;
using Nop.Plugin.Misc.AbcCore.Infrastructure;

namespace AbcWarehouse.Plugin.Widgets.GA4.Components
{
    public class GA4ViewComponent : NopViewComponent
    {
        private readonly GA4Settings _settings;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly IManufacturerService _manufacturerService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public GA4ViewComponent(
            GA4Settings settings,
            ICategoryService categoryService,
            ICustomerService customerService,
            ILogger logger,
            IManufacturerService manufacturerService,
            IOrderService orderService,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _settings = settings;
            _categoryService = categoryService;
            _customerService = customerService;
            _logger = logger;
            _manufacturerService = manufacturerService;
            _orderService = orderService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            if (string.IsNullOrWhiteSpace(_settings.GoogleTag))
            {
                await _logger.ErrorAsync("Widgets.GA4: Google tag not defined, add in Settings.");
                return Content(string.Empty);
            }

            if (widgetZone == CustomPublicWidgetZones.ProductBoxAfter ||
                widgetZone == PublicWidgetZones.ProductDetailsAddInfo)
            {
                var productId = widgetZone == CustomPublicWidgetZones.ProductBoxAfter ?
                    (additionalData as ProductOverviewModel).Id :
                    Convert.ToInt32(Url.ActionContext.RouteData.Values["productId"]);
                var product = await _productService.GetProductByIdAsync(productId);
                var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(productId)).FirstOrDefault();
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(productManufacturer?.ManufacturerId ?? 0);
                var productCategories = (await _categoryService.GetProductCategoriesByProductIdAsync(productId)).FirstOrDefault();
                var category = await _categoryService.GetCategoryByIdAsync(productCategories?.CategoryId ?? 0);
                var addToCartModel = new AddToCartModel()
                {
                    Id = productId,
                    ButtonId = widgetZone == CustomPublicWidgetZones.ProductBoxAfter ?
                        $"product-box-add-to-cart-button-{productId}" :
                        $"add-to-cart-button-{productId}",
                    Item = new GA4OrderItem()
                    {
                        Sku = product.Sku,
                        Name = product.Name,
                        Brand = manufacturer?.Name,
                        Category = category?.Name,
                        Price = product.Price,
                    }
                };

                return View(
                    "~/Plugins/Widgets.GA4/Views/AddToCart.cshtml",
                    addToCartModel);
            }

            if (widgetZone == PublicWidgetZones.CheckoutShippingAddressBottom)
            {
                var cart = await _shoppingCartService.GetCurrentShoppingCartAsync();
                var value = 0M;
                foreach (var sci in cart)
                {
                    value += (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice;
                }
                var ga4OrderItems = await GetGA4OrderItemsFromShoppingCart(cart);

                var addShippingInfoModel = new GA4GeneralModel()
                {
                    Value = value,
                    Items = ga4OrderItems
                };

                return View(
                    "~/Plugins/Widgets.GA4/Views/AddShippingInfo.cshtml",
                    addShippingInfoModel);
            }

            // Start full tag processing
            var model = new GA4Model() {
                GoogleTag = _settings.GoogleTag,
                IsDebugMode = _settings.IsDebugMode
            };

            if (IsPDP())
            {
                var productId = Convert.ToInt32(Url.ActionContext.RouteData.Values["productId"]);
                var product = await _productService.GetProductByIdAsync(productId);
                var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(productId)).FirstOrDefault();
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(productManufacturer?.ManufacturerId ?? 0);
                var productCategories = (await _categoryService.GetProductCategoriesByProductIdAsync(productId)).FirstOrDefault();
                var category = await _categoryService.GetCategoryByIdAsync(productCategories?.CategoryId ?? 0);

                model.ViewItemModel = new GA4OrderItem()
                {
                    Sku = product.Sku,
                    Name = product.Name,
                    Brand = manufacturer?.Name,
                    Category = category?.Name,
                    Price = product.Price,
                };
            }

            if (IsBeginCheckout())
            {
                var cart = await _shoppingCartService.GetCurrentShoppingCartAsync();
                var value = 0M;
                foreach (var sci in cart)
                {
                    value += (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice;
                }
                var ga4OrderItems = await GetGA4OrderItemsFromShoppingCart(cart);

                model.BeginCheckoutModel = new GA4GeneralModel()
                {
                    Value = value,
                    Items = ga4OrderItems
                };
            }

            if (IsPurchase())
            {
                var orderId = Convert.ToInt32(Url.ActionContext.RouteData.Values["orderId"]);
                var order = await _orderService.GetOrderByIdAsync(orderId);
                var orderItems = await _orderService.GetOrderItemsAsync(orderId);
                var ga4OrderItems = await GetGA4OrderItemsFromOrderItems(orderItems);
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                var address = (await _customerService.GetAddressesByCustomerIdAsync(order.CustomerId)).FirstOrDefault();

                model.PurchaseModel = new PurchaseModel()
                {
                    OrderId = order.Id.ToString(),
                    Value = order.OrderTotal,
                    Shipping = order.OrderShippingExclTax,
                    Tax = order.OrderTax,
                    OrderItems = ga4OrderItems
                };
            }

            if (IsViewCart())
            {
                var cart = await _shoppingCartService.GetCurrentShoppingCartAsync();
                var value = 0M;
                foreach (var sci in cart)
                {
                    value += (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice;
                }
                var ga4OrderItems = await GetGA4OrderItemsFromShoppingCart(cart);

                model.ViewCartModel = new GA4GeneralModel()
                {
                    Value = value,
                    Items = ga4OrderItems
                };
            }

            return View("~/Plugins/Widgets.GA4/Views/Tag.cshtml", model);
        }

        private bool IsPurchase()
        {
            var routeData = Url.ActionContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            return controller.Equals("Checkout") && action.Equals("Completed");
        }

        private bool IsBeginCheckout()
        {
            var routeData = Url.ActionContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            return controller.Equals("Checkout") && action.Equals("BillingAddress");
        }

        private bool IsPDP()
        {
            var routeData = Url.ActionContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            return controller.Equals("Product") && action.Equals("ProductDetails");
        }

        private bool IsViewCart()
        {
            var routeData = Url.ActionContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            return controller.Equals("ShoppingCart") && action.Equals("Cart");
        }

        private async Task<IList<GA4OrderItem>> GetGA4OrderItemsFromShoppingCart(IList<ShoppingCartItem> shoppingCartItems)
        {
            var result = new List<GA4OrderItem>();

            foreach (var sci in shoppingCartItems)
            {
                var product = await _productService.GetProductByIdAsync(sci.ProductId);
                var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(sci.ProductId)).FirstOrDefault();
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(productManufacturer?.ManufacturerId ?? 0);
                var productCategories = (await _categoryService.GetProductCategoriesByProductIdAsync(sci.ProductId)).FirstOrDefault();
                var category = await _categoryService.GetCategoryByIdAsync(productCategories?.CategoryId ?? 0);
                result.Add(new GA4OrderItem()
                {
                    Sku = product.Sku,
                    Name = product.Name,
                    Brand = manufacturer?.Name,
                    Category = category?.Name,
                    Quantity = sci.Quantity,
                    Price = (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice
                });
            }

            return result;
        }

        private async Task<IList<GA4OrderItem>> GetGA4OrderItemsFromOrderItems(IList<OrderItem> orderItems)
        {
            var result = new List<GA4OrderItem>();

            foreach (var oi in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(oi.ProductId);
                var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(oi.ProductId)).FirstOrDefault();
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(productManufacturer?.ManufacturerId ?? 0);
                var productCategories = (await _categoryService.GetProductCategoriesByProductIdAsync(oi.ProductId)).FirstOrDefault();
                var category = await _categoryService.GetCategoryByIdAsync(productCategories?.CategoryId ?? 0);
                result.Add(new GA4OrderItem()
                {
                    Sku = product.Sku,
                    Name = product.Name,
                    Brand = manufacturer?.Name,
                    Category = category?.Name,
                    Quantity = oi.Quantity,
                    Price = oi.UnitPriceExclTax,
                });
            }

            return result;
        }
    }
}