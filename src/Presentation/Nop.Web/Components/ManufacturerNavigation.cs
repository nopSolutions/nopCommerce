using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Web.Components
{
    public class ManufacturerNavigationViewComponent : ViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly CatalogSettings _catalogSettings;

        public ManufacturerNavigationViewComponent(ICatalogModelFactory catalogModelFactory,
            CatalogSettings catalogSettings)
        {
            this._catalogModelFactory = catalogModelFactory;
            this._catalogSettings = catalogSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentManufacturerId)
        {
            if (_catalogSettings.ManufacturersBlockItemsToDisplay == 0)
                return Content("");

            var model = _catalogModelFactory.PrepareManufacturerNavigationModel(currentManufacturerId);
            if (!model.Manufacturers.Any())
                return Content("");

            return View(model);
        }
    }
}
