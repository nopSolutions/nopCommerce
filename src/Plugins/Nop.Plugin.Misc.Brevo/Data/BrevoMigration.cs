using FluentMigrator;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Misc.Brevo.Data
{
    [NopMigration("2023-05-22 17:00:00", "Misc.Brevo 4.70.4. Rename Sendinblue to Brevo.", MigrationProcessType.Update)]
    public class BrevoMigration : MigrationBase
    {
        #region Fields

        protected readonly ILocalizationService _localizationService;
        protected readonly INopDataProvider _dataProvider;
        protected readonly ISettingService _settingService;
        protected readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public BrevoMigration(
            ILocalizationService localizationService,
            INopDataProvider dataProvider,
            ISettingService settingService,
            WidgetSettings widgetSettings
            )
        {
            _localizationService = localizationService;
            _dataProvider = dataProvider;
            _settingService = settingService;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods        

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            #region locales

            var (languageId, languages) = this.GetLanguageData();

            //rename locales
            this.RenameLocales(new Dictionary<string, string>
            {
                ["Plugins.Misc.Sendinblue.AccountInfo"] = "Plugins.Misc.Brevo.AccountInfo",
                ["Plugins.Misc.Sendinblue.AccountInfo"] = "Plugins.Misc.Brevo.AccountInfo",
                ["Plugins.Misc.Sendinblue.AccountInfo.Hint"] = "Plugins.Misc.Brevo.AccountInfo.Hint",
                ["Plugins.Misc.Sendinblue.ActivateSMTP"] = "Plugins.Misc.Brevo.ActivateSMTP",
                ["Plugins.Misc.Sendinblue.AddNewSMSNotification"] = "Plugins.Misc.Brevo.AddNewSMSNotification",
                ["Plugins.Misc.Sendinblue.BillingAddressPhone"] = "Plugins.Misc.Brevo.BillingAddressPhone",
                ["Plugins.Misc.Sendinblue.CustomerPhone"] = "Plugins.Misc.Brevo.CustomerPhone",
                ["Plugins.Misc.Sendinblue.EditTemplate"] = "Plugins.Misc.Brevo.EditTemplate",
                ["Plugins.Misc.Sendinblue.EditTemplate"] = "Plugins.Misc.Brevo.EditTemplate",
                ["Plugins.Misc.Sendinblue.Fields.AllowedTokens.Hint"] = "Plugins.Misc.Brevo.Fields.AllowedTokens.Hint",
                ["Plugins.Misc.Sendinblue.Fields.ApiKey"] = "Plugins.Misc.Brevo.Fields.ApiKey",
                ["Plugins.Misc.Sendinblue.Fields.ApiKey.Hint"] = "Plugins.Misc.Brevo.Fields.ApiKey.Hint",
                ["Plugins.Misc.Sendinblue.Fields.CampaignList"] = "Plugins.Misc.Brevo.Fields.CampaignList",
                ["Plugins.Misc.Sendinblue.Fields.CampaignList.Hint"] = "Plugins.Misc.Brevo.Fields.CampaignList.Hint",
                ["Plugins.Misc.Sendinblue.Fields.CampaignSenderName"] = "Plugins.Misc.Brevo.Fields.CampaignSenderName",
                ["Plugins.Misc.Sendinblue.Fields.CampaignSenderName.Hint"] = "Plugins.Misc.Brevo.Fields.CampaignSenderName.Hint",
                ["Plugins.Misc.Sendinblue.Fields.CampaignText"] = "Plugins.Misc.Brevo.Fields.CampaignText",
                ["Plugins.Misc.Sendinblue.Fields.CampaignText.Hint"] = "Plugins.Misc.Brevo.Fields.CampaignText.Hint",
                ["Plugins.Misc.Sendinblue.Fields.List"] = "Plugins.Misc.Brevo.Fields.List",
                ["Plugins.Misc.Sendinblue.Fields.List.Hint"] = "Plugins.Misc.Brevo.Fields.List.Hint",
                ["Plugins.Misc.Sendinblue.Fields.MaKey"] = "Plugins.Misc.Brevo.Fields.MaKey",
                ["Plugins.Misc.Sendinblue.Fields.MaKey.Hint"] = "Plugins.Misc.Brevo.Fields.MaKey.Hint",
                ["Plugins.Misc.Sendinblue.Fields.Sender"] = "Plugins.Misc.Brevo.Fields.Sender",
                ["Plugins.Misc.Sendinblue.Fields.Sender.Hint"] = "Plugins.Misc.Brevo.Fields.Sender.Hint",
                ["Plugins.Misc.Sendinblue.Fields.SmsSenderName"] = "Plugins.Misc.Brevo.Fields.SmsSenderName",
                ["Plugins.Misc.Sendinblue.Fields.SmsSenderName.Hint"] = "Plugins.Misc.Brevo.Fields.SmsSenderName.Hint",
                ["Plugins.Misc.Sendinblue.Fields.SmtpKey"] = "Plugins.Misc.Brevo.Fields.SmtpKey",
                ["Plugins.Misc.Sendinblue.Fields.SmtpKey.Hint"] = "Plugins.Misc.Brevo.Fields.SmtpKey.Hint",
                ["Plugins.Misc.Sendinblue.Fields.StoreOwnerPhoneNumber"] = "Plugins.Misc.Brevo.Fields.StoreOwnerPhoneNumber",
                ["Plugins.Misc.Sendinblue.Fields.StoreOwnerPhoneNumber.Hint"] = "Plugins.Misc.Brevo.Fields.StoreOwnerPhoneNumber.Hint",
                ["Plugins.Misc.Sendinblue.Fields.TrackingScript"] = "Plugins.Misc.Brevo.Fields.TrackingScript",
                ["Plugins.Misc.Sendinblue.Fields.TrackingScript.Hint"] = "Plugins.Misc.Brevo.Fields.TrackingScript.Hint",
                ["Plugins.Misc.Sendinblue.Fields.UseMarketingAutomation"] = "Plugins.Misc.Brevo.Fields.UseMarketingAutomation",
                ["Plugins.Misc.Sendinblue.Fields.UseMarketingAutomation.Hint"] = "Plugins.Misc.Brevo.Fields.UseMarketingAutomation.Hint",
                ["Plugins.Misc.Sendinblue.Fields.UseSmsNotifications"] = "Plugins.Misc.Brevo.Fields.UseSmsNotifications",
                ["Plugins.Misc.Sendinblue.Fields.UseSmsNotifications.Hint"] = "Plugins.Misc.Brevo.Fields.UseSmsNotifications.Hint",
                ["Plugins.Misc.Sendinblue.Fields.UseSmtp"] = "Plugins.Misc.Brevo.Fields.UseSmtp",
                ["Plugins.Misc.Sendinblue.Fields.UseSmtp.Hint"] = "Plugins.Misc.Brevo.Fields.UseSmtp.Hint",
                ["Plugins.Misc.Sendinblue.General"] = "Plugins.Misc.Brevo.General",
                ["Plugins.Misc.Sendinblue.ImportProcess"] = "Plugins.Misc.Brevo.ImportProcess",
                ["Plugins.Misc.Sendinblue.ManualSync"] = "Plugins.Misc.Brevo.ManualSync",
                ["Plugins.Misc.Sendinblue.SyncNow"] = "Plugins.Misc.Brevo.SyncNow",
                ["Plugins.Misc.Sendinblue.MarketingAutomation"] = "Plugins.Misc.Brevo.MarketingAutomation",
                ["Plugins.Misc.Sendinblue.MyPhone"] = "Plugins.Misc.Brevo.MyPhone",
                ["Plugins.Misc.Sendinblue.PhoneType"] = "Plugins.Misc.Brevo.PhoneType",
                ["Plugins.Misc.Sendinblue.PhoneType.Hint"] = "Plugins.Misc.Brevo.PhoneType.Hint",
                ["Plugins.Misc.Sendinblue.SMS"] = "Plugins.Misc.Brevo.SMS",
                ["Plugins.Misc.Sendinblue.SMS.Campaigns"] = "Plugins.Misc.Brevo.SMS.Campaigns",
                ["Plugins.Misc.Sendinblue.SMS.Campaigns.Sent"] = "Plugins.Misc.Brevo.SMS.Campaigns.Sent",
                ["Plugins.Misc.Sendinblue.SMS.Campaigns.Submit"] = "Plugins.Misc.Brevo.SMS.Campaigns.Submit",
                ["Plugins.Misc.Sendinblue.SMSText"] = "Plugins.Misc.Brevo.SMSText",
                ["Plugins.Misc.Sendinblue.SMSText.Hint"] = "Plugins.Misc.Brevo.SMSText.Hint",
                ["Plugins.Misc.Sendinblue.Synchronization"] = "Plugins.Misc.Brevo.Synchronization",
                ["Plugins.Misc.Sendinblue.Transactional"] = "Plugins.Misc.Brevo.Transactional",
                ["Plugins.Misc.Sendinblue.UseSendinblueTemplate"] = "Plugins.Misc.Brevo.UseBrevoTemplate",
            }, languages, _localizationService);

            //add, update and delete localization resources
            _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Misc.Brevo.ActivateSMTP"] = "On your Brevo account, the SMTP has not been enabled yet. To request its activation, simply send an email to our support team at contact@brevo.com and mention that you will be using the SMTP with the nopCommerce plugin.",
                ["Plugins.Misc.Brevo.Fields.ApiKey.Hint"] = "Paste your Brevo account API v3 key.",
                ["Plugins.Misc.Brevo.Fields.List.Hint"] = "Select the Brevo list where your nopCommerce newsletter subscribers will be added.",
                ["Plugins.Misc.Brevo.Fields.TrackingScript.Hint"] = $"Paste the tracking script generated by Brevo here. {BrevoDefaults.TrackingScriptId} and {BrevoDefaults.TrackingScriptCustomerEmail} will be dynamically replaced.",
                ["Plugins.Misc.Brevo.Fields.UseMarketingAutomation.Hint"] = "Check for enable Brevo Automation.",
                ["Plugins.Misc.Brevo.Fields.UseSmtp"] = "Use Brevo SMTP",
                ["Plugins.Misc.Brevo.Fields.UseSmtp.Hint"] = "Check for using Brevo SMTP for sending transactional emails.",
                ["Plugins.Misc.Brevo.UseBrevoTemplate"] = "Brevo template",
            }, languageId);

            #endregion

            #region settings

            var sendinblueSettings = _dataProvider.GetTable<Setting>().Where(x => x.Name.StartsWith("sendinbluesettings.")).ToList();
            if (sendinblueSettings.Any())
            { 
                foreach (var setting in sendinblueSettings)
                {
                    setting.Name = setting.Name.Replace("sendinbluesettings", "brevosettings");
                }
                _dataProvider.UpdateEntities(sendinblueSettings);
            }

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(BrevoDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(BrevoDefaults.SystemName);
                _settingService.SaveSetting(_widgetSettings);
            }

            #endregion

            #region schedule task

            var sendinblueTask = _dataProvider.GetTable<ScheduleTask>().FirstOrDefault(x => x.Type == "Nop.Plugin.Misc.Sendinblue.Services.SynchronizationTask");
            if (sendinblueTask != null)
            {
                sendinblueTask.Type = BrevoDefaults.SynchronizationTask;
                sendinblueTask.Name = BrevoDefaults.SynchronizationTaskName;

                _dataProvider.UpdateEntity(sendinblueTask);
            }

            #endregion
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }

        #endregion
    }
}
