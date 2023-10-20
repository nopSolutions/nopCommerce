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
        private readonly IManufacturerService _manufacturerService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly PriceSpiderSettings _settings;

        public PriceSpiderViewComponent(
            ILogger logger,
            IManufacturerService manufacturerService,
            IOrderService orderService,
            IProductService productService,
            PriceSpiderSettings settings)
        {
            _logger = logger;
            _manufacturerService = manufacturerService;
            _orderService = orderService;
            _productService = productService;
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
            var orderItems = await _orderService.GetOrderItemsAsync(checkoutCompletedModel.OrderId);
            var priceSpiderProductsModels = new List<PriceSpiderProductModel>();

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id)).FirstOrDefault();
                var manufacturerName = productManufacturer != null ?
                    (await _manufacturerService.GetManufacturerByIdAsync(productManufacturer.ManufacturerId)).Name :
                    string.Empty;
                var priceSpiderProductModel = new PriceSpiderProductModel()
                {
                    Gtin = product.Gtin,
                    Price = decimal.Round(orderItem.UnitPriceExclTax, 2, MidpointRounding.AwayFromZero),
                    Quantity = orderItem.Quantity,
                    BrandName = manufacturerName,
                    ProductName = product.Name,
                    Sku = product.Sku
                };
                priceSpiderProductsModels.Add(priceSpiderProductModel);
            }

            // TODO: build pixel model for each product in the order
            var model = new PriceSpiderModel()
            {
                MerchantId = _settings.MerchantId,
                Products = priceSpiderProductsModels
            };

            return View("~/Plugins/Widgets.PriceSpider/Views/Pixel.cshtml", model);
        }
    }
}