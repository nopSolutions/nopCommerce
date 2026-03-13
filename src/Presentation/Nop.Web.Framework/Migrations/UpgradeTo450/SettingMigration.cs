using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
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

        //mini profiler settings are moved to appSettings
        this.DeleteSettingsByNames(["storeinformationsettings.displayminiprofilerforadminonly", "storeinformationsettings.displayminiprofilerinpublicstore"]);

        //#4363
        this.SetSettingIfNotExists<CommonSettings, int>(settings => settings.ClearLogOlderThanDays, 0);

        //#5551
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.EnableSpecificationAttributeFiltering, true);

        //#5204
        this.SetSettingIfNotExists<ShippingSettings, ShippingSortingEnum>(settings => settings.ShippingSorting, ShippingSortingEnum.Position);

        //#5698
        this.SetSettingIfNotExists<OrderSettings, bool>(settings => settings.DisplayOrderSummary, true);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}