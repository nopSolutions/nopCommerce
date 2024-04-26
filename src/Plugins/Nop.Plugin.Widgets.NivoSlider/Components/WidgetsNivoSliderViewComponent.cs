using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.NivoSlider.Infrastructure.Cache;
using Nop.Plugin.Widgets.NivoSlider.Models;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.NivoSlider.Components;

public class WidgetsNivoSliderViewComponent : NopViewComponent
{
    protected readonly IStoreContext _storeContext;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly ISettingService _settingService;
    protected readonly IPictureService _pictureService;
    protected readonly IWebHelper _webHelper;

    public WidgetsNivoSliderViewComponent(IStoreContext storeContext,
        IStaticCacheManager staticCacheManager,
        ISettingService settingService,
        IPictureService pictureService,
        IWebHelper webHelper)
    {
        _storeContext = storeContext;
        _staticCacheManager = staticCacheManager;
        _settingService = settingService;
        _pictureService = pictureService;
        _webHelper = webHelper;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var nivoSliderSettings = await _settingService.LoadSettingAsync<NivoSliderSettings>(store.Id);

        var model = new PublicInfoModel
        {
            Picture1Url = await GetPictureUrlAsync(nivoSliderSettings.Picture1Id),
            Text1 = nivoSliderSettings.Text1,
            Link1 = nivoSliderSettings.Link1,
            AltText1 = nivoSliderSettings.AltText1,

            Picture2Url = await GetPictureUrlAsync(nivoSliderSettings.Picture2Id),
            Text2 = nivoSliderSettings.Text2,
            Link2 = nivoSliderSettings.Link2,
            AltText2 = nivoSliderSettings.AltText2,

            Picture3Url = await GetPictureUrlAsync(nivoSliderSettings.Picture3Id),
            Text3 = nivoSliderSettings.Text3,
            Link3 = nivoSliderSettings.Link3,
            AltText3 = nivoSliderSettings.AltText3,

            Picture4Url = await GetPictureUrlAsync(nivoSliderSettings.Picture4Id),
            Text4 = nivoSliderSettings.Text4,
            Link4 = nivoSliderSettings.Link4,
            AltText4 = nivoSliderSettings.AltText4,

            Picture5Url = await GetPictureUrlAsync(nivoSliderSettings.Picture5Id),
            Text5 = nivoSliderSettings.Text5,
            Link5 = nivoSliderSettings.Link5,
            AltText5 = nivoSliderSettings.AltText5
        };

        if (string.IsNullOrEmpty(model.Picture1Url) && string.IsNullOrEmpty(model.Picture2Url) &&
            string.IsNullOrEmpty(model.Picture3Url) && string.IsNullOrEmpty(model.Picture4Url) &&
            string.IsNullOrEmpty(model.Picture5Url))
            //no pictures uploaded
            return Content("");

        return View("~/Plugins/Widgets.NivoSlider/Views/PublicInfo.cshtml", model);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task<string> GetPictureUrlAsync(int pictureId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ModelCacheEventConsumer.PICTURE_URL_MODEL_KEY,
            pictureId, _webHelper.IsCurrentConnectionSecured() ? Uri.UriSchemeHttps : Uri.UriSchemeHttp);

        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            //little hack here. nulls aren't cacheable so set it to ""
            var url = await _pictureService.GetPictureUrlAsync(pictureId, showDefaultPicture: false) ?? "";
            return url;
        });
    }
}