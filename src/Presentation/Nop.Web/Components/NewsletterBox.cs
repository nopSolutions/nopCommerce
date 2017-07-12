using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class NewsletterBoxViewComponent : ViewComponent
    {
        private readonly INewsletterModelFactory _newsletterModelFactory;

        private readonly CustomerSettings _customerSettings;

        public NewsletterBoxViewComponent(INewsletterModelFactory newsletterModelFactory,
            CustomerSettings customerSettings)
        {
            this._newsletterModelFactory = newsletterModelFactory;
            this._customerSettings = customerSettings;
        }

        public IViewComponentResult Invoke()
        {
            if (_customerSettings.HideNewsletterBlock)
                return Content("");

            var model = _newsletterModelFactory.PrepareNewsletterBoxModel();
            return View(model);
        }
    }
}
