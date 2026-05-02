using FluentMigrator;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopMigration("2026-03-18 00:00:00", "Pseudo-migration to update appSettings.json file", MigrationProcessType.Update)]
public class AppSettingsMigration : MigrationBase
{
    public override void Up()
    {
        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

        var dataConfig = DataSettingsManager.LoadSettings();
        dataConfig.Collation = string.Empty;
        dataConfig.CharacterSet = string.Empty;
        dataConfig.CloseDataContextAfterUse = true;
        dataConfig.BulkCopyWithCheckConstraints = true;

        AppSettingsHelper.SaveAppSettings(new List<IConfig> { dataConfig }, fileProvider);
    }

    public override void Down() { }
}