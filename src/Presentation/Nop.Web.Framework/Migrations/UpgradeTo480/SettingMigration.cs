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
        this.DeleteSettingsByNames(["producteditorsettings.displayattributecombinationimagesonly"]);

        //#7325
        this.SetSettingIfNotExists<OrderSettings, bool>(settings => settings.PlaceOrderWithLock, false);

        //#7394
        this.SetSetting<OrderSettings, int>(settings => settings.MinimumOrderPlacementInterval, setting =>
        {
            switch (setting.MinimumOrderPlacementInterval)
            {
                case <= 10:
                    return;
                case < 60:
                    setting.MinimumOrderPlacementInterval = 1;
                    break;
                default:
                    setting.MinimumOrderPlacementInterval = Math.Truncate(setting.MinimumOrderPlacementInterval / 60.0) + (setting.MinimumOrderPlacementInterval % 60) == 0 ? 0 : 1;
                    break;
            }
        });

        //#7265
        this.SetSettingIfNotExists<TaxSettings, bool>(settings => settings.EuVatRequired, false);

        //#4306
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.ShowSearchBoxCategories, false);

        //#2388
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.ExportImportTierPrices, true);

        //#7228
        this.SetSettingIfNotExists<AdminAreaSettings, int>(settings => settings.ProductsBulkEditGridPageSize, 100);

        //#7244
        this.SetSettingIfNotExists<CatalogSettings, int>(settings => settings.VendorProductReviewsPageSize, 6);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}