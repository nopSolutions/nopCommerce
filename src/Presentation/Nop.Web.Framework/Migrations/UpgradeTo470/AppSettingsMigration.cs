using FluentMigrator;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;

namespace Nop.Web.Framework.Migrations.UpgradeTo470;

[NopMigration("2023-11-17 00:00:00", "Pseudo-migration to update appSettings.json file", MigrationProcessType.Update)]
public class AppSettingsMigration : MigrationBase
{

    public override void Up()
    {
        var dataConfig = DataSettingsManager.LoadSettings();

        dataConfig.WithNoLock = false;

        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
        AppSettingsHelper.SaveAppSettings(new List<IConfig> { dataConfig }, fileProvider);
    }

    public override void Down() { }
}