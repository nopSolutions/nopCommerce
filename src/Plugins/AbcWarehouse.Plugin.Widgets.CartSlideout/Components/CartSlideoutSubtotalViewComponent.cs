using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Data;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Components
{
    public class CartSlideoutSubtotalViewComponent : NopViewComponent
    {
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        public CartSlideoutSubtotalViewComponent(
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IShoppingCartService shoppingCartService)
        {
            _priceFormatter = priceFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, ShoppingCartItem sci)
        {
            var productId = sci.ProductId;
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new ArgumentException($"CartSlideoutSubtotalViewComponent: Unable to find product with id {productId}");
            }

            if (await product.IsAddToCartToSeePriceAsync())
            {
                return View("~/Plugins/Widgets.CartSlideout/Views/_Subtotal.cshtml", (
                    "Add to Cart to See Price"
                ));
            }

            decimal unitPrice = (await _shoppingCartService.GetUnitPriceAsync(sci, false)).unitPrice;

            string subtotal = $"Subtotal: {await _priceFormatter.FormatPriceAsync(unitPrice)}";

            return View("~/Plugins/Widgets.CartSlideout/Views/_Subtotal.cshtml", (
                subtotal
            ));
        }
    }
}
