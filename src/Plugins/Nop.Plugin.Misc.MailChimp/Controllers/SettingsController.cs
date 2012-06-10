using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.Mvc;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Misc.MailChimp.Models;
using Nop.Plugin.Misc.MailChimp.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Tasks;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.MailChimp.Controllers
{
    [AdminAuthorize]
    public class SettingsController : Controller
    {
        private const string VIEW_PATH = "Nop.Plugin.Misc.MailChimp.Views.Settings.Index";
        private readonly IMailChimpApiService _mailChimpApiService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly MailChimpSettings _settings;
        private readonly ISubscriptionEventQueueingService _subscriptionEventQueueingService;

        public SettingsController(ISettingService settingService, IScheduleTaskService scheduleTaskService, 
            IMailChimpApiService mailChimpApiService, ISubscriptionEventQueueingService subscriptionEventQueueingService, 
            ILocalizationService localizationService, MailChimpSettings settings)
        {
            this._settingService = settingService;
            this._scheduleTaskService = scheduleTaskService;
            this._mailChimpApiService = mailChimpApiService;
            this._subscriptionEventQueueingService = subscriptionEventQueueingService;
            this._localizationService = localizationService;
            this._settings = settings;
        }

        [NonAction]
        private void MapListOptions(MailChimpSettingsModel model)
        {
            NameValueCollection listOptions = _mailChimpApiService.RetrieveLists();

            //Ensure there will not be duplicates
            model.ListOptions.Clear();

            foreach (string key in listOptions.AllKeys)
            {
                model.ListOptions.Add(new SelectListItem { Text = key, Value = listOptions[key] });
            }
        }

        [NonAction]
        private MailChimpSettingsModel PrepareModel()
        {
            var model = new MailChimpSettingsModel();

            //Set the properties
            model.ApiKey = _settings.ApiKey;
            model.DefaultListId = _settings.DefaultListId;
            model.WebHookKey = _settings.WebHookKey;
            ScheduleTask task = FindScheduledTask();
            if (task != null)
            {
                model.AutoSyncEachMinutes = task.Seconds / 60;
                model.AutoSync = task.Enabled;
            }

            //Maps the list options
            MapListOptions(model);

            return model;
        }

        [NonAction]
        private ScheduleTask FindScheduledTask()
        {
            return _scheduleTaskService.GetTaskByType("Nop.Plugin.Misc.MailChimp.MailChimpSynchronizationTask, Nop.Plugin.Misc.MailChimp");
        }

        public ActionResult Index()
        {
            var model = PrepareModel();
            //Return the view
            return View(VIEW_PATH, model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Index(MailChimpSettingsModel model)
        {
            string saveResult = "";
            if (ModelState.IsValid)
            {
                _settings.DefaultListId = model.DefaultListId;
                _settings.ApiKey = model.ApiKey;
                _settings.WebHookKey = model.WebHookKey;

                _settingService.SaveSetting(_settings);
            }

            // Update the task
            var task = FindScheduledTask();
            if (task != null)
            {
                task.Enabled = model.AutoSync;
                task.Seconds = model.AutoSyncEachMinutes*60;
                _scheduleTaskService.UpdateTask(task);
                saveResult = _localizationService.GetResource("Plugin.Misc.MailChimp.AutoSyncRestart");
            }

            model = PrepareModel();
            //set result text
            model.SaveResult = saveResult;

            return View(VIEW_PATH, model);
        }

        [HttpPost, ActionName("Index")]
        [FormValueRequired("queueall")]
        public ActionResult QueueAll()
        {
            _subscriptionEventQueueingService.QueueAll();

            return Index();
        }

        [HttpPost, ActionName("Index")]
        [FormValueRequired("sync")]
        public ActionResult Sync()
        {
            var model = PrepareModel();
            try
            {
                var sb = new StringBuilder();

                var result = _mailChimpApiService.Synchronize();
                //subscribe
                sb.Append("Subscribe results: ");
                sb.Append(result.SubscribeResult);
                sb.Append("<br />");
                for (int i = 0; i < result.SubscribeErrors.Count; i++)
                {
                    sb.Append(result.SubscribeErrors[i]);
                    if (i != result.SubscribeErrors.Count - 1)
                        sb.Append("<br />");
                }

                //unsubscribe
                sb.Append("<br />");
                sb.Append("Unsubscribe results: ");
                sb.Append(result.UnsubscribeResult);
                sb.Append("<br />");
                for (int i = 0; i < result.UnsubscribeErrors.Count; i++)
                {
                    sb.Append(result.UnsubscribeErrors[i]);
                    if (i != result.UnsubscribeErrors.Count - 1)
                        sb.Append("<br />");
                }
                //set result text
                model.SyncResult = sb.ToString();
            }
            catch (Exception exc)
            {
                //set result text
                model.SyncResult = exc.ToString();
            }
            
            return View(VIEW_PATH, model);
        }
    }
}