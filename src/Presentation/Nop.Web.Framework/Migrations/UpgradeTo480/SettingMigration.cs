using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo480;

[NopUpdateMigration("2024-11-01 00:00:00", "4.80", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //#7215
        var displayAttributeCombinationImagesOnly = this.GetSetting("producteditorsettings.displayattributecombinationimagesonly");
        if (displayAttributeCombinationImagesOnly is not null)
            this.DeleteSetting(displayAttributeCombinationImagesOnly);

        //#7325
        var orderSettings = this.LoadSetting<OrderSettings>();
        if (!this.SettingExists(orderSettings, settings => settings.PlaceOrderWithLock))
        {
            orderSettings.PlaceOrderWithLock = false;
            this.SaveSetting(orderSettings, settings => settings.PlaceOrderWithLock);
        }

        //#7394
        if (orderSettings.MinimumOrderPlacementInterval > 10)
        {
            if (orderSettings.MinimumOrderPlacementInterval < 60)
                orderSettings.MinimumOrderPlacementInterval = 1;
            else
                orderSettings.MinimumOrderPlacementInterval = Math.Truncate(orderSettings.MinimumOrderPlacementInterval / 60.0) + (orderSettings.MinimumOrderPlacementInterval % 60) == 0 ? 0 : 1;

            this.SaveSetting(orderSettings, settings => settings.MinimumOrderPlacementInterval);
        }

        //#7265
        var taxSetting = this.LoadSetting<TaxSettings>();
        if (!this.SettingExists(taxSetting, settings => settings.EuVatRequired))
        {
            taxSetting.EuVatRequired = false;
            this.SaveSetting(taxSetting, settings => settings.EuVatRequired);
        }

        //#4306
        var catalogSettings = this.LoadSetting<CatalogSettings>();
        if (!this.SettingExists(catalogSettings, settings => settings.ShowSearchBoxCategories))
        {
            catalogSettings.ShowSearchBoxCategories = false;
            this.SaveSetting(catalogSettings, settings => settings.ShowSearchBoxCategories);
        }

        //#2388
        if (!this.SettingExists(catalogSettings, settings => settings.ExportImportTierPrices))
        {
            catalogSettings.ExportImportTierPrices = true;
            this.SaveSetting(catalogSettings, settings => settings.ExportImportTierPrices);
        }

        //#7228
        var adminAreaSettings = this.LoadSetting<AdminAreaSettings>();
        if (!this.SettingExists(adminAreaSettings, settings => settings.ProductsBulkEditGridPageSize))
        {
            adminAreaSettings.ProductsBulkEditGridPageSize = 100;
            this.SaveSetting(adminAreaSettings, settings => settings.ProductsBulkEditGridPageSize);
        }

        //#7244
        if (!this.SettingExists(catalogSettings, settings => settings.VendorProductReviewsPageSize))
        {
            catalogSettings.VendorProductReviewsPageSize = 6;
            this.SaveSetting(catalogSettings, settings => settings.VendorProductReviewsPageSize);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}