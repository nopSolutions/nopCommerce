using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

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

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, ShoppingCartItem sci)
        {
            var unitPrice = await _shoppingCartService.GetUnitPriceAsync(sci, false);

            return View("~/Plugins/Widgets.CartSlideout/Views/_Subtotal.cshtml", await _priceFormatter.FormatPriceAsync(unitPrice.unitPrice));
        }
    }
}
