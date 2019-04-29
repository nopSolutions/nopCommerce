using System.Collections.Generic;
using Nop.Core;
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

        #endregion

        #region Ctor

        public SendinBluePlugin(IEmailAccountService emailAccountService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            IStoreService storeService,
            IWebHelper webHelper)
        {
            _emailAccountService = emailAccountService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _storeService = storeService;
            _webHelper = webHelper;
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
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.AccountInfo", "Account info");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.AccountInfo.Hint", "Display account information.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.ActivateSMTP", "On your SendinBlue account, the SMTP has not been enabled yet. To request its activation, simply send an email to our support team at contact@sendinblue.com and mention that you will be using the SMTP with the nopCommerce plugin.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.AddNewSMSNotification", "Add new SMS notification");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.BillingAddressPhone", "Billing address phone number");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.CustomerPhone", "Customer phone number");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.EditTemplate", "Edit template");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.AllowedTokens", "Allowed message variables");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.AllowedTokens.Hint", "This is a list of the message variables you can use in your SMS.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.ApiKey", "API v3 key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.ApiKey.Hint", "Paste your SendinBlue account API v3 key.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignList", "List");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignList.Hint", "Choose list of contacts to send SMS campaign.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignSenderName", "Send SMS campaign from");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignSenderName.Hint", "Input the name of the sender. The number of characters is limited to 11 (alphanumeric format).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignText", "Text");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignText.Hint", "Specify SMS campaign content. The number of characters is limited to 160 for one message.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.List", "List");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.List.Hint", "Select the SendinBlue list where your nopCommerce newsletter subscribers will be added.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.MaKey", "Tracker ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.MaKey.Hint", "Input your Tracker ID.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.Sender", "Send emails from");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.Sender.Hint", "Choose sender of your transactional emails.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.SmsSenderName", "Send SMS from");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.SmsSenderName.Hint", "Input the name of the sender. The number of characters is limited to 11 (alphanumeric format).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.SmtpKey", "SMTP key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.SmtpKey.Hint", "Specify SMTP key (password).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.StoreOwnerPhoneNumber", "Store owner phone number");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.StoreOwnerPhoneNumber.Hint", "Input store owner phone number for SMS notifications.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.TrackingScript", "Tracking script");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.TrackingScript.Hint", $"Paste the tracking script generated by SendinBlue here. {SendinBlueDefaults.TrackingScriptId} and {SendinBlueDefaults.TrackingScriptCustomerEmail} will be dynamically replaced.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseMarketingAutomation", "Use Marketing Automation");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseMarketingAutomation.Hint", "Check for enable SendinBlue Automation.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseSmsNotifications", "Use SMS notifications");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseSmsNotifications.Hint", "Check for sending transactional SMS.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseSmtp", "Use SendinBlue SMTP");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseSmtp.Hint", "Check for using SendinBlue SMTP for sending transactional emails.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.General", "General");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.ImportProcess", "Your import is in process");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.ManualSync", "Manual synchronization");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.SyncNow", "Sync now");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.MarketingAutomation", "Marketing Automation");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.MyPhone", "Store owner phone number");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.PhoneType", "Type of phone number");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.PhoneType.Hint", "Specify the type of phone number to send SMS.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.SMS", "SMS");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.SMS.Campaigns", "SMS campaigns");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.SMS.Campaigns.Sent", "Campaign successfully sent");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.SMS.Campaigns.Submit", "Send campaign");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.SMSText", "Text");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.SMSText.Hint", "Enter SMS text to send.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Synchronization", "Contacts");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.Transactional", "Transactional emails");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.SendinBlue.UseSendinBlueTemplate", "SendinBlue template");

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
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.AccountInfo");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.AccountInfo.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.ActivateSMTP");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.AddNewSMSNotification");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.BillingAddressPhone");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.BillingAddressPhone");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.CustomerPhone");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.CustomerPhone");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.EditTemplate");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.EditTemplate");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.AllowedTokens");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.AllowedTokens.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.ApiKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.ApiKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignList");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignList.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignSenderName");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignSenderName.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignText");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.CampaignText.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.List");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.List.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.MaKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.MaKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.Sender");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.Sender.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.SmsSenderName");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.SmsSenderName.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.SmtpKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.SmtpKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.StoreOwnerPhoneNumber");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.StoreOwnerPhoneNumber.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.TrackingScript");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.TrackingScript.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseMarketingAutomation");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseMarketingAutomation.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseSmsNotifications");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseSmsNotifications.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseSmtp");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Fields.UseSmtp.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.General");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.ImportProcess");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.ManualSync");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.SyncNow");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.MarketingAutomation");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.MyPhone");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.MyPhone");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.PhoneType");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.PhoneType.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.SendinBlueTemplate");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.SendinBlueTemplate");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.SendinBlueTemplate");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.SMS");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.SMS.Campaigns");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.SMS.Campaigns.Sent");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.SMS.Campaigns.Submit");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.SMSText");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.SMSText.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.StandardTemplate");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Synchronization");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.Transactional");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.SendinBlue.UseSendinBlueTemplate");

            base.Uninstall();
        }

        #endregion
    }
}