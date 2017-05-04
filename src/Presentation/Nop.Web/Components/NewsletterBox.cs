using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;

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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_customerSettings.HideNewsletterBlock)
                return Content("");

            var model = _newsletterModelFactory.PrepareNewsletterBoxModel();
            return View(model);
        }
    }
}
