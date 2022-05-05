using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.Listrak.Models;
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

namespace AbcWarehouse.Plugin.Widgets.Listrak.Components
{
    public class ListrakViewComponent : NopViewComponent
    {
        private readonly ListrakSettings _settings;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        public ListrakViewComponent(
            ListrakSettings settings,
            ICustomerService customerService,
            ILogger logger,
            IOrderService orderService,
            IPictureService pictureService,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _settings = settings;
            _customerService = customerService;
            _logger = logger;
            _orderService = orderService;
            _pictureService = pictureService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            if (string.IsNullOrWhiteSpace(_settings.MerchantId))
            {
                await _logger.ErrorAsync("Widgets.Listrak: Merchant ID not defined, add in Settings.");
                return Content(string.Empty);
            }

            var model = new ListrakModel()
            {
                MerchantId = _settings.MerchantId,
            };

            var routeData = Url.ActionContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];

            // on cart page
            if (controller.ToString() == "ShoppingCart" && action.ToString() == "Cart")
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(customer);

                model.CartUpdateOrderItems = await GetListrakOrderItemsFromShoppingCart(cart);
            }

            // on order confirmation page
            if (controller.ToString() == "Checkout" && action.ToString() == "Completed")
            {
                var orderId = Convert.ToInt32(routeData.Values["orderId"]);
                var order = await _orderService.GetOrderByIdAsync(orderId);
                var orderItems = await _orderService.GetOrderItemsAsync(orderId);
                var listrakOrderItems = await GetListrakOrderItemsFromOrderItems(orderItems);
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                var address = (await _customerService.GetAddressesByCustomerIdAsync(order.CustomerId)).FirstOrDefault();

                model.PlaceOrderModel = new PlaceOrderModel()
                {
                    CustomerEmail = customer.Email,
                    CustomerFirstName = address.FirstName,
                    CustomerLastName = address.LastName,
                    OrderNumber = order.Id,
                    ItemTotal = order.OrderSubtotalExclTax,
                    ShippingTotal = order.OrderShippingExclTax,
                    TaxTotal = order.OrderTax,
                    OrderTotal = order.OrderTotal,
                    OrderItems = listrakOrderItems,
                    Source = "web"
                };
            }

            // on product details page
            if (controller.ToString() == "Product" && action.ToString() == "ProductDetails")
            {
                var productId = Convert.ToInt32(routeData.Values["productId"]);
                var product = await _productService.GetProductByIdAsync(productId);
                model.ProductBrowseSku = product.Sku;
            }

            return View("~/Plugins/Widgets.Listrak/Views/Framework.cshtml", model);
        }

        private async Task<IList<ListrakOrderItem>> GetListrakOrderItemsFromOrderItems(IList<OrderItem> orderItems)
        {
            var result = new List<ListrakOrderItem>();

            foreach (var oi in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(oi.ProductId);
                result.Add(new ListrakOrderItem()
                {
                    Sku = product.Sku,
                    Quantity = oi.Quantity,
                    Price = oi.UnitPriceExclTax,
                });
            }

            return result;
        }

        private async Task<IList<ListrakOrderItem>> GetListrakOrderItemsFromShoppingCart(IList<ShoppingCartItem> shoppingCartItems)
        {
            var result = new List<ListrakOrderItem>();

            foreach (var sci in shoppingCartItems)
            {
                var product = await _productService.GetProductByIdAsync(sci.ProductId);
                var productPicture = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).FirstOrDefault();
                var imageUrl = productPicture != null ?
                    await _pictureService.GetPictureUrlAsync(productPicture.PictureId) :
                    string.Empty;
                result.Add(new ListrakOrderItem()
                {
                    Sku = product.Sku,
                    Quantity = sci.Quantity,
                    Price = (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice,
                    Title = product.Name,
                    ImageUrl = imageUrl,
                    ProductUrl = Url.RouteUrl(
                        "Product",
                        new { SeName = await _urlRecordService.GetSeNameAsync(product) },
                        _webHelper.GetCurrentRequestProtocol()),
                });
            }

            return result;
        }
    }
}