using System.Collections.Generic;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo470
{
    [NopMigration("2023-01-01 00:00:00", "4.70.0", UpdateMigrationType.Localization, MigrationProcessType.Update)]
    public class LocalizationMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var (languageId, languages) = this.GetLanguageData();

            #region Delete locales

            localizationService.DeleteLocaleResources(new List<string>
            {
                //#4834
                "Admin.System.Warnings.PluginNotLoaded",
                //#7
                "Admin.Catalog.Products.Multimedia.Videos.SaveBeforeEdit"
            });

            #endregion

            #region Rename locales

            #endregion

            #region Add or update locales

            localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
            {
                //#4834
                ["Admin.System.Warnings.PluginMainAssemblyNotFound"] = "{0}: The main assembly isn't found. Hence this plugin can't be loaded.",
                ["Admin.System.Warnings.PluginNotCompatibleWithCurrentVersion"] = "{0}: The plugin isn't compatible with the current version. Hence this plugin can't be loaded.",
                //#6309
                ["ShoppingCart.ReorderWarning"] = "Some products are not available anymore, so they weren't added to the cart.",
                //import product refactoring
                ["Admin.Catalog.Products.Import.DatabaseNotContainCategory"] = "Import product '{0}'. Database doesn't contain the '{1}' category",
                ["Admin.Catalog.Products.Import.DatabaseNotContainManufacturer"] = "Import product '{0}'. Database doesn't contain the '{1}' manufacturer",
                //6551
                ["Admin.Configuration.Settings.Catalog.CacheProductPrices.Hint"] = "Check to cache product prices. It can significantly improve performance. But you should not enable it if you use some complex discounts, discount requirement rules, or coupon codes.",
                //6541
                ["Customer.FullNameFormat"] = "{0} {1}",
                //6521
                ["Admin.Configuration.AppSettings.Common.CrawlerOnlyAdditionalUserAgentStringsPath"] = "Crawler user agent additional strings path",
                ["Admin.Configuration.AppSettings.Common.CrawlerOnlyAdditionalUserAgentStringsPath.Hint"] = "Specify a path to the file with additional crawler only user agent strings.",
                //6557
                ["Admin.Configuration.Settings.CustomerUser.PasswordMaxLength"] = "Password maximum length",
                ["Admin.Configuration.Settings.CustomerUser.PasswordMaxLength.Hint"] = "Specify password maximum length",
                ["Validation.Password.LengthValidation"] = "<li>must have at least {0} characters and not greater than {1} characters</li>",
                //7
                ["Admin.Catalog.Products.Multimedia.SaveBeforeEdit"] = "You need to save the product before you can upload pictures or videos for this product page.",
                //6543
                ["Admin.Catalog.Products.Multimedia.Videos.Alert.VideoAdd.EmptyUrl"] = "Video URL is required.",
                //6388 PayPal Commerce tour
                ["Admin.ConfigurationSteps.PaymentMethods.PayPal.Text"] = "If you want to process payments online, we’d recommend you to set up the PayPal Commerce payment method. PayPal Commerce gives your buyers a simplified and secure checkout experience. Learn how to set this plugin <a href=\"{0}\" target=\"_blank\">here</a>.",
                ["Admin.ConfigurationSteps.PaymentMethods.PayPal.Title"] = "PayPal Commerce",
                ["Admin.ConfigurationSteps.PaymentMethods.Configure.Text"] = "You can configure each payment method by clicking the appropriate <b>Configure</b> button. Now we’ll configure the PayPal Commerce payment method.",
                ["Admin.ConfigurationSteps.PaymentMethods.Configure.Title"] = "Configure PayPal Commerce",
                
                ["Admin.ConfigurationSteps.PaymentPayPal.SignUp.Title"] = "Create a PayPal account",
                ["Admin.ConfigurationSteps.PaymentPayPal.SignUp.Text"] = "You have two options to do this: you can either register an account on the PayPal website or you can do this from the plugin configuration page. Let’s register an account from the plugin configuration page.",
                ["Admin.ConfigurationSteps.PaymentPayPal.Register.Title"] = "Register an account",
                ["Admin.ConfigurationSteps.PaymentPayPal.Register.Text"] = "Enter your email address and let PayPal check everything out by clicking the <b>Save</b> button.",
                ["Admin.ConfigurationSteps.PaymentPayPal.Register.Text2"] = "If everything is OK, you will see the green notification and a newly added <b>Sign up for PayPal</b> button. Click this button to register an account. You need to go through a few steps to fill in all the required data. The last step will be to verify your email address in order to activate your account.",

                ["Admin.ConfigurationSteps.PaymentPayPal.ApiCredentials.Title"] = "Specify API credentials",
                ["Admin.ConfigurationSteps.PaymentPayPal.ApiCredentials.Text"] = "If you already have an app created in your PayPal account or would like to test it in the sandbox mode, follow these steps.",
                
                ["Admin.ConfigurationSteps.PaymentPayPal.Sandbox.Title"] = "Sandbox",
                ["Admin.ConfigurationSteps.PaymentPayPal.Sandbox.Text"] = "Use sandbox if you want to test the payment method first.",

                ["Admin.ConfigurationSteps.PaymentPayPal.Credentials.Title"] = "Credentials",
                ["Admin.ConfigurationSteps.PaymentPayPal.Credentials.Text"] = "After you create and set up your application in your <b>PayPal</b> account, you need to copy the <b>Client ID</b> and <b>Secret</b>, and paste them into these fields.",

                ["Admin.ConfigurationSteps.PaymentPayPal.PaymentType.Title"] = "Payment type",
                ["Admin.ConfigurationSteps.PaymentPayPal.PaymentType.Text"] = "Choose the <b>Payment type</b> to either capture payment immediately (<b>Capture</b>) or <b>Authorize</b> payment for an order after order creation.",

                ["Admin.ConfigurationSteps.PaymentPayPal.Prominently.Title"] = "Feature PayPal Prominently",
                ["Admin.ConfigurationSteps.PaymentPayPal.Prominently.Text"] = "Logos and banners are a great way to let your customers know that you choose PayPal to securely process their payments. This panel allows you to customize the display options for the PayPal buttons and logo on your store pages.",

                ["Admin.ConfigurationSteps.PaymentPayPal.Configured.Title"] = "Configured",
                ["Admin.ConfigurationSteps.PaymentPayPal.Configured.Text"] = "These settings are different from the basic ones, you have already configured the PayPal Commerce plugin!",
                //6567
                ["Admin.Configuration.Settings.Catalog.DisplayAllPicturesOnCatalogPages.Hint"] = "When enabled, customers will see a slider at the bottom of each picture block. It'll be visible only when a product has more than one picture.",
            }, languageId);

            #endregion
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
