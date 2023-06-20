using Microsoft.AspNetCore.Mvc;
using Nop.Services.Media;
using Nop.Services.TireDeals;
using Nop.Web.Areas.Admin.Models.TireDeals;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public class TireDealViewComponent : NopViewComponent
{
    private readonly ITireDealService _tireDealService;
    private readonly IPictureService _pictureService;

    public TireDealViewComponent(ITireDealService tireDealService, IPictureService pictureService)
    {
        _tireDealService = tireDealService;
        _pictureService = pictureService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var entities = await _tireDealService.GetAllActiveAsync();
        List<PublicInfoModel> models = new();
        
        foreach (var item in entities)
        {
            models.Add(new PublicInfoModel()
            {
                Id = item.Id,
                Title = item.Title,
                ShortDescription = item.ShortDescription,
                LongDescription = item.LongDescription,
                BackgroundPictureUrl = await _pictureService.GetPictureUrlAsync(item.BackgroundPictureId),
                BrandPictureUrl = await _pictureService.GetPictureUrlAsync(item.BrandPictureId),
                IsActive = item.IsActive
            });
        }
        
        return View("Default.cshtml", models);
    }
}