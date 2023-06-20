using Microsoft.AspNetCore.Mvc;
using Nop.Services.TireDeals;
using Nop.Web.Areas.Admin.Models.TireDeals;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public class TireDealsViewComponent : NopViewComponent
{
    private readonly ITireDealService _tireDealService;

    public TireDealsViewComponent(ITireDealService tireDealService)
    {
        _tireDealService = tireDealService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var entities = await _tireDealService.GetAllActiveAsync();
        List<TireDealModel> models = new();
        
        foreach (var item in entities)
        {
            models.Add(new TireDealModel()
            {
                Id = item.Id,
                Title = item.Title,
                ShortDescription = item.ShortDescription,
                LongDescription = item.LongDescription,
                BackgroundPictureId = item.BackgroundPictureId,
                BrandPictureId = item.BrandPictureId
            });
        }
        
        return View(models);
    }
}