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

        this.SetSettingIfNotExists<CustomerSettings, int>(settings => settings.PasswordMaxLength, 64);
        this.SetSettingIfNotExists<CustomerSettings, int?>(settings => settings.DefaultCountryId, value: null);

        this.SetSettingIfNotExists<SecuritySettings, bool>(settings => settings.UseAesEncryptionAlgorithm, false);
        this.SetSettingIfNotExists<SecuritySettings, bool>(settings => settings.AllowStoreOwnerExportImportCustomersWithHashedPassword, true);

        //#7053
        this.SetSettingIfNotExists<SecuritySettings, bool>(settings => settings.LogHoneypotDetection, true);
        this.SetSettingIfNotExists<AddressSettings, int?>(settings => settings.DefaultCountryId, value: null);

        //#6682
        this.SetSettingIfNotExists<CaptchaSettings, bool>(settings => settings.ShowOnNewsletterPage, false);
        this.SetSettingIfNotExists<TaxSettings, bool>(settings => settings.AutomaticallyDetectCountry, true);

        //#6716
        this.SetSettingIfNotExists<RobotsTxtSettings, List<string>>(settings => settings.DisallowPaths, setting =>
        {
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

            foreach (var path in newDisallowPaths)
            {
                if (setting.DisallowPaths.Contains(path))
                    continue;

                setting.DisallowPaths.Add(path);
            }
        });

        //#6853
        this.SetSettingIfNotExists<CustomerSettings, bool>(settings => settings.NeutralGenderEnabled, false);

        //#6891
        this.SetSettingIfNotExists<CustomerSettings, bool>(settings => settings.RequiredReLoginAfterPasswordChange, false);

        //#7064
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.UseStandardSearchWhenSearchProviderThrowsException, true);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}