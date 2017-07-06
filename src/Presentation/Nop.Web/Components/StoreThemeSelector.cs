using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class StoreThemeSelectorViewComponent : ViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly StoreInformationSettings _storeInformationSettings;

        public StoreThemeSelectorViewComponent(ICommonModelFactory commonModelFactory,
            StoreInformationSettings storeInformationSettings)
        {
            this._commonModelFactory = commonModelFactory;
            this._storeInformationSettings = storeInformationSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_storeInformationSettings.AllowCustomerToSelectTheme)
                return Content("");

            var model = _commonModelFactory.PrepareStoreThemeSelectorModel();
            return View(model);
        }
    }
}
