using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class CategoryNavigationViewComponent : ViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;

        public CategoryNavigationViewComponent(ICatalogModelFactory catalogModelFactory)
        {
            this._catalogModelFactory = catalogModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
        {
            var model = _catalogModelFactory.PrepareCategoryNavigationModel(currentCategoryId, currentProductId);
            return View(model);
        }
    }
}
