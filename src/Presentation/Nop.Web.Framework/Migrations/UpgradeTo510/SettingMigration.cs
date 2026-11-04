using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;

namespace Nop.Web.Framework.Migrations.UpgradeTo510;

[NopUpdateMigration("2026-10-28 00:00:00", "5.10", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
