using System.Web.Mvc;
using Nop.Plugin.Widgets.LivePersonChat.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.LivePersonChat.Controllers
{
    public class WidgetsLivePersonChatController : Controller
    {
        private readonly ISettingService _settingService;

        public WidgetsLivePersonChatController(ISettingService settingService)
        {
            this._settingService = settingService;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(int widgetId)
        {
            var model = new ConfigurationModel();
            if (widgetId > 0)
            {
                model.ButtonCode = _settingService.GetSettingByKey<string>(string.Format("Widgets.LivePersonChat.ButtonCode.{0}", widgetId));
                model.MonitoringCode = _settingService.GetSettingByKey<string>(string.Format("Widgets.LivePersonChat.MonitoringCode.{0}", widgetId));
            }

            return View("Nop.Plugin.Widgets.LivePersonChat.Views.WidgetsLivePersonChat.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(int widgetId, ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure(widgetId);
            
            //save settings
            _settingService.SetSetting(string.Format("Widgets.LivePersonChat.ButtonCode.{0}", widgetId), model.ButtonCode);
            _settingService.SetSetting(string.Format("Widgets.LivePersonChat.MonitoringCode.{0}", widgetId), model.MonitoringCode);
            
            return View("Nop.Plugin.Widgets.LivePersonChat.Views.WidgetsLivePersonChat.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(int widgetId)
        {
            var model = new PublicInfoModel();
            model.ButtonCode = _settingService.GetSettingByKey<string>(string.Format("Widgets.LivePersonChat.ButtonCode.{0}", widgetId));
            model.MonitoringCode = _settingService.GetSettingByKey<string>(string.Format("Widgets.LivePersonChat.MonitoringCode.{0}", widgetId));

            return View("Nop.Plugin.Widgets.LivePersonChat.Views.WidgetsLivePersonChat.PublicInfo", model);
        }
    }
}