using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ManufacturerNavigationViewComponent : NopViewComponent
{
    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public ManufacturerNavigationViewComponent(CatalogSettings catalogSettings, ICatalogModelFactory catalogModelFactory)
    {
        _catalogSettings = catalogSettings;
        _catalogModelFactory = catalogModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="currentManufacturerId">The current manufacturer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(int currentManufacturerId)
    {
        if (_catalogSettings.ManufacturersBlockItemsToDisplay == 0)
            return Content("");

        var model = await _catalogModelFactory.PrepareManufacturerNavigationModelAsync(currentManufacturerId);
        if (!model.Manufacturers.Any())
            return Content("");

        return await ViewAsync(model);
    }
}