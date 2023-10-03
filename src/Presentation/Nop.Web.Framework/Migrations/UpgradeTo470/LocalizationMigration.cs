using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo470
{
    [NopUpdateMigration("2023-01-01 00:00:00", "4.70", UpdateMigrationType.Localization)]
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
                "Admin.Catalog.Products.Multimedia.Videos.SaveBeforeEdit",
                //#6518
                "Reviews.ProductReviewsFor",
                //1934
                "Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Picture.Hint",
                "Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Picture.NoPicture",
                "Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Picture.Hint",
                "Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Picture.NoPicture",

                "Admin.Configuration.AppSettings.Common.MiniProfilerEnabled",
                "Admin.Configuration.AppSettings.Common.MiniProfilerEnabled.Hint",
                "Permission.AccessProfiling",
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

                //6555
                ["Admin.Configuration.AppSettings.Hosting.KnownNetworks"] = "Addresses of known proxy networks",
                ["Admin.Configuration.AppSettings.Hosting.KnownNetworks.Hint"] = "Specify a list of IP CIDR notations (comma separated) to accept forwarded headers. e.g. 172.64.0.0/13,162.158.0.0/15",
                //#6602
                ["Admin.Configuration.Settings.Media.PicturesStoredIntoDatabase.Note"] = "NOTE: Do not forget to backup your database before changing this option. It is not recommended to change this setting in production environment.",
                //6167
                ["Admin.ContentManagement.MessageTemplates.Fields.AllowDirectReply"] = "Allow Direct Reply",
                ["Admin.ContentManagement.MessageTemplates.Fields.AllowDirectReply.Hint"] = "When checked, the store owner can reply directly to the customer's email address from mailbox when a customer-related message is received.",
                //5023
                ["Header.SkipNavigation.Text"] = "Skip Navigation",
                //6640
                ["Admin.Configuration.Settings.CustomerUser.AddressFormFields.DefaultCountry"] = "Default Country",
                ["Admin.Configuration.Settings.CustomerUser.AddressFormFields.DefaultCountry.Hint"] = "Select the default country for address form fields. This can speed up the checkout process.",
                ["Admin.Configuration.Settings.CustomerUser.DefaultCountry"] = "Default Country",
                ["Admin.Configuration.Settings.CustomerUser.DefaultCountry.Hint"] = "Select the default country for customer form fields. This can speed up the registration process.",

                //5312
                ["Admin.Customers.Customers.Imported"] = "Customers have been imported successfully.",
                ["Admin.Customers.Customers.ImportFromExcelTip"] = "Imported customers are distinguished by customer GUID. If the customer GUID already exists, then its details will be updated. If GUID not exists we try to use email address as an identifier. You may leave customer GUID empty to new customers.",
                ["ActivityLog.ImportCustomers"] = "{0} customers were imported",
                //6660
                ["ActivityLog.DeletePlugin"] = "Deleted a plugin (FriendlyName: '{0}' version: {1})",
                ["ActivityLog.InstallNewPlugin"] = "Installed a new plugin (FriendlyName: '{0}' version: {1})",
                ["ActivityLog.UninstallPlugin"] = "Uninstalled a plugin (FriendlyName: '{0}' version: {1})",
                ["ActivityLog.UpdatePlugin"] = "Updated plugin (FriendlyName: '{0}' from version: {1} to version: {2})",
                //6645
                ["Address.LineFormat"] = "{0}{1}{2}{3}{4}{5}{6}",
                ["Pdf.AddressLine"] = "Address",
                //6678
                ["Admin.Configuration.Settings.Catalog.PageShareCode.Hint"] = "A page share button code. By default, we're using ShareThis service.",

                //6682
                ["Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsletterPage"] = "Show in newsletter block",
                ["Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsletterPage.Hint"] = "Check to show CAPTCHA in newsletter block when subscribing.",
                //1934
                ["Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Pictures"] = "Pictures",
                ["Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Pictures.Hint"] = "Choose pictures associated to this attribute combination. These pictures will replace the main product image when this product attribute combination is selected.",
                ["Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Pictures"] = "Pictures",
                ["Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Pictures.Hint"] = "Choose pictures associated to this attribute combination. These pictures will replace the main product image when this product attribute combination is selected.",
                ["Admin.Configuration.Settings.ProductEditor.DisplayAttributeCombinationImagesOnly"] = "Display attribute combination images only",
                ["Admin.Catalog.Products.Fields.DisplayAttributeCombinationImagesOnly"] = "Display attribute combination images only",
                ["Admin.Catalog.Products.Fields.DisplayAttributeCombinationImagesOnly.Hint"] = "Check to display attribute combination images only. When enabled, customers will see attribute combination images only if any.",

                //5768
                ["Admin.Configuration.Settings.Tax.AutomaticallyDetectCountry"] = "Automatically detect country by IP address",
                ["Admin.Configuration.Settings.Tax.AutomaticallyDetectCountry.Hint"] = "When this setting is enabled, if the customer's billing/shipping address is not already set, the country of address used for tax calculation will be determined automatically by the GEO service (by IP address).",

                //433
                ["Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.Downloadable"] = "The associated product is downloadable, keep in mind that customers won't be able to download it.",

                //#6788
                ["Admin.Catalog.Products.Fields.IsDownload.Hint"] = "Check if the product is downloadable. When customers purchase a downloadable product, they can download it directly from your store. The link will be visible after checkout. Please note that it's recommended to use the 'Use download URL' feature for large files (instead of uploading them to the database).",

                //#6853
                ["Admin.Configuration.Settings.CustomerUser.NeutralGenderEnabled"] = "'Neutral' option enabled",
                ["Admin.Configuration.Settings.CustomerUser.NeutralGenderEnabled.Hint"] = "Set if you need three gender options available - Male, female, neutral (as per German laws).",
                ["Admin.Customers.Customers.Fields.Gender.Neutral"] = "Neutral",
                ["Account.Fields.Gender.Neutral"] = "Neutral",

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
