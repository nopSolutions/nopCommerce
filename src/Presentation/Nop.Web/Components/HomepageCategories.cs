using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class HomepageCategoriesViewComponent : ViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;

        public HomepageCategoriesViewComponent(ICatalogModelFactory catalogModelFactory)
        {
            this._catalogModelFactory = catalogModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _catalogModelFactory.PrepareHomepageCategoryModels();
            if (!model.Any())
                return Content("");

            return View(model);
        }
    }
}
