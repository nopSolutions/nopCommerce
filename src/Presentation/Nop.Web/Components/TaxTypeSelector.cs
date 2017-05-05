using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Threading.Tasks;
using Nop.Core.Domain.Tax;

namespace Nop.Web.Components
{
    public class TaxTypeSelectorViewComponent : ViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly TaxSettings _taxSettings;

        public TaxTypeSelectorViewComponent(ICommonModelFactory commonModelFactory,
            TaxSettings taxSettings)
        {
            this._commonModelFactory = commonModelFactory;
            this._taxSettings = taxSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
                return Content("");

            var model = _commonModelFactory.PrepareTaxTypeSelectorModel();
            return View(model);
        }
    }
}
