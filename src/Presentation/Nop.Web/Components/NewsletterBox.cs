using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class NewsletterBoxViewComponent : NopViewComponent
    {
        private readonly CustomerSettings _customerSettings;
        private readonly INewsletterModelFactory _newsletterModelFactory;

        public NewsletterBoxViewComponent(CustomerSettings customerSettings, INewsletterModelFactory newsletterModelFactory)
        {
            _customerSettings = customerSettings;
            _newsletterModelFactory = newsletterModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_customerSettings.HideNewsletterBlock)
                return Content("");

            var model = await _newsletterModelFactory.PrepareNewsletterBoxModelAsync();
            return View(model);
        }
    }
}
