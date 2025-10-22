using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo470;

[NopUpdateMigration("2023-02-01 14:00:03", "4.70", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var customerSettings = this.LoadSetting<CustomerSettings>();
        if (!this.SettingExists(customerSettings, settings => settings.PasswordMaxLength))
        {
            customerSettings.PasswordMaxLength = 64;
            this.SaveSetting(customerSettings, settings => settings.PasswordMaxLength);
        }

        if (!this.SettingExists(customerSettings, settings => settings.DefaultCountryId))
        {
            customerSettings.DefaultCountryId = null;
            this.SaveSetting(customerSettings, settings => settings.DefaultCountryId);
        }

        var securitySettings = this.LoadSetting<SecuritySettings>();
        if (!this.SettingExists(securitySettings, settings => settings.UseAesEncryptionAlgorithm))
        {
            securitySettings.UseAesEncryptionAlgorithm = false;
            this.SaveSetting(securitySettings, settings => settings.UseAesEncryptionAlgorithm);
        }

        if (!this.SettingExists(securitySettings, settings => settings.AllowStoreOwnerExportImportCustomersWithHashedPassword))
        {
            securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword = true;
            this.SaveSetting(securitySettings, settings => settings.AllowStoreOwnerExportImportCustomersWithHashedPassword);
        }

        //#7053
        if (!this.SettingExists(securitySettings, settings => settings.LogHoneypotDetection))
        {
            securitySettings.LogHoneypotDetection = true;
            this.SaveSetting(securitySettings, settings => settings.LogHoneypotDetection);
        }

        var addressSettings = this.LoadSetting<AddressSettings>();
        if (!this.SettingExists(addressSettings, settings => settings.DefaultCountryId))
        {
            addressSettings.DefaultCountryId = null;
            this.SaveSetting(addressSettings, settings => settings.DefaultCountryId);
        }

        var captchaSettings = this.LoadSetting<CaptchaSettings>();
        //#6682
        if (!this.SettingExists(captchaSettings, settings => settings.ShowOnNewsletterPage))
        {
            captchaSettings.ShowOnNewsletterPage = false;
            this.SaveSetting(captchaSettings, settings => settings.ShowOnNewsletterPage);
        }

        var taxSettings = this.LoadSetting<TaxSettings>();
        if (!this.SettingExists(taxSettings, settings => settings.AutomaticallyDetectCountry))
        {
            taxSettings.AutomaticallyDetectCountry = true;
            this.SaveSetting(taxSettings, settings => settings.AutomaticallyDetectCountry);
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

        var robotsTxtSettings = this.LoadSetting<RobotsTxtSettings>();

        foreach (var path in newDisallowPaths)
        {
            if (robotsTxtSettings.DisallowPaths.Contains(path))
                continue;

            robotsTxtSettings.DisallowPaths.Add(path);
        }

        this.SaveSetting(robotsTxtSettings, settings => settings.DisallowPaths);

        //#6853
        if (!this.SettingExists(customerSettings, settings => settings.NeutralGenderEnabled))
        {
            customerSettings.NeutralGenderEnabled = false;
            this.SaveSetting(customerSettings, settings => settings.NeutralGenderEnabled);
        }

        //#6891
        if (!this.SettingExists(customerSettings, settings => settings.RequiredReLoginAfterPasswordChange))
        {
            customerSettings.RequiredReLoginAfterPasswordChange = false;
            this.SaveSetting(customerSettings, settings => settings.RequiredReLoginAfterPasswordChange);
        }

        //#7064
        var catalogSettings = this.LoadSetting<CatalogSettings>();
        if (!this.SettingExists(catalogSettings, settings => settings.UseStandardSearchWhenSearchProviderThrowsException))
        {
            catalogSettings.UseStandardSearchWhenSearchProviderThrowsException = true;
            this.SaveSetting(catalogSettings, settings => settings.UseStandardSearchWhenSearchProviderThrowsException);
        }

    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}