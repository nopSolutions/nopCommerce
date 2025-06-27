using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.FilterLevels;
using Nop.Services.FilterLevels;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class CompatibleWithFilterLevelValuesViewComponent : NopViewComponent
{
    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly IFilterLevelValueModelFactory _filterLevelValueModelFactory;
    protected readonly IFilterLevelValueService _filterLevelValueService;

    public CompatibleWithFilterLevelValuesViewComponent(FilterLevelSettings filterLevelSettings,
        IFilterLevelValueModelFactory filterLevelValueModelFactory,
        IFilterLevelValueService filterLevelValueService)
    {
        _filterLevelSettings = filterLevelSettings;
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
        _filterLevelValueService = filterLevelValueService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int productId)
    {
        if (_filterLevelSettings.FilterLevelEnabled && _filterLevelSettings.DisplayOnProductDetailsPage)
        {
            //get filter level values
            var filterLevelValues = await _filterLevelValueService
                .GetFilterLevelValuesByProductIdAsync(productId: productId);            

            var model = _filterLevelValueModelFactory.PrepareFilterLevelValueOverviewModel(filterLevelValues);

            return View(model);
        }

        return Content(string.Empty);
    }
}
