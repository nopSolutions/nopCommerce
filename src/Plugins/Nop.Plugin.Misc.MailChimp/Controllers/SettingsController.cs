using System.Web.Mvc;
using Nop.Plugin.Misc.MailChimp.Data;
using Nop.Plugin.Misc.MailChimp.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.MailChimp.Controllers {
    public class SettingsController : Controller {
        private const string VIEW_PATH = "Nop.Plugin.Misc.MailChimp.Views.Settings.Index";
        private readonly ISettingService _settingService;
        private readonly MailChimpSettings _settings;

        public SettingsController(ISettingService settingService, MailChimpSettings settings) {
            _settingService = settingService;
            _settings = settings;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Index() {
            var model = new MailChimpSettingsModel();

            //Set the properties
            model.ApiKey = _settings.ApiKey;
            model.DefaultListId = _settings.DefaultListId;

            //Return the view
            return View(VIEW_PATH, model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Index(MailChimpSettingsModel model) {
            if (ModelState.IsValid) {
                _settings.DefaultListId = model.DefaultListId;
                _settings.ApiKey = model.ApiKey;

                _settingService.SaveSetting(_settings);
            }

            return View(VIEW_PATH, model);
        }
    }
}