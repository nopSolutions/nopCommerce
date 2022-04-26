using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.Listrak.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Checkout;

namespace AbcWarehouse.Plugin.Widgets.Listrak.Components
{
    public class ListrakViewComponent : NopViewComponent
    {
        private readonly ListrakSettings _settings;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public ListrakViewComponent(
            ListrakSettings settings,
            ICustomerService customerService,
            ILogger logger,
            IOrderService orderService,
            IProductService productService)
        {
            _settings = settings;
            _customerService = customerService;
            _logger = logger;
            _orderService = orderService;
            _productService = productService;
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

            await _logger.ErrorAsync($"Widgets.Listrak: Unsupported widget zone: {widgetZone}.");
            return Content(string.Empty);
        }

        private async Task<IViewComponentResult> OrderFulfillmentAsync(CheckoutCompletedModel checkoutCompletedModel)
        {
            var order = await _orderService.GetOrderByIdAsync(checkoutCompletedModel.OrderId);
            var orderItems = await _orderService.GetOrderItemsAsync(checkoutCompletedModel.OrderId);
            var listrakOrderItems = await GetListrakOrderItems(orderItems);
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
                MerchantId = _settings.MerchantId,
            };

            return View("~/Plugins/Widgets.Listrak/Views/PlaceOrder.cshtml", model);
        }

        private async Task<IList<ListrakOrderItem>> GetListrakOrderItems(IList<OrderItem> orderItems)
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
    }
}