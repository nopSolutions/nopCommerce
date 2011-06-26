using System.Web.Mvc;
using Nop.Plugin.LiveChat.LivePerson.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.LiveChat.LivePerson.Controllers
{
    public class LiveChatLivePersonController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly LivePersonChatSettings _livePersonChatSettings;

        public LiveChatLivePersonController(ISettingService settingService, 
            LivePersonChatSettings livePersonChatSettings)
        {
            this._settingService = settingService;
            this._livePersonChatSettings = livePersonChatSettings;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.ButtonCode = _livePersonChatSettings.ButtonCode;
            model.MonitoringCode = _livePersonChatSettings.MonitoringCode;

            return View("Nop.Plugin.LiveChat.LivePerson.Views.LiveChatLivePerson.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            
            //save settings
            _livePersonChatSettings.ButtonCode = model.ButtonCode;
            _livePersonChatSettings.MonitoringCode = model.MonitoringCode;
            _settingService.SaveSetting(_livePersonChatSettings);
            
            return View("Nop.Plugin.LiveChat.LivePerson.Views.LiveChatLivePerson.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo()
        {
            var model = new PublicInfoModel();
            model.ButtonCode = _livePersonChatSettings.ButtonCode;
            model.MonitoringCode = _livePersonChatSettings.MonitoringCode;

            return View("Nop.Plugin.LiveChat.LivePerson.Views.LiveChatLivePerson.PublicInfo", model);
        }
    }
}