using FluentMigrator;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopUpdateMigration("2025-10-28 00:00:00", "5.00", UpdateMigrationType.Localization)]
public class LocalizationMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        var (languageId, languages) = this.GetLanguageData();

        #region Delete locales

        localizationService.DeleteLocaleResources(new List<string>
        {
            
        });

        #endregion

        #region Rename locales

        this.RenameLocales(new Dictionary<string, string>
        {
            
        }, languages, localizationService);
        
        #endregion

        #region Add or update locales

        localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            //#7898
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.LogRequests"] = "Log AI requests",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.LogRequests.Hint"] = "Check to enable logging of all requests to AI services.",

            //#7743
            ["Admin.Promotions.Reminder.AbandonedCartEnabled"] = "Abandoned сart reminders enabled",
            ["Admin.Promotions.Reminder.AbandonedCartEnabled.Hint"] = "Check to enable abandoned сart reminders.",
            ["Admin.Promotions.Reminder.FollowUp.DelayBeforeSend"] = "Delay send",
            ["Admin.Promotions.Reminder.FollowUp.DelayBeforeSend.Hint"] = "A delay before sending the follow up.",
            ["Admin.Promotions.Reminder.FollowUp.DelayBeforeSend.MustBeGreaterThanZero"] = "The delay must be greater than '0'.",
            ["Admin.Promotions.Reminder.FollowUp.DelayBeforeSend.Required"] = "The delay is required.",
            ["Admin.Promotions.Reminder.FollowUp.Enabled"] = "Follow up #{0}",
            ["Admin.Promotions.Reminder.FollowUp.Enabled.Hint"] = "Check to enable reminder.",
            ["Admin.Promotions.Reminder.IncompleteRegistrationEnabled"] = "Incomplete registration reminder enabled",
            ["Admin.Promotions.Reminder.IncompleteRegistrationEnabled.Hint"] = "Check to enable incomplete registration reminders.",
            ["Admin.Promotions.Reminder.PendingOrdersEnabled"] = "Pending orders reminders enabled",
            ["Admin.Promotions.Reminder.PendingOrdersEnabled.Hint"] = "Check to enable pending orders reminders.",
            ["Admin.Promotions.Reminders"] = "Reminders",
            ["Admin.Promotions.Reminders.Warning.TaskDisabled"] = "Please remember <a href=\"{0}\" target=\"_blank\">to enable</a> the \"{1}\" scheduled task.",
            ["Admin.Promotions.Reminders.Warning.NotFound"] = "The scheduled task not found.",
            [$"Admin.ContentManagement.MessageTemplates.Description.{MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_1_MESSAGE}"] = "This message template is used to send the follow-up #1 for a abandoned cart.",
            [$"Admin.ContentManagement.MessageTemplates.Description.{MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_2_MESSAGE}"] = "This message template is used to send the follow-up #2 for a abandoned cart.",
            [$"Admin.ContentManagement.MessageTemplates.Description.{MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_3_MESSAGE}"] = "This message template is used to send the follow-up #3 for a abandoned cart.",
            [$"Admin.ContentManagement.MessageTemplates.Description.{MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_1_MESSAGE}"] = "This message template is used to send the follow-up #1 for a pending order.",
            [$"Admin.ContentManagement.MessageTemplates.Description.{MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_2_MESSAGE}"] = "This message template is used to send the follow-up #2 for a pending order.",
            [$"Admin.ContentManagement.MessageTemplates.Description.{MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE}"] = "This message template is used to send the follow-up #1 for a incomplete registration.",


        }, languageId);

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
