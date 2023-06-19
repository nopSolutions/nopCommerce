using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.Deals.Mapping;
using Nop.Plugin.Widgets.Deals.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Deals.Components;

public class TireDealsViewComponent : NopViewComponent
{
    private readonly ITireDealService _tireDealService;
    private readonly ITireDealMapper _tireDealMapper;

    public TireDealsViewComponent(ITireDealService tireDealService, ITireDealMapper tireDealMapper)
    {
        _tireDealService = tireDealService;
        _tireDealMapper = tireDealMapper;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var models = await _tireDealService.GetAllActiveAsync();

        var publicModels = await _tireDealMapper.ToModel(models);
        
        return View("~/Plugins/Widgets.TireDeals/Views/PublicInfo.cshtml", publicModels);
    }
}