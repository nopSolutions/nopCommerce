using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class VendorNavigationViewComponent : ViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly VendorSettings _vendorSettings;

        public VendorNavigationViewComponent(ICatalogModelFactory catalogModelFactory,
            VendorSettings vendorSettings)
        {
            this._catalogModelFactory = catalogModelFactory;
            this._vendorSettings = vendorSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return Content("");

            var model = _catalogModelFactory.PrepareVendorNavigationModel();
            if (!model.Vendors.Any())
                return Content("");

            return View(model);
        }
    }
}
