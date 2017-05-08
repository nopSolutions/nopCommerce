using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class PopularProductTagsViewComponent : ViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;

        public PopularProductTagsViewComponent(ICatalogModelFactory catalogModelFactory)
        {
            this._catalogModelFactory = catalogModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _catalogModelFactory.PreparePopularProductTagsModel();

            if (!model.Tags.Any())
                return Content("");

            return View(model);
        }
    }
}
