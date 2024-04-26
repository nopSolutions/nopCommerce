using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class NewsletterBoxViewComponent : NopViewComponent
{
    protected readonly CustomerSettings _customerSettings;
    protected readonly INewsletterModelFactory _newsletterModelFactory;

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