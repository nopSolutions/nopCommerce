using System.Web.Mvc;
using Nop.Plugin.Widgets.NivoSlider.Models;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.NivoSlider.Controllers
{
    public class WidgetsNivoSliderController : Controller
    {
        private readonly IPictureService _pictureService;
        private readonly NivoSliderSettings _nivoSliderSettings;
        private readonly ISettingService _settingService;

        public WidgetsNivoSliderController(IPictureService pictureService, 
            NivoSliderSettings nivoSliderSettings, ISettingService settingService)
        {
            this._pictureService = pictureService;
            this._nivoSliderSettings = nivoSliderSettings;
            this._settingService = settingService;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.Picture1Id = _nivoSliderSettings.Picture1Id;
            model.Text1 = _nivoSliderSettings.Text1;
            model.Link1 = _nivoSliderSettings.Link1;

            model.Picture2Id = _nivoSliderSettings.Picture2Id;
            model.Text2 = _nivoSliderSettings.Text2;
            model.Link2 = _nivoSliderSettings.Link2;

            model.Picture3Id = _nivoSliderSettings.Picture3Id;
            model.Text3 = _nivoSliderSettings.Text3;
            model.Link3 = _nivoSliderSettings.Link3;

            model.Picture4Id = _nivoSliderSettings.Picture4Id;
            model.Text4 = _nivoSliderSettings.Text4;
            model.Link4 = _nivoSliderSettings.Link4;

            return View("Nop.Plugin.Widgets.NivoSlider.Views.WidgetsNivoSlider.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _nivoSliderSettings.Picture1Id = model.Picture1Id;
            _nivoSliderSettings.Text1 = model.Text1;
            _nivoSliderSettings.Link1 = model.Link1;

            _nivoSliderSettings.Picture2Id = model.Picture2Id;
            _nivoSliderSettings.Text2 = model.Text2;
            _nivoSliderSettings.Link2 = model.Link2;

            _nivoSliderSettings.Picture3Id = model.Picture3Id;
            _nivoSliderSettings.Text3 = model.Text3;
            _nivoSliderSettings.Link3 = model.Link3;

            _nivoSliderSettings.Picture4Id = model.Picture4Id;
            _nivoSliderSettings.Text4 = model.Text4;
            _nivoSliderSettings.Link4 = model.Link4;
            _settingService.SaveSetting(_nivoSliderSettings);

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone)
        {
            var model = new PublicInfoModel();
            model.Picture1Url = _pictureService.GetPictureUrl(_nivoSliderSettings.Picture1Id, showDefaultPicture: false);
            model.Text1 = _nivoSliderSettings.Text1;
            model.Link1 = _nivoSliderSettings.Link1;

            model.Picture2Url = _pictureService.GetPictureUrl(_nivoSliderSettings.Picture2Id, showDefaultPicture: false);
            model.Text2 = _nivoSliderSettings.Text2;
            model.Link2 = _nivoSliderSettings.Link2;

            model.Picture3Url = _pictureService.GetPictureUrl(_nivoSliderSettings.Picture3Id, showDefaultPicture: false);
            model.Text3 = _nivoSliderSettings.Text3;
            model.Link3 = _nivoSliderSettings.Link3;

            model.Picture4Url = _pictureService.GetPictureUrl(_nivoSliderSettings.Picture4Id, showDefaultPicture: false);
            model.Text4 = _nivoSliderSettings.Text4;
            model.Link4 = _nivoSliderSettings.Link4;

            if (string.IsNullOrEmpty(model.Picture1Url) && string.IsNullOrEmpty(model.Picture2Url) &&
                string.IsNullOrEmpty(model.Picture3Url) && string.IsNullOrEmpty(model.Picture4Url))
                //no pictures uploaded
                return Content("");
            

            return View("Nop.Plugin.Widgets.NivoSlider.Views.WidgetsNivoSlider.PublicInfo", model);
        }
    }
}