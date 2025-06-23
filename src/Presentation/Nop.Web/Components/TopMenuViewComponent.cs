using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class TopMenuViewComponent : NopViewComponent
{
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public TopMenuViewComponent(ICatalogModelFactory catalogModelFactory)
    {
        _catalogModelFactory = catalogModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="productThumbPictureSize">The product thumb picture size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize)
    {
        var model = await _catalogModelFactory.PrepareTopMenuModelAsync();
        return await ViewAsync(model);
    }
}