using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Migrations;

[NopMigration("2023-08-17 15:00:00", "Shipping.FixedByWeightByTotal add LoadAllRecord setting", MigrationProcessType.Update)]
public class AddLoadAllRecordSetting : Migration
{
    public override void Up()
    {
        //do not use DI, because it produces exception on the installation process
        var settingService = EngineContext.Current.Resolve<ISettingService>();

        var pluginSettings = settingService.LoadSetting<FixedByWeightByTotalSettings>();

        if (!settingService.SettingExists(pluginSettings, settings => settings.LoadAllRecord))
        {
            pluginSettings.LoadAllRecord = true;
            settingService.SaveSetting(pluginSettings, settings => settings.LoadAllRecord);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}