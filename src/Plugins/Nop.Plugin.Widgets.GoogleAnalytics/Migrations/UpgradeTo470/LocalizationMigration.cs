using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Migrations.UpgradeTo470;

[NopMigration("2023-03-01 17:00:00", "Widgets.GoogleAnalytics 2.00. Update localizations", MigrationProcessType.Update)]
public class LocalizationMigration : MigrationBase
{
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }

    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        var (languageId, _) = this.GetLanguageData();

        //use localizationService to add, update and delete localization resources
        localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Widgets.GoogleAnalytics.UseSandbox"] = "UseSandbox",
            ["Plugins.Widgets.GoogleAnalytics.UseSandbox.Hint"] = "Determine whether to use the sandbox environment for testing purposes. This setting only applies to sending eCommerce information via the Measurement Protocol.",
            ["Plugins.Widgets.GoogleAnalytics.ApiSecret"] = "API Secret",
            ["Plugins.Widgets.GoogleAnalytics.ApiSecret.Hint"] = "Enter API Secret.",
            ["Plugins.Widgets.GoogleAnalytics.Instructions"] = "<p>Google Analytics is a free website stats tool from Google. It keeps track of statistics about the visitors and eCommerce conversion on your website.<br /><br />Follow the next steps to enable Google Analytics integration:<br /><ul><li><a href=\"http://www.google.com/analytics/\" target=\"_blank\">Create a Google Analytics account</a> and follow the wizard to add your website</li><li>Copy the <b>MEASUREMENT ID</b> into the <b>ID</b> box below</li><li>In Google Analytics click on the <b>Measurement Protocol API secrets</b> under <b>Events</b></li><li>Click on <b>Create</b> button and follow the instructions to create a new API secret</li><li>Copy the API secret into the <b>API Secret</b> box below</li><li>Click the 'Save' button below and Google Analytics will be integrated into your store</li></ul><br /></p>"

        }, languageId);
    }
}