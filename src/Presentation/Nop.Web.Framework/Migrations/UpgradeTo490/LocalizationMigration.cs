﻿using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopUpdateMigration("2024-12-01 00:00:00", "4.90", UpdateMigrationType.Localization)]
public class LocalizationMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        var (languageId, _) = this.GetLanguageData();

        #region Delete locales

        #endregion

        #region Rename locales

        #endregion

        #region Add or update locales

        localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            //#4834
            ["Admin.Configuration.Settings.GeneralCommon.AdminArea.UseStickyHeaderLayout"] = "Use sticky header",
            ["Admin.Configuration.Settings.GeneralCommon.AdminArea.UseStickyHeaderLayout.Hint"] = "The content header (containing the action buttons) will stick to the top when you reach its scroll position.",

            //#3425
            ["Admin.ContentManagement.MessageTemplates.Description.OrderCompleted.StoreOwnerNotification"] = "This message template is used to notify a store owner that the certain order was completed. The order gets the order status Complete when it's paid and delivered, or it can be changed manually to Complete in Sales - Orders.",

            //#7387
            ["Admin.Catalog.Products.Fields.AgeVerification"] = "Age verification",
            ["Admin.Catalog.Products.Fields.AgeVerification.Hint"] = "Check to require customer registration with date of birth before placing an order.",
            ["Admin.Catalog.Products.Fields.AgeVerification.DateOfBirthDisabled"] = "It looks like you have <a href=\"{0}\" target=\"_blank\">Date of Birth</a> setting disabled.",
            ["Admin.Catalog.Products.Fields.MinimumAgeToPurchase"] = "Minimum age to purchase",
            ["Admin.Catalog.Products.Fields.MinimumAgeToPurchase.Hint"] = "Enter the minimum age for purchasing this product.",
            ["Admin.Catalog.Products.Fields.MinimumAgeToPurchase.ShouldBeGreaterThanZero"] = "The minimum age for purchasing should be greater 0",
            ["ShoppingCart.DateOfBirthRequired"] = "This product has age restrictions. Please specify your age in the account details",
            ["ShoppingCart.MinimumAgeToPurchase"] = "This product is available to customers who are {0} years of age or older",
            ["Admin.Configuration.Settings.ProductEditor.AgeVerification"] = "Age verification",

            //#2184
            ["Admin.Catalog.Products.Multimedia.Pictures.Alert.VendorNumberPicturesLimit"] = "The maximum number of product pictures has been reached.",

            //#7398
            ["Admin.ConfigurationSteps.Product.Details.Text"] = "Enter the relevant product details in these fields. The screenshot below shows how they will be displayed on the product page with the default nopCommerce theme: <div class=\"row row-cols-1\"><img class=\"img-thumbnail mt-3\" src=\"/js/admintour/images/product-page.jpg\"/></div>",
            ["Admin.ConfigurationSteps.PaymentPayPal.ApiCredentials.Text"] = "If you already have an app created in your PayPal account, follow these steps.",

            //#5345
            ["Admin.ContentManagement.Topics.Fields.AvailableEndDateTime"] = "Availability end date",
            ["Admin.ContentManagement.Topics.Fields.AvailableEndDateTime.Hint"] = "The end of the topic's availability (UTC).",
            ["Admin.ContentManagement.Topics.Fields.AvailableEndDateTime.GreaterThanOrEqualToStartDate"] = "The end date must be greater than or equal to the start date.",
            ["Admin.ContentManagement.Topics.Fields.AvailableStartDateTime"] = "Availability start date",
            ["Admin.ContentManagement.Topics.Fields.AvailableStartDateTime.Hint"] = "The start of the topic's availability (UTC).",

            //#6407
            ["ActivityLog.PublicStore.PasswordChanged"] = "Public store. Customer has changed the password",

        }, languageId);

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
