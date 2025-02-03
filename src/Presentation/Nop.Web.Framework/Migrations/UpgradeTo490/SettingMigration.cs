﻿using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopUpdateMigration("2024-12-01 00:00:00", "4.90", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var settingService = EngineContext.Current.Resolve<ISettingService>();

        //#6590
        var adminAreaSettings = settingService.LoadSetting<AdminAreaSettings>();
        if (!settingService.SettingExists(adminAreaSettings, settings => settings.UseStickyHeaderLayout))
        {
            adminAreaSettings.UseStickyHeaderLayout = false;
            settingService.SaveSetting(adminAreaSettings, settings => settings.UseStickyHeaderLayout);
        }

        //#7387
        var productEditorSettings = settingService.LoadSetting<ProductEditorSettings>();
        if (!settingService.SettingExists(productEditorSettings, settings => settings.AgeVerification))
        {
            productEditorSettings.AgeVerification = false;
            settingService.SaveSetting(productEditorSettings, settings => settings.AgeVerification);
        }

        //#2184
        var vendorSettings = settingService.LoadSetting<VendorSettings>();
        if (!settingService.SettingExists(vendorSettings, settings => settings.MaximumProductPicturesNumber))
        {
            vendorSettings.MaximumProductPicturesNumber = 5;
            settingService.SaveSetting(vendorSettings, settings => settings.MaximumProductPicturesNumber);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
