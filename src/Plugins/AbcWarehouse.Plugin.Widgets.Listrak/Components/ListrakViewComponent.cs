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

            if (widgetZone == PublicWidgetZones.BodyEndHtmlTagBefore)
            {
                return View("~/Plugins/Widgets.Listrak/Views/Framework.cshtml", _settings.MerchantId);
            }

            if (widgetZone == PublicWidgetZones.CheckoutCompletedBottom)
            {
                return await OrderFulfillmentAsync(additionalData as CheckoutCompletedModel);
            }

            if (widgetZone == PublicWidgetZones.ProductDetailsBottom)
            {
                return View("~/Plugins/Widgets.Listrak/Views/ProductBrowse.cshtml", (additionalData as ProductDetailsModel).Sku);
            }

            if (widgetZone == PublicWidgetZones.OrderSummaryCartFooter)
            {
                return await CartUpdateAsync();
            }

            await _logger.ErrorAsync($"Widgets.Listrak: Unsupported widget zone: {widgetZone}.");
            return Content(string.Empty);
        }

        private async Task<IViewComponentResult> CartUpdateAsync()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer);
            var listrakOrderItems = await GetListrakOrderItemsFromShoppingCart(cart);

            return View("~/Plugins/Widgets.Listrak/Views/CartUpdate.cshtml", listrakOrderItems);
        }

        private async Task<IViewComponentResult> OrderFulfillmentAsync(CheckoutCompletedModel checkoutCompletedModel)
        {
            var order = await _orderService.GetOrderByIdAsync(checkoutCompletedModel.OrderId);
            var orderItems = await _orderService.GetOrderItemsAsync(checkoutCompletedModel.OrderId);
            var listrakOrderItems = await GetListrakOrderItemsFromOrderItems(orderItems);
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            var address = (await _customerService.GetAddressesByCustomerIdAsync(order.CustomerId)).FirstOrDefault();

            var model = new PlaceOrderModel()
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
                Source = "web",
                MerchantId = _settings.MerchantId,
            };

            return View("~/Plugins/Widgets.Listrak/Views/PlaceOrder.cshtml", model);
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