using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Migrations.UpgradeTo470;

[NopMigration("2024-04-19 12:00:00", "Tax.FixedOrByCountryStateZip 4.70.5. Update localizations", MigrationProcessType.Update)]
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
            ["Plugins.Tax.FixedOrByCountryStateZip.SwitchRate"] = @"
                    <p>
                        You are going to change the way the tax rate is calculated. This will cause the tax rate to be calculated based on the settings specified on the configuration page.
                    </p>
                    <p>
                        Any current tax rate settings will be saved, but will not be active until you return to this tax calculation method.
                    </p>",

        }, languageId);
    }
}
