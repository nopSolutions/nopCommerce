using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public partial class TaxTypeSelectorViewComponent : NopViewComponent
    {
        protected readonly ICommonModelFactory _commonModelFactory;
        protected readonly TaxSettings _taxSettings;

        public TaxTypeSelectorViewComponent(ICommonModelFactory commonModelFactory,
            TaxSettings taxSettings)
        {
            _commonModelFactory = commonModelFactory;
            _taxSettings = taxSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
                return Content("");

            var model = await _commonModelFactory.PrepareTaxTypeSelectorModelAsync();
            return View(model);
        }
    }
}
