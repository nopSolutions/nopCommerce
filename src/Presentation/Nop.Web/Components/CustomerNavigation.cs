using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class CustomerNavigationViewComponent : ViewComponent
    {
        private readonly ICustomerModelFactory _customerModelFactory;

        public CustomerNavigationViewComponent(ICustomerModelFactory customerModelFactory)
        {
            this._customerModelFactory = customerModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int selectedTabId = 0)
        {
            var model = _customerModelFactory.PrepareCustomerNavigationModel(selectedTabId);
            return View(model);
        }
    }
}
