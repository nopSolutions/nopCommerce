using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Extensions;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Components
{
    public class CartSlideoutSubtotalViewComponent : NopViewComponent
    {
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        public CartSlideoutSubtotalViewComponent(
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IShoppingCartService shoppingCartService)
        {
            _priceFormatter = priceFormatter;
            _productAttributeParser = productAttributeParser;
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

            decimal unitPrice = (await _shoppingCartService.GetUnitPriceAsync(sci, false)).unitPrice;
            string unitPriceString = await product.IsAddToCartToSeePriceAsync() ?
                    "Add to Cart to See Price" :
                    $"Subtotal: {await _priceFormatter.FormatPriceAsync(unitPrice)}";

            return View("~/Plugins/Widgets.CartSlideout/Views/_Subtotal.cshtml", unitPriceString);
        }
    }
}
