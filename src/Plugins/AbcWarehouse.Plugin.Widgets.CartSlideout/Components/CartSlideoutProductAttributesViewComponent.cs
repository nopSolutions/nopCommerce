using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Factories;
using Nop.Web.Framework.Components;
using Nop.Core.Domain.Orders;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Components
{
    public class CartSlideoutProductAttributesViewComponent : NopViewComponent
    {
        private readonly IAbcProductModelFactory _productModelFactory;

        public CartSlideoutProductAttributesViewComponent(
            IAbcProductModelFactory productModelFactory)
        {
            _productModelFactory = productModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(Product product, string[] includedAttributeNames, ShoppingCartItem updateCartItem = null)
        {
            var models = await _productModelFactory.PrepareProductAttributeModelsAsync(
                product,
                updateCartItem,
                includedAttributeNames);

            // This will only work for Pavilion theme, will need to be adjusted for Pacific
            // once Mickey Shorr is written
            // More ideally, we won't need to modify _ProductAttributes.cshtml
            // Could also just place this in AbcFrontend/Core?
            return View("~/Themes/Pavilion/Views/Product/_ProductAttributes.cshtml", models);
        }
    }
}
