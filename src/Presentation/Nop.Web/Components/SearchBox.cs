using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class SearchBoxViewComponent : ViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;

        public SearchBoxViewComponent(ICatalogModelFactory catalogModelFactory)
        {
            this._catalogModelFactory = catalogModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _catalogModelFactory.PrepareSearchBoxModel();
            return View(model);
        }
    }
}
