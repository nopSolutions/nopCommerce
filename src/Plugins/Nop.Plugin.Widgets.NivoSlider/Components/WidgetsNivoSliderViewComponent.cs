using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.NivoSlider.Infrastructure.Cache;
using Nop.Plugin.Widgets.NivoSlider.Models;
using Nop.Services.Configuration;
using Nop.Services.Media;

namespace Nop.Plugin.Widgets.NivoSlider.Components
{
    [ViewComponent(Name = "WidgetsNivoSlider")]
    public class WidgetsNivoSliderViewComponent : ViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;

        public WidgetsNivoSliderViewComponent(IStoreContext storeContext, ICacheManager cacheManager, 
            ISettingService settingService, IPictureService pictureService)
        {
            this._storeContext = storeContext;
            this._cacheManager = cacheManager;
            this._settingService = settingService;
            this._pictureService = pictureService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var nivoSliderSettings = _settingService.LoadSetting<NivoSliderSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel();
            model.Picture1Url = GetPictureUrl(nivoSliderSettings.Picture1Id);
            model.Text1 = nivoSliderSettings.Text1;
            model.Link1 = nivoSliderSettings.Link1;

            model.Picture2Url = GetPictureUrl(nivoSliderSettings.Picture2Id);
            model.Text2 = nivoSliderSettings.Text2;
            model.Link2 = nivoSliderSettings.Link2;

            model.Picture3Url = GetPictureUrl(nivoSliderSettings.Picture3Id);
            model.Text3 = nivoSliderSettings.Text3;
            model.Link3 = nivoSliderSettings.Link3;

            model.Picture4Url = GetPictureUrl(nivoSliderSettings.Picture4Id);
            model.Text4 = nivoSliderSettings.Text4;
            model.Link4 = nivoSliderSettings.Link4;

            model.Picture5Url = GetPictureUrl(nivoSliderSettings.Picture5Id);
            model.Text5 = nivoSliderSettings.Text5;
            model.Link5 = nivoSliderSettings.Link5;

            if (string.IsNullOrEmpty(model.Picture1Url) && string.IsNullOrEmpty(model.Picture2Url) &&
                string.IsNullOrEmpty(model.Picture3Url) && string.IsNullOrEmpty(model.Picture4Url) &&
                string.IsNullOrEmpty(model.Picture5Url))
                //no pictures uploaded
                return Content("");

            return View("~/Plugins/Widgets.NivoSlider/Views/PublicInfo.cshtml", model);
        }

        protected string GetPictureUrl(int pictureId)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.PICTURE_URL_MODEL_KEY, pictureId);
            return _cacheManager.Get(cacheKey, () =>
            {
                //little hack here. nulls aren't cacheable so set it to ""
                var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false) ?? "";

                return url;
            });
        }
    }
}
