using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class CategoryNavigationViewComponent : NopViewComponent
{
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public CategoryNavigationViewComponent(ICatalogModelFactory catalogModelFactory)
    {
        _catalogModelFactory = catalogModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="currentCategoryId">The current category identifier</param>
    /// <param name="currentProductId">The current product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
    {
        var model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(currentCategoryId, currentProductId);
        return await ViewAsync(model);
    }
}