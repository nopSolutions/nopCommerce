using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class HomepagePollsViewComponent : NopViewComponent
{
    protected readonly IPollModelFactory _pollModelFactory;

    public HomepagePollsViewComponent(IPollModelFactory pollModelFactory)
    {
        _pollModelFactory = pollModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _pollModelFactory.PrepareHomepagePollModelsAsync();
        if (!model.Any())
            return Content("");

        return await ViewAsync(model);
    }
}