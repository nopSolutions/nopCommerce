using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo480;

[NopUpdateMigration("2024-05-15 00:00:00", "4.80.0", UpdateMigrationType.Settings)]
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

        //#7325
        var orderSettings = settingService.LoadSetting<OrderSettings>();
        if (!settingService.SettingExists(orderSettings, settings => settings.PlaceOrderWithLock))
        {
            orderSettings.PlaceOrderWithLock = false;
            settingService.SaveSetting(orderSettings, settings => settings.PlaceOrderWithLock);
        }

        if (!settingService.SettingExists(orderSettings, settings => settings.PlaceOrderWithLockInterval))
        {
            orderSettings.PlaceOrderWithLockInterval = 1;
            settingService.SaveSetting(orderSettings, settings => settings.PlaceOrderWithLockInterval);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}