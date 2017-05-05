using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class CurrencySelectorViewComponent : ViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public CurrencySelectorViewComponent(ICommonModelFactory commonModelFactory)
        {
            this._commonModelFactory = commonModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _commonModelFactory.PrepareCurrencySelectorModel();
            if (model.AvailableCurrencies.Count == 1)
                Content("");

            return View(model);
        }
    }
}
