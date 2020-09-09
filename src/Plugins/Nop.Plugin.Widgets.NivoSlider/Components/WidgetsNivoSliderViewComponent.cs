using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.NivoSlider.Infrastructure.Cache;
using Nop.Plugin.Widgets.NivoSlider.Models;
using Nop.Services.Caching;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.NivoSlider.Components
{
    [ViewComponent(Name = "WidgetsNivoSlider")]
    public class WidgetsNivoSliderViewComponent : NopViewComponent
    {
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;

        public WidgetsNivoSliderViewComponent(ICacheKeyService cacheKeyService,
            IStoreContext storeContext, 
            IStaticCacheManager staticCacheManager, 
            ISettingService settingService, 
            IPictureService pictureService,
            IWebHelper webHelper)
        {
            _cacheKeyService = cacheKeyService;
            _storeContext = storeContext;
            _staticCacheManager = staticCacheManager;
            _settingService = settingService;
            _pictureService = pictureService;
            _webHelper = webHelper;
        }

        public async Task<IViewComponentResult> Invoke(string widgetZone, object additionalData)
        {
            var nivoSliderSettings = await _settingService.LoadSetting<NivoSliderSettings>((await _storeContext.GetCurrentStore()).Id);

            var model = new PublicInfoModel
            {
                Picture1Url = await GetPictureUrl(nivoSliderSettings.Picture1Id),
                Text1 = nivoSliderSettings.Text1,
                Link1 = nivoSliderSettings.Link1,
                AltText1 = nivoSliderSettings.AltText1,

                Picture2Url = await GetPictureUrl(nivoSliderSettings.Picture2Id),
                Text2 = nivoSliderSettings.Text2,
                Link2 = nivoSliderSettings.Link2,
                AltText2 = nivoSliderSettings.AltText2,

                Picture3Url = await GetPictureUrl(nivoSliderSettings.Picture3Id),
                Text3 = nivoSliderSettings.Text3,
                Link3 = nivoSliderSettings.Link3,
                AltText3 = nivoSliderSettings.AltText3,

                Picture4Url = await GetPictureUrl(nivoSliderSettings.Picture4Id),
                Text4 = nivoSliderSettings.Text4,
                Link4 = nivoSliderSettings.Link4,
                AltText4 = nivoSliderSettings.AltText4,

                Picture5Url = await GetPictureUrl(nivoSliderSettings.Picture5Id),
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

        protected async Task<string> GetPictureUrl(int pictureId)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.PICTURE_URL_MODEL_KEY, 
                pictureId, await _webHelper.IsCurrentConnectionSecured() ? Uri.UriSchemeHttps : Uri.UriSchemeHttp);

            return await _staticCacheManager.Get(cacheKey, async () =>
            {
                //little hack here. nulls aren't cacheable so set it to ""
                var url = await _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false) ?? "";
                return url;
            });
        }
    }
}
