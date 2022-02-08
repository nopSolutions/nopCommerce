using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Core.Domain;

namespace Nop.Web.Framework.Migrations.UpgradeTo460
{
    [NopMigration("2022-02-08 00:00:00", "4.60.0", UpdateMigrationType.Settings, MigrationProcessType.Update)]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            var storeInformationSettings = settingService.LoadSettingAsync<StoreInformationSettings>().Result;

            //#3997
            if (!settingService.SettingExistsAsync(storeInformationSettings, settings => settings.InstagramLink).Result)
            {
                storeInformationSettings.InstagramLink = "https://www.instagram.com/nopcommerce_official";
                settingService.SaveSettingAsync(storeInformationSettings, settings => settings.InstagramLink).Wait();
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}