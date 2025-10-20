using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.FilterLevels;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Components;

public partial class FilterLevelValueSearchViewComponent : NopViewComponent
{
    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly IFilterLevelValueModelFactory _filterLevelValueModelFactory;

    public FilterLevelValueSearchViewComponent(FilterLevelSettings filterLevelSettings,
        IFilterLevelValueModelFactory filterLevelValueModelFactory)
    {
        _filterLevelSettings = filterLevelSettings;
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (_filterLevelSettings.FilterLevelEnabled && _filterLevelSettings.DisplayOnHomePage)
        {
            var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueSearchModelAsync(new FilterLevelValueSearchModel());

            return View(model);
        }

        return Content(string.Empty);
    }
}
