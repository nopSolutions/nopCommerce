using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class TaxTypeSelectorViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly TaxSettings _taxSettings;

        public TaxTypeSelectorViewComponent(ICommonModelFactory commonModelFactory,
            TaxSettings taxSettings)
        {
            this._commonModelFactory = commonModelFactory;
            this._taxSettings = taxSettings;
        }

        public IViewComponentResult Invoke()
        {
            if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
                return Content("");

            var model = _commonModelFactory.PrepareTaxTypeSelectorModel();
            return View(model);
        }
    }
}
