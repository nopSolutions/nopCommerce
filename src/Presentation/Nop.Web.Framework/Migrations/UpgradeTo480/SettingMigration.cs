using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo480;

[NopUpdateMigration("2024-05-15 01:00:00", "4.80", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var settingService = EngineContext.Current.Resolve<ISettingService>();

        //#7215
        var displayAttributeCombinationImagesOnly = settingService.GetSetting("producteditorsettings.displayattributecombinationimagesonly");
        if (displayAttributeCombinationImagesOnly is not null)
            settingService.DeleteSetting(displayAttributeCombinationImagesOnly);

        //#7228
        var adminAreaSettings = settingService.LoadSetting<AdminAreaSettings>();
        if (!settingService.SettingExists(adminAreaSettings, settings => settings.ProductsBulkEditGridPageSize))
        {
            adminAreaSettings.ProductsBulkEditGridPageSize = 100;
            settingService.SaveSetting(adminAreaSettings, settings => settings.ProductsBulkEditGridPageSize);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}