using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class PollBlockViewComponent : NopViewComponent
{
    protected readonly IPollModelFactory _pollModelFactory;

    public PollBlockViewComponent(IPollModelFactory pollModelFactory)
    {
        _pollModelFactory = pollModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="systemKeyword">System keyword</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string systemKeyword)
    {

        if (string.IsNullOrWhiteSpace(systemKeyword))
            return Content("");

        var model = await _pollModelFactory.PreparePollModelBySystemNameAsync(systemKeyword);
        if (model == null)
            return Content("");

        return await ViewAsync(model);
    }
}