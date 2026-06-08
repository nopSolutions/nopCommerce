using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Migrations;

[NopMigration("2023-08-17 15:00:00", "Shipping.FixedByWeightByTotal add LoadAllRecord setting", MigrationProcessType.Update)]
public class AddLoadAllRecordSetting : Migration
{
    public override void Up()
    {
        this.SetSettingIfNotExists<FixedByWeightByTotalSettings, bool>(settings => settings.LoadAllRecord, true);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}