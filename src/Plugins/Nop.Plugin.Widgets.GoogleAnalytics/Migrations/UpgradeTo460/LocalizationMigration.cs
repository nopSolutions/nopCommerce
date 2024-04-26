using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Migrations.UpgradeTo460;

[NopMigration("2022-04-05 17:00:00", "Widgets.GoogleAnalytics 1.74. Update localizations", MigrationProcessType.Update)]
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
            ["Plugins.Widgets.GoogleAnalytics.Instructions"] = "<p>Google Analytics is a free website stats tool from Google. It keeps track of statistics about the visitors and eCommerce conversion on your website.<br /><br />Follow the next steps to enable Google Analytics integration:<br /><ul><li><a href=\"http://www.google.com/analytics/\" target=\"_blank\">Create a Google Analytics account</a> and follow the wizard to add your website</li><li>Copy the Tracking ID into the 'ID' box below</li><li>Click the 'Save' button below and Google Analytics will be integrated into your store</li></ul><br /></p>"

        }, languageId);
    }
}