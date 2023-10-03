using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo470
{
    [NopUpdateMigration("2023-02-01 14:00:03", "4.70", UpdateMigrationType.Settings)]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            var customerSettings = settingService.LoadSetting<CustomerSettings>();
            if (!settingService.SettingExists(customerSettings, settings => settings.PasswordMaxLength))
            {
                customerSettings.PasswordMaxLength = 64;
                settingService.SaveSetting(customerSettings, settings => settings.PasswordMaxLength);
            }

            if (!settingService.SettingExists(customerSettings, settings => settings.DefaultCountryId))
            {
                customerSettings.DefaultCountryId = null;
                settingService.SaveSetting(customerSettings, settings => settings.DefaultCountryId);
            }

            var securitySettings = settingService.LoadSetting<SecuritySettings>();
            if (!settingService.SettingExists(securitySettings, settings => settings.UseAesEncryptionAlgorithm))
            {
                securitySettings.UseAesEncryptionAlgorithm = false;
                settingService.SaveSetting(securitySettings, settings => settings.UseAesEncryptionAlgorithm);
            }

            if (!settingService.SettingExists(securitySettings, settings => settings.AllowStoreOwnerExportImportCustomersWithHashedPassword))
            {
                securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword = true;
                settingService.SaveSetting(securitySettings, settings => settings.AllowStoreOwnerExportImportCustomersWithHashedPassword);
            }

            var addressSettings = settingService.LoadSetting<AddressSettings>();
            if (!settingService.SettingExists(addressSettings, settings => settings.DefaultCountryId))
            {
                addressSettings.DefaultCountryId = null;
                settingService.SaveSetting(addressSettings, settings => settings.DefaultCountryId);
            }

            var captchaSettings = settingService.LoadSetting<CaptchaSettings>();
            //#6682
            if (!settingService.SettingExists(captchaSettings, settings => settings.ShowOnNewsletterPage))
            {
                captchaSettings.ShowOnNewsletterPage = false;
                settingService.SaveSetting(captchaSettings, settings => settings.ShowOnNewsletterPage);
            }

            var taxSettings = settingService.LoadSetting<TaxSettings>();
            if (!settingService.SettingExists(taxSettings, settings => settings.AutomaticallyDetectCountry))
            {
                taxSettings.AutomaticallyDetectCountry = true;
                settingService.SaveSetting(taxSettings, settings => settings.AutomaticallyDetectCountry);
            }

            //#6716
            var newDisallowPaths = new[]
            {
                "/cart/estimateshipping", "/cart/selectshippingoption", "/customer/addressdelete",
                "/customer/removeexternalassociation", "/customer/checkusernameavailability",
                "/catalog/searchtermautocomplete", "/catalog/getcatalogroot", "/addproducttocart/catalog/*",
                "/addproducttocart/details/*", "/compareproducts/add/*", "/backinstocksubscribe/*",
                "/subscribenewsletter", "/t-popup/*", "/setproductreviewhelpfulness", "/poll/vote",
                "/country/getstatesbycountryid/", "/eucookielawaccept", "/topic/authenticate",
                "/category/products/", "/product/combinations", "/uploadfileproductattribute/*",
                "/shoppingcart/productdetails_attributechange/*", "/uploadfilereturnrequest",
                "/boards/topicwatch/*", "/boards/forumwatch/*", "/install/restartapplication",
                "/boards/postvote", "/product/estimateshipping/*", "/shoppingcart/checkoutattributechange/*"
            };

            var robotsTxtSettings = settingService.LoadSetting<RobotsTxtSettings>();

            foreach (var path in newDisallowPaths)
            {
                if (robotsTxtSettings.DisallowPaths.Contains(path))
                    continue;

                robotsTxtSettings.DisallowPaths.Add(path);
            }

            settingService.SaveSetting(robotsTxtSettings, settings => settings.DisallowPaths);

            //#6853
            if (!settingService.SettingExists(customerSettings, settings => settings.NeutralGenderEnabled))
            {
                customerSettings.NeutralGenderEnabled = false;
                settingService.SaveSetting(customerSettings, settings => settings.NeutralGenderEnabled);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}