using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Stores;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.Sendinblue
{
    /// <summary>
    /// Represents the Sendinblue plugin
    /// </summary>
    public class SendinbluePlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
    {
        #region Fields

        private readonly IEmailAccountService _emailAccountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWebHelper _webHelper;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public SendinbluePlugin(IEmailAccountService emailAccountService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            IStoreService storeService,
            IWebHelper webHelper,
            WidgetSettings widgetSettings)
        {
            _emailAccountService = emailAccountService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _storeService = storeService;
            _webHelper = webHelper;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HeadHtmlTag });
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return SendinblueDefaults.TRACKING_VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Sendinblue/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new SendinblueSettings
            {
                //prepopulate a tracking script
                TrackingScript = $@"<!-- Sendinblue tracking code -->
                <script>
                    (function() {{
                        window.sib = {{ equeue: [], client_key: '{SendinblueDefaults.TrackingScriptId}' }};
                        window.sib.email_id = '{SendinblueDefaults.TrackingScriptCustomerEmail}';
                        window.sendinblue = {{}}; for (var j = ['track', 'identify', 'trackLink', 'page'], i = 0; i < j.length; i++) {{ (function(k) {{ window.sendinblue[k] = function() {{ var arg = Array.prototype.slice.call(arguments); (window.sib[k] || function() {{ var t = {{}}; t[k] = arg; window.sib.equeue.push(t);}})(arg[0], arg[1], arg[2]);}};}})(j[i]);}}var n = document.createElement('script'),i = document.getElementsByTagName('script')[0]; n.type = 'text/javascript', n.id = 'sendinblue-js', n.async = !0, n.src = 'https://sibautomation.com/sa.js?key=' + window.sib.client_key, i.parentNode.insertBefore(n, i), window.sendinblue.page();
                    }})();
                </script>"
            });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(SendinblueDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(SendinblueDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            //install synchronization task
            if (await _scheduleTaskService.GetTaskByTypeAsync(SendinblueDefaults.SynchronizationTask) == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
                {
                    Enabled = true,
                    LastEnabledUtc = DateTime.UtcNow,
                    Seconds = SendinblueDefaults.DefaultSynchronizationPeriod * 60 * 60,
                    Name = SendinblueDefaults.SynchronizationTaskName,
                    Type = SendinblueDefaults.SynchronizationTask,
                });
            }

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.Sendinblue.AccountInfo"] = "Account info",
                ["Plugins.Misc.Sendinblue.AccountInfo.Hint"] = "Display account information.",
                ["Plugins.Misc.Sendinblue.ActivateSMTP"] = "On your Sendinblue account, the SMTP has not been enabled yet. To request its activation, simply send an email to our support team at contact@sendinblue.com and mention that you will be using the SMTP with the nopCommerce plugin.",
                ["Plugins.Misc.Sendinblue.AddNewSMSNotification"] = "Add new SMS notification",
                ["Plugins.Misc.Sendinblue.BillingAddressPhone"] = "Billing address phone number",
                ["Plugins.Misc.Sendinblue.CustomerPhone"] = "Customer phone number",
                ["Plugins.Misc.Sendinblue.EditTemplate"] = "Edit template",
                ["Plugins.Misc.Sendinblue.Fields.AllowedTokens"] = "Allowed message variables",
                ["Plugins.Misc.Sendinblue.Fields.AllowedTokens.Hint"] = "This is a list of the message variables you can use in your SMS.",
                ["Plugins.Misc.Sendinblue.Fields.ApiKey"] = "API v3 key",
                ["Plugins.Misc.Sendinblue.Fields.ApiKey.Hint"] = "Paste your Sendinblue account API v3 key.",
                ["Plugins.Misc.Sendinblue.Fields.CampaignList"] = "List",
                ["Plugins.Misc.Sendinblue.Fields.CampaignList.Hint"] = "Choose list of contacts to send SMS campaign.",
                ["Plugins.Misc.Sendinblue.Fields.CampaignSenderName"] = "Send SMS campaign from",
                ["Plugins.Misc.Sendinblue.Fields.CampaignSenderName.Hint"] = "Input the name of the sender. The number of characters is limited to 11 (alphanumeric format).",
                ["Plugins.Misc.Sendinblue.Fields.CampaignText"] = "Text",
                ["Plugins.Misc.Sendinblue.Fields.CampaignText.Hint"] = "Specify SMS campaign content. The number of characters is limited to 160 for one message.",
                ["Plugins.Misc.Sendinblue.Fields.List"] = "List",
                ["Plugins.Misc.Sendinblue.Fields.List.Hint"] = "Select the Sendinblue list where your nopCommerce newsletter subscribers will be added.",
                ["Plugins.Misc.Sendinblue.Fields.MaKey"] = "Tracker ID",
                ["Plugins.Misc.Sendinblue.Fields.MaKey.Hint"] = "Input your Tracker ID.",
                ["Plugins.Misc.Sendinblue.Fields.Sender"] = "Send emails from",
                ["Plugins.Misc.Sendinblue.Fields.Sender.Hint"] = "Choose sender of your transactional emails.",
                ["Plugins.Misc.Sendinblue.Fields.SmsSenderName"] = "Send SMS from",
                ["Plugins.Misc.Sendinblue.Fields.SmsSenderName.Hint"] = "Input the name of the sender. The number of characters is limited to 11 (alphanumeric format).",
                ["Plugins.Misc.Sendinblue.Fields.SmtpKey"] = "SMTP key",
                ["Plugins.Misc.Sendinblue.Fields.SmtpKey.Hint"] = "Specify SMTP key (password).",
                ["Plugins.Misc.Sendinblue.Fields.StoreOwnerPhoneNumber"] = "Store owner phone number",
                ["Plugins.Misc.Sendinblue.Fields.StoreOwnerPhoneNumber.Hint"] = "Input store owner phone number for SMS notifications.",
                ["Plugins.Misc.Sendinblue.Fields.TrackingScript"] = "Tracking script",
                ["Plugins.Misc.Sendinblue.Fields.TrackingScript.Hint"] = $"Paste the tracking script generated by Sendinblue here. {SendinblueDefaults.TrackingScriptId} and {SendinblueDefaults.TrackingScriptCustomerEmail} will be dynamically replaced.",
                ["Plugins.Misc.Sendinblue.Fields.UseMarketingAutomation"] = "Use Marketing Automation",
                ["Plugins.Misc.Sendinblue.Fields.UseMarketingAutomation.Hint"] = "Check for enable Sendinblue Automation.",
                ["Plugins.Misc.Sendinblue.Fields.UseSmsNotifications"] = "Use SMS notifications",
                ["Plugins.Misc.Sendinblue.Fields.UseSmsNotifications.Hint"] = "Check for sending transactional SMS.",
                ["Plugins.Misc.Sendinblue.Fields.UseSmtp"] = "Use Sendinblue SMTP",
                ["Plugins.Misc.Sendinblue.Fields.UseSmtp.Hint"] = "Check for using Sendinblue SMTP for sending transactional emails.",
                ["Plugins.Misc.Sendinblue.General"] = "General",
                ["Plugins.Misc.Sendinblue.ImportProcess"] = "Your import is in process",
                ["Plugins.Misc.Sendinblue.ManualSync"] = "Manual synchronization",
                ["Plugins.Misc.Sendinblue.SyncNow"] = "Sync now",
                ["Plugins.Misc.Sendinblue.MarketingAutomation"] = "Marketing Automation",
                ["Plugins.Misc.Sendinblue.MyPhone"] = "Store owner phone number",
                ["Plugins.Misc.Sendinblue.PhoneType"] = "Type of phone number",
                ["Plugins.Misc.Sendinblue.PhoneType.Hint"] = "Specify the type of phone number to send SMS.",
                ["Plugins.Misc.Sendinblue.SMS"] = "SMS",
                ["Plugins.Misc.Sendinblue.SMS.Campaigns"] = "SMS campaigns",
                ["Plugins.Misc.Sendinblue.SMS.Campaigns.Sent"] = "Campaign successfully sent",
                ["Plugins.Misc.Sendinblue.SMS.Campaigns.Submit"] = "Send campaign",
                ["Plugins.Misc.Sendinblue.SMSText"] = "Text",
                ["Plugins.Misc.Sendinblue.SMSText.Hint"] = "Enter SMS text to send.",
                ["Plugins.Misc.Sendinblue.Synchronization"] = "Contacts",
                ["Plugins.Misc.Sendinblue.Transactional"] = "Transactional emails",
                ["Plugins.Misc.Sendinblue.UseSendinblueTemplate"] = "Sendinblue template"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //smtp accounts
            foreach (var store in await _storeService.GetAllStoresAsync())
            {
                var key = $"{nameof(SendinblueSettings)}.{nameof(SendinblueSettings.EmailAccountId)}";
                var emailAccountId = await _settingService.GetSettingByKeyAsync<int>(key, storeId: store.Id, loadSharedValueIfNotFound: true);
                var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(emailAccountId);
                if (emailAccount != null)
                    await _emailAccountService.DeleteEmailAccountAsync(emailAccount);
            }

            //settings
            if (_widgetSettings.ActiveWidgetSystemNames.Contains(SendinblueDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(SendinblueDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }
            await _settingService.DeleteSettingAsync<SendinblueSettings>();

            //generic attributes
            foreach (var store in await _storeService.GetAllStoresAsync())
            {
                var messageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(store.Id);
                foreach (var messageTemplate in messageTemplates)
                {
                    await _genericAttributeService.SaveAttributeAsync<int?>(messageTemplate, SendinblueDefaults.TemplateIdAttribute, null);
                }
            }

            //schedule task
            var task = await _scheduleTaskService.GetTaskByTypeAsync(SendinblueDefaults.SynchronizationTask);
            if (task != null)
                await _scheduleTaskService.DeleteTaskAsync(task);

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.Sendinblue");

            await base.UninstallAsync();
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => true;
    }
}
