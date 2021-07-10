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
using Nop.Core;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Migrations.UpgradeTo440
{
    [NopMigration("2020-06-10 00:00:00", "4.40.0", UpdateMigrationType.Settings)]
    [SkipMigrationOnInstall]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            AsyncHelper.RunSync(() => UpAsync());
        }

        protected virtual async Task UpAsync()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            //#4904 External authentication errors logging
            var externalAuthenticationSettings = await settingService.LoadSettingAsync<ExternalAuthenticationSettings>();
            if (!await settingService.SettingExistsAsync(externalAuthenticationSettings, settings => settings.LogErrors))
            {
                externalAuthenticationSettings.LogErrors = false;
                await settingService.SaveSettingAsync(externalAuthenticationSettings);
            }

            var multiFactorAuthenticationSettings = await settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>();
            if (!await settingService.SettingExistsAsync(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication))
            {
                multiFactorAuthenticationSettings.ForceMultifactorAuthentication = false;

                await settingService.SaveSettingAsync(multiFactorAuthenticationSettings);
            }

            //#5102 Delete Full-text settings
            await settingRepository
                .DeleteAsync(setting => setting.Name == "commonsettings.usefulltextsearch" || setting.Name == "commonsettings.fulltextmode");

            //#4196
            await settingRepository
                .DeleteAsync(setting => setting.Name == "commonsettings.scheduletaskruntimeout" ||
                    setting.Name == "commonsettings.staticfilescachecontrol" ||
                    setting.Name == "commonsettings.supportpreviousnopcommerceversions" ||
                    setting.Name == "securitysettings.pluginstaticfileextensionsBlacklist"
                );

            //#5384
            var seoSettings = await settingService.LoadSettingAsync<SeoSettings>();
            foreach (var slug in NopSeoDefaults.ReservedUrlRecordSlugs)
            {
                if (!seoSettings.ReservedUrlRecordSlugs.Contains(slug))
                    seoSettings.ReservedUrlRecordSlugs.Add(slug);
            }
            await settingService.SaveSettingAsync(seoSettings);

            //#3015
            if (!await settingService.SettingExistsAsync(seoSettings, settings => settings.HomepageTitle))
            {
                seoSettings.HomepageTitle = seoSettings.DefaultTitle;
                await settingService.SaveSettingAsync(seoSettings);
            }

            if (!await settingService.SettingExistsAsync(seoSettings, settings => settings.HomepageDescription))
            {
                seoSettings.HomepageDescription = "Your home page description";
                await settingService.SaveSettingAsync(seoSettings);
            }

            //#5210
            var adminAreaSettings = await settingService.LoadSettingAsync<AdminAreaSettings>();
            if (!await settingService.SettingExistsAsync(adminAreaSettings, settings => settings.ShowDocumentationReferenceLinks))
            {
                adminAreaSettings.ShowDocumentationReferenceLinks = true;
                await settingService.SaveSettingAsync(adminAreaSettings);
            }

            //#4944
            var shippingSettings = await settingService.LoadSettingAsync<ShippingSettings>();
            if (!await settingService.SettingExistsAsync(shippingSettings, settings => settings.RequestDelay))
            {
                shippingSettings.RequestDelay = 300;
                await settingService.SaveSettingAsync(shippingSettings);
            }

            //#276 AJAX filters
            var catalogSettings = await settingService.LoadSettingAsync<CatalogSettings>();
            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.UseAjaxCatalogProductsLoading))
            {
                catalogSettings.UseAjaxCatalogProductsLoading = true;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.EnableManufacturerFiltering))
            {
                catalogSettings.EnableManufacturerFiltering = true;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.EnablePriceRangeFiltering))
            {
                catalogSettings.EnablePriceRangeFiltering = true;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.SearchPagePriceRangeFiltering))
            {
                catalogSettings.SearchPagePriceRangeFiltering = true;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.SearchPagePriceFrom))
            {
                catalogSettings.SearchPagePriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.SearchPagePriceTo))
            {
                catalogSettings.SearchPagePriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.SearchPageManuallyPriceRange))
            {
                catalogSettings.SearchPageManuallyPriceRange = false;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.ProductsByTagPriceRangeFiltering))
            {
                catalogSettings.ProductsByTagPriceRangeFiltering = true;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.ProductsByTagPriceFrom))
            {
                catalogSettings.ProductsByTagPriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.ProductsByTagPriceTo))
            {
                catalogSettings.ProductsByTagPriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.ProductsByTagManuallyPriceRange))
            {
                catalogSettings.ProductsByTagManuallyPriceRange = true;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            //#4303
            var orderSettings = await settingService.LoadSettingAsync<OrderSettings>();
            if (!await settingService.SettingExistsAsync(orderSettings, settings => settings.DisplayCustomerCurrencyOnOrders))
            {
                orderSettings.DisplayCustomerCurrencyOnOrders = false;
                await settingService.SaveSettingAsync(orderSettings);
            }

            //#16 #2909
            if (!await settingService.SettingExistsAsync(catalogSettings, settings => settings.AttributeValueOutOfStockDisplayType))
            {
                catalogSettings.AttributeValueOutOfStockDisplayType = AttributeValueOutOfStockDisplayType.AlwaysDisplay;
                await settingService.SaveSettingAsync(catalogSettings);
            }

            //#5482
            await settingService.SetSettingAsync("avalarataxsettings.gettaxratebyaddressonly", true);
            await settingService.SetSettingAsync("avalarataxsettings.taxratebyaddresscachetime", 480);

            //#5349
            if (!await settingService.SettingExistsAsync(shippingSettings, settings => settings.EstimateShippingCityNameEnabled))
            {
                shippingSettings.EstimateShippingCityNameEnabled = false;
                await settingService.SaveSettingAsync(shippingSettings);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}