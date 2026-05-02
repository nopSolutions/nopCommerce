using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Catalog;
using Nop.Services.Seo;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo440;

[NopUpdateMigration("2020-06-10 00:00:00", "4.40", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //#4904 External authentication errors logging
        this.SetSettingIfNotExists<ExternalAuthenticationSettings, bool>(settings => settings.LogErrors, false);
        this.SetSettingIfNotExists<MultiFactorAuthenticationSettings, bool>(settings => settings.ForceMultifactorAuthentication, false);

        //#5102 Delete Full-text settings
        this.DeleteSettingsByNames(["commonsettings.usefulltextsearch", "commonsettings.fulltextmode"]);

        //#4196
        this.DeleteSettingsByNames([
            "commonsettings.scheduletaskruntimeout", "commonsettings.staticfilescachecontrol",
            "commonsettings.supportpreviousnopcommerceversions", "securitysettings.pluginstaticfileextensionsBlacklist"
        ]);

        //#5384
        this.SetSetting<SeoSettings, List<string>>(settings => settings.ReservedUrlRecordSlugs, setting =>
        {
            foreach (var slug in NopSeoDefaults.ReservedUrlRecordSlugs.Where(slug => !setting.ReservedUrlRecordSlugs.Contains(slug)).ToList())
                setting.ReservedUrlRecordSlugs.Add(slug);
        });

        //#3015
        this.SetSettingIfNotExists($"{nameof(SeoSettings)}.HomepageTitle", this.GetSettingByKey<string>($"{nameof(SeoSettings)}.DefaultTitle"));
        this.SetSettingIfNotExists($"{nameof(SeoSettings)}.HomepageDescription", "Your home page description");

        //#5210
        this.SetSettingIfNotExists<AdminAreaSettings, bool>(settings => settings.ShowDocumentationReferenceLinks, true);

        //#4944
        this.SetSettingIfNotExists<ShippingSettings, int>(settings => settings.RequestDelay, 300);

        //#276 AJAX filters
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.UseAjaxCatalogProductsLoading, true);
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.EnableManufacturerFiltering, true);
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.EnablePriceRangeFiltering, true);
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.SearchPagePriceRangeFiltering, true);
        this.SetSettingIfNotExists<CatalogSettings, decimal>(settings => settings.SearchPagePriceFrom, NopCatalogDefaults.DefaultPriceRangeFrom);
        this.SetSettingIfNotExists<CatalogSettings, decimal>(settings => settings.SearchPagePriceTo, NopCatalogDefaults.DefaultPriceRangeTo);
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.SearchPageManuallyPriceRange, false);
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.ProductsByTagPriceRangeFiltering, true);
        this.SetSettingIfNotExists<CatalogSettings, decimal>(settings => settings.ProductsByTagPriceFrom, NopCatalogDefaults.DefaultPriceRangeFrom);
        this.SetSettingIfNotExists<CatalogSettings, decimal>(settings => settings.ProductsByTagPriceTo, NopCatalogDefaults.DefaultPriceRangeTo);
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.ProductsByTagManuallyPriceRange, false);

        //#4303
        this.SetSettingIfNotExists<OrderSettings, bool>(settings => settings.DisplayCustomerCurrencyOnOrders, false);

        //#16 #2909
        this.SetSettingIfNotExists<CatalogSettings, AttributeValueOutOfStockDisplayType>(settings => settings.AttributeValueOutOfStockDisplayType, AttributeValueOutOfStockDisplayType.AlwaysDisplay);

        //#5482
        this.SetSettingIfNotExists("avalarataxsettings.gettaxratebyaddressonly", true);
        this.SetSettingIfNotExists("avalarataxsettings.taxratebyaddresscachetime", 480);

        //#5349
        this.SetSettingIfNotExists<ShippingSettings, bool>(settings => settings.EstimateShippingCityNameEnabled, false);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}