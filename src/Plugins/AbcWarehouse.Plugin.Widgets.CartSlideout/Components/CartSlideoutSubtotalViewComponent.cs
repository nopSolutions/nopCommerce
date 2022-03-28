using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Web.Framework.Components;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Components
{
    public class CartSlideoutSubtotalViewComponent : NopViewComponent
    {
        private readonly IPriceFormatter _priceFormatter;

        public CartSlideoutSubtotalViewComponent(
            IPriceFormatter priceFormatter)
        {
            _priceFormatter = priceFormatter;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, decimal price)
        {
            return View("~/Plugins/Widgets.CartSlideout/Views/_Subtotal.cshtml", await _priceFormatter.FormatPriceAsync(price));
        }
    }
}
