using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Components;

public partial class WidgetViewComponent : NopViewComponent
{
    protected readonly IWidgetModelFactory _widgetModelFactory;

    public WidgetViewComponent(IWidgetModelFactory widgetModelFactory)
    {
        _widgetModelFactory = widgetModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
    {
        var model = await _widgetModelFactory.PrepareRenderWidgetModelAsync(widgetZone, additionalData);

        //no data?
        if (!model.Any())
            return Content("");

        return await ViewAsync(model);
    }
}