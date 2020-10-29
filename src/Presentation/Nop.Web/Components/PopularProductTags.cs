using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class PopularProductTagsViewComponent : NopViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;

        public PopularProductTagsViewComponent(ICatalogModelFactory catalogModelFactory)
        {
            _catalogModelFactory = catalogModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync();

            if (!model.Tags.Any())
                return Content("");

            return View(model);
        }
    }
}
