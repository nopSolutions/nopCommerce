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
using Nop.Web.Models.Checkout;
using AbcWarehouse.Plugin.Widgets.PriceSpider.Models;

namespace AbcWarehouse.Plugin.Widgets.PriceSpider.Components
{
    public class PriceSpiderViewComponent : NopViewComponent
    {
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly PriceSpiderSettings _settings;

        public PriceSpiderViewComponent(
            ILogger logger,
            IOrderService orderService,
            PriceSpiderSettings settings)
        {
            _logger = logger;
            _orderService = orderService;
            _settings = settings;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            if (string.IsNullOrWhiteSpace(_settings.MerchantId))
            {
                await _logger.ErrorAsync("Widgets.PriceSpider: Merchant ID not defined, add in Settings.");
                return Content(string.Empty);
            }

            var checkoutCompletedModel = additionalData as CheckoutCompletedModel;
            var order = await _orderService.GetOrderByIdAsync(checkoutCompletedModel.OrderId);

            // TODO: build pixel model for each product in the order
            var model = new PriceSpiderModel()
            {
                MerchantId = _settings.MerchantId
            };

            return View("~/Plugins/Widgets.PriceSpider/Views/Pixel.cshtml", model);
        }
    }
}