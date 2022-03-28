using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Factories;
using Nop.Web.Framework.Components;

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

        public async Task<IViewComponentResult> InvokeAsync(Product product)
        {
            var includedAttributeNames = new string[]
            {
                "Delivery/Pickup Options",
                "Haul Away",
            };

            var models = await _productModelFactory.PrepareProductAttributeModelsAsync(
                product,
                null,
                includedAttributeNames);

            return View("~/Plugins/Misc.AbcFrontend/Views/Product/_ProductAttributes.cshtml", models);
        }
    }
}
