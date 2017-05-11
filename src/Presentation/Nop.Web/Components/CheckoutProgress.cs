using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Threading.Tasks;
using Nop.Web.Models.Checkout;

namespace Nop.Web.Components
{
    public class CheckoutProgressViewComponent : ViewComponent
    {
        private readonly ICheckoutModelFactory _checkoutModelFactory;

        public CheckoutProgressViewComponent(ICheckoutModelFactory checkoutModelFactory)
        {
            this._checkoutModelFactory = checkoutModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(CheckoutProgressStep step)
        {
            var model = _checkoutModelFactory.PrepareCheckoutProgressModel(step);
            return View(model);
        }
    }
}
