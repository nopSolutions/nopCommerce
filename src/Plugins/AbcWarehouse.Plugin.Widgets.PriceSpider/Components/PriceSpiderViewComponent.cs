using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace AbcWarehouse.Plugin.Widgets.PriceSpider.Components
{
    public class PriceSpiderViewComponent : NopViewComponent
    {
        private readonly IOrderService _orderService;

        public PriceSpiderViewComponent(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            // TODO: check for merchant ID

            var checkoutCompletedModel = additionalData as CheckoutCompletedModel;
            var order = await _orderService.GetOrderByIdAsync(checkoutCompletedModel.OrderId);

            // TODO: build pixel model for each product in the order

            return View("~/Plugins/Widgets.PriceSpider/Views/Pixel.cshtml");
        }
    }
}