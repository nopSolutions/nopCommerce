using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class NewsLetterBoxViewComponent : NopViewComponent
{
    protected readonly CustomerSettings _customerSettings;
    protected readonly INewsLetterModelFactory _newsLetterModelFactory;

    public NewsLetterBoxViewComponent(CustomerSettings customerSettings, INewsLetterModelFactory newsLetterModelFactory)
    {
        _customerSettings = customerSettings;
        _newsLetterModelFactory = newsLetterModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (_customerSettings.HideNewsletterBlock)
            return Content("");

        var model = await _newsLetterModelFactory.PrepareNewsLetterBoxModelAsync();
        return View(model);
    }
}