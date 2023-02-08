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
        private readonly IShoppingCartService _shoppingCartService;

        public CartSlideoutSubtotalViewComponent(
            IPriceFormatter priceFormatter,
            IShoppingCartService shoppingCartService)
        {
            _priceFormatter = priceFormatter;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object productOrSci)
        {
            decimal unitPrice = 0M;
            string unitPriceString = string.Empty;
            if (productOrSci is Product)
            {
                var product = productOrSci as Product;
                unitPriceString = await product.IsAddToCartToSeePriceAsync() ?
                    "Add to Cart to See Price" :
                    $"Subtotal: {await _priceFormatter.FormatPriceAsync(product.Price)}";
            }
            else if (productOrSci is ShoppingCartItem)
            {
                unitPrice = (await _shoppingCartService.GetUnitPriceAsync((productOrSci as ShoppingCartItem), false)).unitPrice;
                unitPriceString = $"Subtotal: {await _priceFormatter.FormatPriceAsync(unitPrice)}";
            }
            else
            {
                throw new NopException("CartSlideoutSubtotalViewComponent: Invalid object provided");
            }

            return View("~/Plugins/Widgets.CartSlideout/Views/_Subtotal.cshtml", unitPriceString);
        }
    }
}
