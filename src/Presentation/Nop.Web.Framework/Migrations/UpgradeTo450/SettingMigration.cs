using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo450;

[NopUpdateMigration("2021-04-23 00:00:00", "4.50", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var dataProvider = EngineContext.Current.Resolve<INopDataProvider>();

        //miniprofiler settings are moved to appSettings
        dataProvider.BulkDeleteEntities<Setting>(setting => setting.Name == "storeinformationsettings.displayminiprofilerforadminonly" ||
                               setting.Name == "storeinformationsettings.displayminiprofilerinpublicstore");

        //#4363
        var commonSettings = this.LoadSetting<CommonSettings>();

        if (!this.SettingExists(commonSettings, settings => settings.ClearLogOlderThanDays))
        {
            commonSettings.ClearLogOlderThanDays = 0;
            this.SaveSetting(commonSettings, settings => settings.ClearLogOlderThanDays);
        }

        //#5551
        var catalogSettings = this.LoadSetting<CatalogSettings>();

        if (!this.SettingExists(catalogSettings, settings => settings.EnableSpecificationAttributeFiltering))
        {
            catalogSettings.EnableSpecificationAttributeFiltering = true;
            this.SaveSetting(catalogSettings, settings => settings.EnableSpecificationAttributeFiltering);
        }

        //#5204
        var shippingSettings = this.LoadSetting<ShippingSettings>();

        if (!this.SettingExists(shippingSettings, settings => settings.ShippingSorting))
        {
            shippingSettings.ShippingSorting = ShippingSortingEnum.Position;
            this.SaveSetting(shippingSettings, settings => settings.ShippingSorting);
        }

        //#5698
        var orderSettings = this.LoadSetting<OrderSettings>();
        if (!this.SettingExists(orderSettings, settings => settings.DisplayOrderSummary))
        {
            orderSettings.DisplayOrderSummary = true;
            this.SaveSetting(orderSettings, settings => settings.DisplayOrderSummary);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}