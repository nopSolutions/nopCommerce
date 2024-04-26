using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class LogoViewComponent : NopViewComponent
{
    protected readonly ICommonModelFactory _commonModelFactory;

    public LogoViewComponent(ICommonModelFactory commonModelFactory)
    {
        _commonModelFactory = commonModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _commonModelFactory.PrepareLogoModelAsync();
        return View(model);
    }
}