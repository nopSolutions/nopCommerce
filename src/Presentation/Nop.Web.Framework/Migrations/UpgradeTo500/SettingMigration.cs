using FluentMigrator;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopUpdateMigration("2025-10-27 00:00:00", "5.00", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var settingService = EngineContext.Current.Resolve<ISettingService>();
        
        //#7898
        var aiSettings = settingService.LoadSetting<ArtificialIntelligenceSettings>();

        if (!settingService.SettingExists(aiSettings, settings => settings.LogRequests))
        {
            aiSettings.LogRequests = false;
            settingService.SaveSetting(aiSettings, settings => settings.LogRequests);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
