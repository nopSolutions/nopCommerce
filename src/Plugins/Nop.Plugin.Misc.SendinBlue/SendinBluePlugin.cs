using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Tasks;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.SendinBlue
{
    /// <summary>
    /// Represents the SendinBlue plugin
    /// </summary>
    public class SendinBluePlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
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

        public SendinBluePlugin(IEmailAccountService emailAccountService,
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
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.HeadHtmlTag };
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return SendinBlueDefaults.TRACKING_VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/SendinBlue/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new SendinBlueSettings
            {
                //prepopulate a tracking script
                TrackingScript = $@"<!-- SendinBlue tracting code -->
                <script>
                    (function() {{
                        window.sib = {{ equeue: [], client_key: '{SendinBlueDefaults.TrackingScriptId}' }};
                        window.sib.email_id = '{SendinBlueDefaults.TrackingScriptCustomerEmail}';
                        window.sendinblue = {{}}; for (var j = ['track', 'identify', 'trackLink', 'page'], i = 0; i < j.length; i++) {{ (function(k) {{ window.sendinblue[k] = function() {{ var arg = Array.prototype.slice.call(arguments); (window.sib[k] || function() {{ var t = {{}}; t[k] = arg; window.sib.equeue.push(t);}})(arg[0], arg[1], arg[2]);}};}})(j[i]);}}var n = document.createElement('script'),i = document.getElementsByTagName('script')[0]; n.type = 'text/javascript', n.id = 'sendinblue-js', n.async = !0, n.src = 'https://sibautomation.com/sa.js?key=' + window.sib.client_key, i.parentNode.insertBefore(n, i), window.sendinblue.page();
                    }})();
                </script>"
            });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(SendinBlueDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(SendinBlueDefaults.SystemName);
                _settingService.SaveSetting(_widgetSettings);
            }

            //install synchronization task
            if (_scheduleTaskService.GetTaskByType(SendinBlueDefaults.SynchronizationTask) == null)
            {
                _scheduleTaskService.InsertTask(new ScheduleTask
                {
                    Enabled = true,
                    Seconds = SendinBlueDefaults.DefaultSynchronizationPeriod * 60 * 60,
                    Name = SendinBlueDefaults.SynchronizationTaskName,
                    Type = SendinBlueDefaults.SynchronizationTask,
                });
            }

            //locales
            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Misc.SendinBlue.AccountInfo"] = "Account info",
                ["Plugins.Misc.SendinBlue.AccountInfo.Hint"] = "Display account information.",
                ["Plugins.Misc.SendinBlue.ActivateSMTP"] = "On your SendinBlue account, the SMTP has not been enabled yet. To request its activation, simply send an email to our support team at contact@sendinblue.com and mention that you will be using the SMTP with the nopCommerce plugin.",
                ["Plugins.Misc.SendinBlue.AddNewSMSNotification"] = "Add new SMS notification",
                ["Plugins.Misc.SendinBlue.BillingAddressPhone"] = "Billing address phone number",
                ["Plugins.Misc.SendinBlue.CustomerPhone"] = "Customer phone number",
                ["Plugins.Misc.SendinBlue.EditTemplate"] = "Edit template",
                ["Plugins.Misc.SendinBlue.Fields.AllowedTokens"] = "Allowed message variables",
                ["Plugins.Misc.SendinBlue.Fields.AllowedTokens.Hint"] = "This is a list of the message variables you can use in your SMS.",
                ["Plugins.Misc.SendinBlue.Fields.ApiKey"] = "API v3 key",
                ["Plugins.Misc.SendinBlue.Fields.ApiKey.Hint"] = "Paste your SendinBlue account API v3 key.",
                ["Plugins.Misc.SendinBlue.Fields.CampaignList"] = "List",
                ["Plugins.Misc.SendinBlue.Fields.CampaignList.Hint"] = "Choose list of contacts to send SMS campaign.",
                ["Plugins.Misc.SendinBlue.Fields.CampaignSenderName"] = "Send SMS campaign from",
                ["Plugins.Misc.SendinBlue.Fields.CampaignSenderName.Hint"] = "Input the name of the sender. The number of characters is limited to 11 (alphanumeric format).",
                ["Plugins.Misc.SendinBlue.Fields.CampaignText"] = "Text",
                ["Plugins.Misc.SendinBlue.Fields.CampaignText.Hint"] = "Specify SMS campaign content. The number of characters is limited to 160 for one message.",
                ["Plugins.Misc.SendinBlue.Fields.List"] = "List",
                ["Plugins.Misc.SendinBlue.Fields.List.Hint"] = "Select the SendinBlue list where your nopCommerce newsletter subscribers will be added.",
                ["Plugins.Misc.SendinBlue.Fields.MaKey"] = "Tracker ID",
                ["Plugins.Misc.SendinBlue.Fields.MaKey.Hint"] = "Input your Tracker ID.",
                ["Plugins.Misc.SendinBlue.Fields.Sender"] = "Send emails from",
                ["Plugins.Misc.SendinBlue.Fields.Sender.Hint"] = "Choose sender of your transactional emails.",
                ["Plugins.Misc.SendinBlue.Fields.SmsSenderName"] = "Send SMS from",
                ["Plugins.Misc.SendinBlue.Fields.SmsSenderName.Hint"] = "Input the name of the sender. The number of characters is limited to 11 (alphanumeric format).",
                ["Plugins.Misc.SendinBlue.Fields.SmtpKey"] = "SMTP key",
                ["Plugins.Misc.SendinBlue.Fields.SmtpKey.Hint"] = "Specify SMTP key (password).",
                ["Plugins.Misc.SendinBlue.Fields.StoreOwnerPhoneNumber"] = "Store owner phone number",
                ["Plugins.Misc.SendinBlue.Fields.StoreOwnerPhoneNumber.Hint"] = "Input store owner phone number for SMS notifications.",
                ["Plugins.Misc.SendinBlue.Fields.TrackingScript"] = "Tracking script",
                ["Plugins.Misc.SendinBlue.Fields.TrackingScript.Hint"] = $"Paste the tracking script generated by SendinBlue here. {SendinBlueDefaults.TrackingScriptId} and {SendinBlueDefaults.TrackingScriptCustomerEmail} will be dynamically replaced.",
                ["Plugins.Misc.SendinBlue.Fields.UseMarketingAutomation"] = "Use Marketing Automation",
                ["Plugins.Misc.SendinBlue.Fields.UseMarketingAutomation.Hint"] = "Check for enable SendinBlue Automation.",
                ["Plugins.Misc.SendinBlue.Fields.UseSmsNotifications"] = "Use SMS notifications",
                ["Plugins.Misc.SendinBlue.Fields.UseSmsNotifications.Hint"] = "Check for sending transactional SMS.",
                ["Plugins.Misc.SendinBlue.Fields.UseSmtp"] = "Use SendinBlue SMTP",
                ["Plugins.Misc.SendinBlue.Fields.UseSmtp.Hint"] = "Check for using SendinBlue SMTP for sending transactional emails.",
                ["Plugins.Misc.SendinBlue.General"] = "General",
                ["Plugins.Misc.SendinBlue.ImportProcess"] = "Your import is in process",
                ["Plugins.Misc.SendinBlue.ManualSync"] = "Manual synchronization",
                ["Plugins.Misc.SendinBlue.SyncNow"] = "Sync now",
                ["Plugins.Misc.SendinBlue.MarketingAutomation"] = "Marketing Automation",
                ["Plugins.Misc.SendinBlue.MyPhone"] = "Store owner phone number",
                ["Plugins.Misc.SendinBlue.PhoneType"] = "Type of phone number",
                ["Plugins.Misc.SendinBlue.PhoneType.Hint"] = "Specify the type of phone number to send SMS.",
                ["Plugins.Misc.SendinBlue.SMS"] = "SMS",
                ["Plugins.Misc.SendinBlue.SMS.Campaigns"] = "SMS campaigns",
                ["Plugins.Misc.SendinBlue.SMS.Campaigns.Sent"] = "Campaign successfully sent",
                ["Plugins.Misc.SendinBlue.SMS.Campaigns.Submit"] = "Send campaign",
                ["Plugins.Misc.SendinBlue.SMSText"] = "Text",
                ["Plugins.Misc.SendinBlue.SMSText.Hint"] = "Enter SMS text to send.",
                ["Plugins.Misc.SendinBlue.Synchronization"] = "Contacts",
                ["Plugins.Misc.SendinBlue.Transactional"] = "Transactional emails",
                ["Plugins.Misc.SendinBlue.UseSendinBlueTemplate"] = "SendinBlue template"
            });

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //smtp accounts
            foreach (var store in _storeService.GetAllStores())
            {
                var key = $"{nameof(SendinBlueSettings)}.{nameof(SendinBlueSettings.EmailAccountId)}";
                var emailAccountId = _settingService.GetSettingByKey<int>(key, storeId: store.Id, loadSharedValueIfNotFound: true);
                var emailAccount = _emailAccountService.GetEmailAccountById(emailAccountId);
                if (emailAccount != null)
                    _emailAccountService.DeleteEmailAccount(emailAccount);
            }

            //settings
            if (_widgetSettings.ActiveWidgetSystemNames.Contains(SendinBlueDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(SendinBlueDefaults.SystemName);
                _settingService.SaveSetting(_widgetSettings);
            }
            _settingService.DeleteSetting<SendinBlueSettings>();

            //generic attributes
            foreach (var store in _storeService.GetAllStores())
            {
                var messageTemplates = _messageTemplateService.GetAllMessageTemplates(store.Id);
                foreach (var messageTemplate in messageTemplates)
                {
                    _genericAttributeService.SaveAttribute<int?>(messageTemplate, SendinBlueDefaults.TemplateIdAttribute, null);
                }
            }

            //schedule task
            var task = _scheduleTaskService.GetTaskByType(SendinBlueDefaults.SynchronizationTask);
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.Misc.SendinBlue");

            base.Uninstall();
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => true;
    }
}