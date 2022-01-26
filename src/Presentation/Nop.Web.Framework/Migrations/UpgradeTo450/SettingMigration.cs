using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Core.Domain.Shipping;

namespace Nop.Web.Framework.Migrations.UpgradeTo450
{
    [NopMigration("2021-04-23 00:00:00", "4.50.0", UpdateMigrationType.Settings, MigrationProcessType.Update)]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            //miniprofiler settings are moved to appSettings
            settingRepository
                .DeleteAsync(setting => setting.Name == "storeinformationsettings.displayminiprofilerforadminonly" ||
                    setting.Name == "storeinformationsettings.displayminiprofilerinpublicstore").Wait();

            //#4363
            var commonSettings = settingService.LoadSettingAsync<CommonSettings>().Result;

            if (!settingService.SettingExistsAsync(commonSettings, settings => settings.ClearLogOlderThanDays).Result)
            {
                commonSettings.ClearLogOlderThanDays = 0;
                settingService.SaveSettingAsync(commonSettings, settings => settings.ClearLogOlderThanDays).Wait();
            }

            //#5551
            var catalogSettings = settingService.LoadSettingAsync<CatalogSettings>().Result;

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.EnableSpecificationAttributeFiltering).Result)
            {
                catalogSettings.EnableSpecificationAttributeFiltering = true;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.EnableSpecificationAttributeFiltering).Wait();
            }

            //#5204
            var shippingSettings = settingService.LoadSettingAsync<ShippingSettings>().Result;

            if (!settingService.SettingExistsAsync(shippingSettings, settings => settings.ShippingSorting).Result)
            {
                shippingSettings.ShippingSorting = ShippingSortingEnum.Position;
                settingService.SaveSettingAsync(shippingSettings, settings => settings.ShippingSorting).Wait();
            }

            //#5698
            var orderSettings = settingService.LoadSettingAsync<OrderSettings>().Result;
            if (!settingService.SettingExistsAsync(orderSettings, settings => settings.DisplayOrderSummary).Result)
            {
                orderSettings.DisplayOrderSummary = true;
                settingService.SaveSettingAsync(orderSettings, settings => settings.DisplayOrderSummary).Wait();
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}