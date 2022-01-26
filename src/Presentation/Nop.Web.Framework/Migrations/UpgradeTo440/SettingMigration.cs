using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Seo;

namespace Nop.Web.Framework.Migrations.UpgradeTo440
{
    [NopMigration("2020-06-10 00:00:00", "4.40.0", UpdateMigrationType.Settings, MigrationProcessType.Update)]
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

            //#4904 External authentication errors logging
            var externalAuthenticationSettings = settingService.LoadSettingAsync<ExternalAuthenticationSettings>().Result;
            if (!settingService.SettingExistsAsync(externalAuthenticationSettings, settings => settings.LogErrors).Result)
            {
                externalAuthenticationSettings.LogErrors = false;
                settingService.SaveSettingAsync(externalAuthenticationSettings, settings => settings.LogErrors).Wait();
            }

            var multiFactorAuthenticationSettings = settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>().Result;
            if (!settingService.SettingExistsAsync(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication).Result)
            {
                multiFactorAuthenticationSettings.ForceMultifactorAuthentication = false;

                settingService.SaveSettingAsync(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication).Wait();
            }

            //#5102 Delete Full-text settings
            settingRepository
                .DeleteAsync(setting => setting.Name == "commonsettings.usefulltextsearch" || setting.Name == "commonsettings.fulltextmode")
                .Wait();

            //#4196
            settingRepository
                .DeleteAsync(setting => setting.Name == "commonsettings.scheduletaskruntimeout" ||
                    setting.Name == "commonsettings.staticfilescachecontrol" ||
                    setting.Name == "commonsettings.supportpreviousnopcommerceversions" ||
                    setting.Name == "securitysettings.pluginstaticfileextensionsBlacklist")
                .Wait();

            //#5384
            var seoSettings = settingService.LoadSettingAsync<SeoSettings>().Result;
            foreach (var slug in NopSeoDefaults.ReservedUrlRecordSlugs)
            {
                if (!seoSettings.ReservedUrlRecordSlugs.Contains(slug))
                    seoSettings.ReservedUrlRecordSlugs.Add(slug);
            }
            settingService.SaveSettingAsync(seoSettings, settings => seoSettings.ReservedUrlRecordSlugs).Wait();

            //#3015
            if (!settingService.SettingExistsAsync(seoSettings, settings => settings.HomepageTitle).Result)
            {
                seoSettings.HomepageTitle = seoSettings.DefaultTitle;
                settingService.SaveSettingAsync(seoSettings, settings => settings.HomepageTitle).Wait();
            }

            if (!settingService.SettingExistsAsync(seoSettings, settings => settings.HomepageDescription).Result)
            {
                seoSettings.HomepageDescription = "Your home page description";
                settingService.SaveSettingAsync(seoSettings, settings => settings.HomepageDescription).Wait();
            }

            //#5210
            var adminAreaSettings = settingService.LoadSettingAsync<AdminAreaSettings>().Result;
            if (!settingService.SettingExistsAsync(adminAreaSettings, settings => settings.ShowDocumentationReferenceLinks).Result)
            {
                adminAreaSettings.ShowDocumentationReferenceLinks = true;
                settingService.SaveSettingAsync(adminAreaSettings, settings => settings.ShowDocumentationReferenceLinks).Wait();
            }

            //#4944
            var shippingSettings = settingService.LoadSettingAsync<ShippingSettings>().Result;
            if (!settingService.SettingExistsAsync(shippingSettings, settings => settings.RequestDelay).Result)
            {
                shippingSettings.RequestDelay = 300;
                settingService.SaveSettingAsync(shippingSettings, settings => settings.RequestDelay).Wait();
            }

            //#276 AJAX filters
            var catalogSettings = settingService.LoadSettingAsync<CatalogSettings>().Result;
            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.UseAjaxCatalogProductsLoading).Result)
            {
                catalogSettings.UseAjaxCatalogProductsLoading = true;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.UseAjaxCatalogProductsLoading).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.EnableManufacturerFiltering).Result)
            {
                catalogSettings.EnableManufacturerFiltering = true;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.EnableManufacturerFiltering).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.EnablePriceRangeFiltering).Result)
            {
                catalogSettings.EnablePriceRangeFiltering = true;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.EnablePriceRangeFiltering).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.SearchPagePriceRangeFiltering).Result)
            {
                catalogSettings.SearchPagePriceRangeFiltering = true;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.SearchPagePriceRangeFiltering).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.SearchPagePriceFrom).Result)
            {
                catalogSettings.SearchPagePriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.SearchPagePriceFrom).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.SearchPagePriceTo).Result)
            {
                catalogSettings.SearchPagePriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.SearchPagePriceTo).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.SearchPageManuallyPriceRange).Result)
            {
                catalogSettings.SearchPageManuallyPriceRange = false;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.SearchPageManuallyPriceRange).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.ProductsByTagPriceRangeFiltering).Result)
            {
                catalogSettings.ProductsByTagPriceRangeFiltering = true;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.ProductsByTagPriceRangeFiltering).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.ProductsByTagPriceFrom).Result)
            {
                catalogSettings.ProductsByTagPriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.ProductsByTagPriceFrom).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.ProductsByTagPriceTo).Result)
            {
                catalogSettings.ProductsByTagPriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.ProductsByTagPriceTo).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.ProductsByTagManuallyPriceRange).Result)
            {
                catalogSettings.ProductsByTagManuallyPriceRange = false;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.ProductsByTagManuallyPriceRange).Wait();
            }

            //#4303
            var orderSettings = settingService.LoadSettingAsync<OrderSettings>().Result;
            if (!settingService.SettingExistsAsync(orderSettings, settings => settings.DisplayCustomerCurrencyOnOrders).Result)
            {
                orderSettings.DisplayCustomerCurrencyOnOrders = false;
                settingService.SaveSettingAsync(orderSettings, settings => settings.DisplayCustomerCurrencyOnOrders).Wait();
            }

            //#16 #2909
            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.AttributeValueOutOfStockDisplayType).Result)
            {
                catalogSettings.AttributeValueOutOfStockDisplayType = AttributeValueOutOfStockDisplayType.AlwaysDisplay;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.AttributeValueOutOfStockDisplayType).Wait();
            }

            //#5482
            settingService.SetSettingAsync("avalarataxsettings.gettaxratebyaddressonly", true).Wait();
            settingService.SetSettingAsync("avalarataxsettings.taxratebyaddresscachetime", 480).Wait();

            //#5349
            if (!settingService.SettingExistsAsync(shippingSettings, settings => settings.EstimateShippingCityNameEnabled).Result)
            {
                shippingSettings.EstimateShippingCityNameEnabled = false;
                settingService.SaveSettingAsync(shippingSettings, settings => settings.EstimateShippingCityNameEnabled).Wait();
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}