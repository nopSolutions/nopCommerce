using System.Web.Mvc;
using Nop.Plugin.Widgets.GoogleAnalytics.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Controllers
{
    public class WidgetsGoogleAnalyticsController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly GoogleAnalyticsSettings _googleAnalyticsSettings;

        public WidgetsGoogleAnalyticsController(ISettingService settingService,
            GoogleAnalyticsSettings googleAnalyticsSettings)
        {
            this._settingService = settingService;
            this._googleAnalyticsSettings = googleAnalyticsSettings;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(int widgetId)
        {
            var model = new ConfigurationModel();
            model.GoogleId = _googleAnalyticsSettings.GoogleId;
            model.JavaScript = _googleAnalyticsSettings.JavaScript;

            return View("Nop.Plugin.Widgets.GoogleAnalytics.Views.WidgetsGoogleAnalytics.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(int widgetId, ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure(widgetId);

            //save settings
            _googleAnalyticsSettings.GoogleId = model.GoogleId;
            _googleAnalyticsSettings.JavaScript = model.JavaScript;
            _settingService.SaveSetting(_googleAnalyticsSettings);

            return View("Nop.Plugin.Widgets.GoogleAnalytics.Views.WidgetsGoogleAnalytics.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(int widgetId)
        {
            var model = new PublicInfoModel();
            model.GoogleId = _googleAnalyticsSettings.GoogleId;
            model.JavaScript = _googleAnalyticsSettings.JavaScript;

            return View("Nop.Plugin.Widgets.GoogleAnalytics.Views.WidgetsGoogleAnalytics.PublicInfo", model);
        }
    }
}