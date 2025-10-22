using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
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

        //do not use DI, because it produces exception on the installation process
        var dataProvider = EngineContext.Current.Resolve<INopDataProvider>();

        //#4904 External authentication errors logging
        var externalAuthenticationSettings = this.LoadSetting<ExternalAuthenticationSettings>();
        if (!this.SettingExists(externalAuthenticationSettings, settings => settings.LogErrors))
        {
            externalAuthenticationSettings.LogErrors = false;
            this.SaveSetting(externalAuthenticationSettings, settings => settings.LogErrors);
        }

        var multiFactorAuthenticationSettings = this.LoadSetting<MultiFactorAuthenticationSettings>();
        if (!this.SettingExists(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication))
        {
            multiFactorAuthenticationSettings.ForceMultifactorAuthentication = false;

            this.SaveSetting(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication);
        }

        //#5102 Delete Full-text settings
        dataProvider.BulkDeleteEntities<Setting>(setting => setting.Name == "commonsettings.usefulltextsearch" || setting.Name == "commonsettings.fulltextmode");

        //#4196
        dataProvider.BulkDeleteEntities<Setting>(setting => setting.Name == "commonsettings.scheduletaskruntimeout" ||
            setting.Name == "commonsettings.staticfilescachecontrol" ||
            setting.Name == "commonsettings.supportpreviousnopcommerceversions" ||
            setting.Name == "securitysettings.pluginstaticfileextensionsBlacklist");

        //#5384
        var seoSettings = this.LoadSetting<SeoSettings>();
        foreach (var slug in NopSeoDefaults.ReservedUrlRecordSlugs)
        {
            if (!seoSettings.ReservedUrlRecordSlugs.Contains(slug))
                seoSettings.ReservedUrlRecordSlugs.Add(slug);
        }
        this.SaveSetting(seoSettings, settings => seoSettings.ReservedUrlRecordSlugs);

        //#3015
        var homepageTitleKey = $"{nameof(SeoSettings)}.HomepageTitle".ToLower();
        if (this.GetSettingByKey<string>(homepageTitleKey) == null)
            this.SetSetting(homepageTitleKey, this.GetSettingByKey<string>($"{nameof(SeoSettings)}.DefaultTitle"));

        var homepageDescriptionKey = $"{nameof(SeoSettings)}.HomepageDescription".ToLower();
        if (this.GetSettingByKey<string>(homepageDescriptionKey) == null)
            this.SetSetting(homepageDescriptionKey, "Your home page description");

        //#5210
        var adminAreaSettings = this.LoadSetting<AdminAreaSettings>();
        if (!this.SettingExists(adminAreaSettings, settings => settings.ShowDocumentationReferenceLinks))
        {
            adminAreaSettings.ShowDocumentationReferenceLinks = true;
            this.SaveSetting(adminAreaSettings, settings => settings.ShowDocumentationReferenceLinks);
        }

        //#4944
        var shippingSettings = this.LoadSetting<ShippingSettings>();
        if (!this.SettingExists(shippingSettings, settings => settings.RequestDelay))
        {
            shippingSettings.RequestDelay = 300;
            this.SaveSetting(shippingSettings, settings => settings.RequestDelay);
        }

        //#276 AJAX filters
        var catalogSettings = this.LoadSetting<CatalogSettings>();
        if (!this.SettingExists(catalogSettings, settings => settings.UseAjaxCatalogProductsLoading))
        {
            catalogSettings.UseAjaxCatalogProductsLoading = true;
            this.SaveSetting(catalogSettings, settings => settings.UseAjaxCatalogProductsLoading);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.EnableManufacturerFiltering))
        {
            catalogSettings.EnableManufacturerFiltering = true;
            this.SaveSetting(catalogSettings, settings => settings.EnableManufacturerFiltering);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.EnablePriceRangeFiltering))
        {
            catalogSettings.EnablePriceRangeFiltering = true;
            this.SaveSetting(catalogSettings, settings => settings.EnablePriceRangeFiltering);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.SearchPagePriceRangeFiltering))
        {
            catalogSettings.SearchPagePriceRangeFiltering = true;
            this.SaveSetting(catalogSettings, settings => settings.SearchPagePriceRangeFiltering);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.SearchPagePriceFrom))
        {
            catalogSettings.SearchPagePriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
            this.SaveSetting(catalogSettings, settings => settings.SearchPagePriceFrom);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.SearchPagePriceTo))
        {
            catalogSettings.SearchPagePriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
            this.SaveSetting(catalogSettings, settings => settings.SearchPagePriceTo);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.SearchPageManuallyPriceRange))
        {
            catalogSettings.SearchPageManuallyPriceRange = false;
            this.SaveSetting(catalogSettings, settings => settings.SearchPageManuallyPriceRange);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.ProductsByTagPriceRangeFiltering))
        {
            catalogSettings.ProductsByTagPriceRangeFiltering = true;
            this.SaveSetting(catalogSettings, settings => settings.ProductsByTagPriceRangeFiltering);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.ProductsByTagPriceFrom))
        {
            catalogSettings.ProductsByTagPriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
            this.SaveSetting(catalogSettings, settings => settings.ProductsByTagPriceFrom);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.ProductsByTagPriceTo))
        {
            catalogSettings.ProductsByTagPriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
            this.SaveSetting(catalogSettings, settings => settings.ProductsByTagPriceTo);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.ProductsByTagManuallyPriceRange))
        {
            catalogSettings.ProductsByTagManuallyPriceRange = false;
            this.SaveSetting(catalogSettings, settings => settings.ProductsByTagManuallyPriceRange);
        }

        //#4303
        var orderSettings = this.LoadSetting<OrderSettings>();
        if (!this.SettingExists(orderSettings, settings => settings.DisplayCustomerCurrencyOnOrders))
        {
            orderSettings.DisplayCustomerCurrencyOnOrders = false;
            this.SaveSetting(orderSettings, settings => settings.DisplayCustomerCurrencyOnOrders);
        }

        //#16 #2909
        if (!this.SettingExists(catalogSettings, settings => settings.AttributeValueOutOfStockDisplayType))
        {
            catalogSettings.AttributeValueOutOfStockDisplayType = AttributeValueOutOfStockDisplayType.AlwaysDisplay;
            this.SaveSetting(catalogSettings, settings => settings.AttributeValueOutOfStockDisplayType);
        }

        //#5482
        this.SetSetting("avalarataxsettings.gettaxratebyaddressonly", true);
        this.SetSetting("avalarataxsettings.taxratebyaddresscachetime", 480);

        //#5349
        if (!this.SettingExists(shippingSettings, settings => settings.EstimateShippingCityNameEnabled))
        {
            shippingSettings.EstimateShippingCityNameEnabled = false;
            this.SaveSetting(shippingSettings, settings => settings.EstimateShippingCityNameEnabled);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}