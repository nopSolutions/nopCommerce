using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Areas.Admin.Factories;

namespace Nop.Web.Areas.Admin.Components;

public partial class MultistorePreviewViewComponent : NopViewComponent
{
    private readonly ICommonModelFactory _commonModelFactory;

    public MultistorePreviewViewComponent(ICommonModelFactory commonModelFactory)
    {
        _commonModelFactory = commonModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync(object model)
    {
        var multistorePreviewModels = await _commonModelFactory.PrepareMultistorePreviewModelsAsync(model);

        return View(multistorePreviewModels);
    }
}
