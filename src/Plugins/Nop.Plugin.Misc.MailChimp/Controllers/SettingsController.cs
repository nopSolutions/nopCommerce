using System.Collections.Specialized;
using System.Web.Mvc;
using Nop.Plugin.Misc.MailChimp.Data;
using Nop.Plugin.Misc.MailChimp.Models;
using Nop.Plugin.Misc.MailChimp.Services;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.MailChimp.Controllers {
    public class SettingsController : Controller {
        private const string VIEW_PATH = "Nop.Plugin.Misc.MailChimp.Views.Settings.Index";
        private readonly ISettingService _settingService;
        private readonly IMailChimpApiService _mailChimpApiService;
        private readonly MailChimpSettings _settings;

        public SettingsController(ISettingService settingService, IMailChimpApiService mailChimpApiService, MailChimpSettings settings) {
            _settingService = settingService;
            _mailChimpApiService = mailChimpApiService;
            _settings = settings;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Index() {
            var model = new MailChimpSettingsModel();

            //Set the properties
            model.ApiKey = _settings.ApiKey;
            model.DefaultListId = _settings.DefaultListId;

            //Maps the list options
            MapListOptions(model);

            //Return the view
            return View(VIEW_PATH, model);
        }

        [NonAction]
        private void MapListOptions(MailChimpSettingsModel model) {
            NameValueCollection listOptions = _mailChimpApiService.RetrieveLists();

            //Ensure there will not be duplicates
            model.ListOptions.Clear();

            foreach(var key in listOptions.AllKeys) {
                model.ListOptions.Add( new SelectListItem { Text = key, Value = listOptions[key] });
            }
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

            //Maps the list options
            MapListOptions(model);

            return View(VIEW_PATH, model);
        }
    }
}