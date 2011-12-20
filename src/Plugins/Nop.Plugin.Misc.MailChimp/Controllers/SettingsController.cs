using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Misc.MailChimp.Models;
using Nop.Plugin.Misc.MailChimp.Services;
using Nop.Services.Configuration;
using Nop.Services.Tasks;
using Nop.Web.Framework.Controllers;
using PerceptiveMCAPI.Types;

namespace Nop.Plugin.Misc.MailChimp.Controllers
{
    [AdminAuthorize]
    public class SettingsController : Controller
    {
        private const string VIEW_PATH = "Nop.Plugin.Misc.MailChimp.Views.Settings.Index";
        private readonly IMailChimpApiService _mailChimpApiService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;
        private readonly MailChimpSettings _settings;
        private readonly ISubscriptionEventQueueingService _subscriptionEventQueueingService;

        public SettingsController(ISettingService settingService, IScheduleTaskService scheduleTaskService, IMailChimpApiService mailChimpApiService, ISubscriptionEventQueueingService subscriptionEventQueueingService, MailChimpSettings settings)
        {
            _settingService = settingService;
            _scheduleTaskService = scheduleTaskService;
            _mailChimpApiService = mailChimpApiService;
            _subscriptionEventQueueingService = subscriptionEventQueueingService;
            _settings = settings;
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
        private string ParseErrorReport(IList<Api_Error> apiErrorMessages)
        {
            if (apiErrorMessages.Count > 0)
            {
                var sb = new StringBuilder();
                //output.api_Request, output.api_Response, // raw data
                //output.api_ErrorMessages, output.api_ValidatorMessages); // & errors
                for (int i = 0; i < apiErrorMessages.Count; i++)
                {
                    sb.Append(apiErrorMessages[i].error);
                    if (i != apiErrorMessages.Count - 1) sb.Append("<br />");
                }
                //output.result.add_count
                //output.result.error_count
                //output.result.update_count
                //output.result.errors

                return sb.ToString();
            }
            return null;
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
            return _scheduleTaskService.GetAllTasks().Where(x => x.Type.Equals("Nop.Plugin.Misc.MailChimp.MailChimpSynchronizationTask, Nop.Plugin.Misc.MailChimp", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
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
                saveResult = "If sync task period has been changed, please restart the application";
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

                //subscribe
                var subscribeResult = _mailChimpApiService.BatchSubscribe();
                sb.Append("Subscribe results: ");
                if (subscribeResult.api_ErrorMessages.Count > 0)
                {
                    //output.result.add_count
                    //output.result.error_count
                    //output.result.update_count
                    //output.result.errors
                    //output.api_Request, output.api_Response, // raw data
                    //output.api_ErrorMessages, output.api_ValidatorMessages); // & errors
                    for (int i = 0; i < subscribeResult.api_ErrorMessages.Count; i++)
                    {
                        sb.Append(subscribeResult.api_ErrorMessages[i].error);
                        if (i != subscribeResult.api_ErrorMessages.Count - 1) 
                            sb.Append("<br />");
                    }
                }
                else
                {
                    sb.Append(subscribeResult);
                }

                //unsubscribe
                var unsubscribeResult = _mailChimpApiService.BatchUnsubscribe();
                sb.Append("<br />Unsubscribe results: ");
                if (unsubscribeResult.api_ErrorMessages.Count > 0)
                {
                    for (int i = 0; i < unsubscribeResult.api_ErrorMessages.Count; i++)
                    {
                        sb.Append(unsubscribeResult.api_ErrorMessages[i].error);
                        if (i != unsubscribeResult.api_ErrorMessages.Count - 1) 
                            sb.Append("<br />");
                    }
                }
                else
                {
                    sb.Append(unsubscribeResult);
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