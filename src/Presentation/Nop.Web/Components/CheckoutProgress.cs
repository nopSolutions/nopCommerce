using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
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

        public IViewComponentResult Invoke(CheckoutProgressStep step)
        {
            var model = _checkoutModelFactory.PrepareCheckoutProgressModel(step);
            return View(model);
        }
    }
}
