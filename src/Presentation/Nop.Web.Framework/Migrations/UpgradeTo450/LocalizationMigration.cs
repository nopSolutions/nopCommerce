using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Migrations.UpgradeTo450
{
    [NopMigration("2021-04-23 00:00:00", "4.50.0", UpdateMigrationType.Localization, MigrationProcessType.Update)]
    public class LocalizationMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            //use localizationService to add, update and delete localization resources
            localizationService.DeleteLocaleResourcesAsync(new List<string>
            {
                "Admin.Configuration.AppSettings.Hosting.UseHttpClusterHttps",
                "Admin.Configuration.AppSettings.Hosting.UseHttpClusterHttps.Hint",
                "Admin.Configuration.AppSettings.Hosting.UseHttpXForwardedProto",
                "Admin.Configuration.AppSettings.Hosting.UseHttpXForwardedProto.Hint",
                "Admin.Configuration.AppSettings.Hosting.ForwardedHttpHeader",
                "Admin.Configuration.AppSettings.Hosting.ForwardedHttpHeader.Hint",
                //#5042
                "Admin.Help.Topics"
            }).Wait();

            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            var languages = languageService.GetAllLanguagesAsync(true).Result;
            var languageId = languages
                .Where(lang => lang.UniqueSeoCode == new CultureInfo(NopCommonDefaults.DefaultLanguageCulture).TwoLetterISOLanguageName)
                .Select(lang => lang.Id).FirstOrDefault();

            localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                //#5696
                ["Admin.ContentManagement.MessageTemplates.List.SearchKeywords"] = "Search keywords",
                ["Admin.ContentManagement.MessageTemplates.List.SearchKeywords.Hint"] = "Keywords to search by name, body, or subject.",

				//New configurations to forward proxied headers
                ["Admin.Configuration.AppSettings.Hosting.ForwardedForHeaderName"] = "The header used to retrieve the originating client IP",
                ["Admin.Configuration.AppSettings.Hosting.ForwardedForHeaderName.Hint"] = "Specify a custom HTTP header name to determine the originating IP address (e.g., CF-Connecting-IP, X-ProxyUser-Ip).",
                ["Admin.Configuration.AppSettings.Hosting.ForwardedProtoHeaderName"] = "The header used to retrieve the value for the originating scheme",
                ["Admin.Configuration.AppSettings.Hosting.ForwardedProtoHeaderName.Hint"] = "Specify a custom HTTP header name for identifying the protocol (HTTP or HTTPS) that a client used to connect to your proxy or load balancer.",
                ["Admin.Configuration.AppSettings.Hosting.UseProxy"] = "Use proxy servers",
                ["Admin.Configuration.AppSettings.Hosting.UseProxy.Hint"] = "Enable this setting to apply forwarded headers to their matching fields on the current HTTP-request.",
                ["Admin.Configuration.AppSettings.Hosting.KnownProxies"] = "Addresses of known proxies",
                ["Admin.Configuration.AppSettings.Hosting.KnownProxies.Hint"] = "Specify a list of IP addresses (comma separated) to accept forwarded headers.",

                //#5618
                ["Admin.Configuration.Settings.GeneralCommon.EnableCssBundling.Hint"] = "Enable to combine (bundle) multiple CSS files into a single file. Do not enable if you're running nopCommerce in IIS virtual directory. And please note it could take up to two minutes for changes to existing files to be applied (when enabled).",
                ["Admin.Configuration.Settings.GeneralCommon.EnableJsBundling.Hint"] = "Enable to combine (bundle) multiple JavaScript files into a single file. And please note it could take up to two minutes for changes to existing files to be applied (when enabled).",

                //#4880
                ["Admin.Configuration.AppSettings.Common.ServeUnknownFileTypes"] = "Serve unknown types of static files",
                ["Admin.Configuration.AppSettings.Common.ServeUnknownFileTypes.Hint"] = "Enable this setting to serve files that are in the .well-known directory without a recognized content-type",

                //#5532
                ["Admin.Configuration.Languages.CLDR.Warning"] = "Please make sure the appropriate CLDR package is installed for this culture. You can set CLDR for the specified culture on the <a href=\"{0}\" target=\"_blank\">General Settings page, Localization tab</a>.",

                //#4325
                ["Admin.Common.Alert.NothingSelected"] = "Please select at least one record.",

                //#5316
                ["Account.Login.AlreadyLogin"] = "You are already logged in as {0}. You may log in with another account.",

                //#5511
                ["Admin.Configuration.AppSettings.Data"] = "Data configuration",
                ["Admin.Configuration.AppSettings.Data.ConnectionString"] = "Connection string",
                ["Admin.Configuration.AppSettings.Data.ConnectionString.Hint"] = "Sets a connection string.",
                ["Admin.Configuration.AppSettings.Data.DataProvider"] = "Data provider",
                ["Admin.Configuration.AppSettings.Data.DataProvider.Hint"] = "Sets a data provider.",
                ["Admin.Configuration.AppSettings.Data.SQLCommandTimeout"] = "SQL command timeout",
                ["Admin.Configuration.AppSettings.Data.SQLCommandTimeout.Hint"] = "Gets or sets the wait time (in seconds) before terminating the attempt to execute a command and generating an error. By default, timeout isn't set and a default value for the current provider used. Set 0 to use infinite timeout.",
                ["Admin.Configuration.AppSettings.Description"] = "Configuration in ASP.NET Core is performed using a different configuration providers (e.g. external appsettings.json configuration file, environment variables, etc). These settings are used when the application is launched, so after editing them, the application will be restarted. You can find a detailed description of all configuration parameters in <a href=\"{0}\" target=\"_blank\">our documentation</a>. Please note that changing the values here will overwrite the external appsettings.json file, settings from other configuration providers will not be affected.",
                ["Enums.Nop.Data.DataProviderType.Unknown"] = "Unknown",
                ["Enums.Nop.Data.DataProviderType.SqlServer"] = "Microsoft SQL Server",
                ["Enums.Nop.Data.DataProviderType.MySql"] = "MySQL",
                ["Enums.Nop.Data.DataProviderType.PostgreSQL"] = "PostgreSQL",
                
                //#5838
                ["Admin.Configuration.Languages.NeedRestart"] = "Since language cultures are loaded only when the application is starting, you have to restart the application for it to work correctly once the language is changed.",

                //#5155
                ["Admin.System.Warnings.PluginNotInstalled"] = "You haven't installed the following plugin(s)",
                ["Admin.System.Warnings.PluginNotInstalled.HelpText"] = "You may delete the plugins you don't use in order to increase startup time",

                ["Admin.Catalog.Products.ProductAttributes.Attributes.Values.AlreadyExistsInCombination"] = "This attribute value cannot be removed because it is already used in this combination: {0}.",

                //#5162
                ["Admin.ContentManagement.MessageTemplates.Description.ShipmentDelivered.CustomerNotification"] = "This message template is used to notify a customer that the shipment of the certain order is delivered.",
                ["Admin.ContentManagement.MessageTemplates.Description.ShipmentReadyForPickup.CustomerNotification"] = "This message template is used to notify a customer that the shipment of the certain order is ready for pickup.",
                ["Admin.ContentManagement.MessageTemplates.Description.ShipmentSent.CustomerNotification"] = "This message template is used to notify a customer that the shipment of the certain order is sent.",
                ["Admin.Orders.Shipments.CanMarkAsReadyForPickup"] = "Ready for pickup",
                ["Admin.Orders.Shipments.CanMarkAsReadyForPickup.Hint"] = "Check to apply current date to shipment.",
                ["Admin.Orders.Shipments.DateNotAvailable"] = "-",
                ["Admin.Orders.Shipments.List.LoadNotReadyForPickup"] = "Load not ready for pickup",
                ["Admin.Orders.Shipments.List.LoadNotReadyForPickup.Hint"] = "Load only not ready for pickup shipments.",
                ["Admin.Orders.Shipments.PickupInStore"] = "Pickup from store",
                ["Admin.Orders.Shipments.ReadyForPickupDate"] = "Date ready for pickup",
                ["Admin.Orders.Shipments.ReadyForPickupDate.Button"] = "Set as ready for pickup",
                ["Admin.Orders.Shipments.ReadyForPickupDate.EnterUtc"] = "Date and time should be entered in Coordinated Universal Time (UTC)",
                ["Admin.Orders.Shipments.ReadyForPickupDate.Hint"] = "The date this shipment was ready for pickup.",
                ["Admin.Orders.Shipments.ReadyForPickupDate.NotYet"] = "Not yet",
                ["Admin.Orders.Shipments.ReadyForPickupSelected"] = "Set as ready for pickup (selected)",
                ["Order.Shipments.ReadyForPickupDate"] = "Date ready for pickup",
                ["Order.Shipments.ReadyForPickupDate.NotYet"] = "Not yet",
                ["Admin.Orders.Shipments.Products.QtyReadyForPickup"] = "Qty ready for pickup",
                ["Admin.Orders.Shipments.Products.QtyToPickup"] = "Qty to pickup",
                
                //#5042
                ["Admin.Help.Documentation"] = "Documentation",
                ["Admin.Help.SolutionPartners"] = "Solution partners",

                //#2061
                ["Admin.ReturnRequests.QuantityReturnedToStock"] = "{0} item(s) have been successfully returned to the stock.",
                ["Admin.ReturnRequests.Fields.ReturnedQuantity"] = "Quantity returned to stock",
                ["Admin.ReturnRequests.Fields.ReturnedQuantity.Hint"] = "The quantity already returned to stock.",
                ["Admin.ReturnRequests.Fields.ReturnedQuantity.CannotBeLessThanQuantityAlreadyReturned"] = "The quantity to be returned to stock cannot be less than quantity already returned: {0}.",
                ["Admin.ReturnRequests.Fields.ReturnedQuantity.MustBeEqualOrGreaterThanZero"] = "The quantity to be returned to stock must be equal or greater than zero.",
                ["Admin.ReturnRequests.Fields.ReturnedQuantity.MustBeLessOrEqualQuantityField"] = "The quantity to be returned to stock must be less or equal the quantity field: {0}.",
                ["Admin.ReturnRequests.Fields.Quantity.MustBeEqualOrGreaterThanReturnedQuantityField"] = "The quantity must be equal or greater than the quantity to be returned to stock: {0}.",
                ["Admin.ReturnRequests.Fields.Quantity.Required"] = "The quantity is required.",
                
                //#5551
                ["Admin.Configuration.Settings.Catalog.EnableSpecificationAttributeFiltering"] = "Enable specification attribute filtering",
                ["Admin.Configuration.Settings.Catalog.EnableSpecificationAttributeFiltering.Hint"] = "Check to enable the specification attribute filtering on catalog pages.",

                //#5952
                ["Admin.Catalog.Products.Fields.ShortDescription.Hint"] = "Short description is the text that is displayed in product list i.e. category / manufacturer pages.",

                //#5654
                ["Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled.Warning"] = "Attention! The changes will take effect only after the application is restarted.",

                //#5204
                ["Admin.Configuration.Settings.Shipping.ShippingSorting"] = "Sort shipping methods by",
                ["Admin.Configuration.Settings.Shipping.ShippingSorting.Hint"] = "Select the field to sort shipping methods by.",
                ["Enums.Nop.Core.Domain.Shipping.ShippingSortingEnum.Position"] = "Position",
                ["Enums.Nop.Core.Domain.Shipping.ShippingSortingEnum.ShippingCost"] = "Shipping Cost",
                //#5138
                ["PDFInvoice.FileName"] = "order",

                //#5834
                ["Admin.Configuration.AppSettings.WebOptimizer"] = "Bundling & minimization",
                ["Admin.Configuration.AppSettings.WebOptimizer.EnableDiskCache"] = "Enable disk cache",
                ["Admin.Configuration.AppSettings.WebOptimizer.EnableDiskCache.Hint"] = "Check to store assets on disk.",
                ["Admin.Configuration.AppSettings.WebOptimizer.CacheDirectory"] = "Cache directory",
                ["Admin.Configuration.AppSettings.WebOptimizer.CacheDirectory.Hint"] = "Path of the directory where assets will be stored if disk cache is enabled.",
                ["Admin.Configuration.AppSettings.WebOptimizer.JavaScriptBundleSuffix"] = "Suffix of JavaScript bundle files",
                ["Admin.Configuration.AppSettings.WebOptimizer.JavaScriptBundleSuffix.Hint"] = "Enter the key that will be put to the end of the bundle name.",
                ["Admin.Configuration.AppSettings.WebOptimizer.CssBundleSuffix"] = "Suffix of style bundle files",
                ["Admin.Configuration.AppSettings.WebOptimizer.CssBundleSuffix.Hint"] = "Enter the key that will be put to the end of the bundle name.",

                //#4905
                ["Admin.Configuration.Settings.RewardPoints.MaximumRedeemedRate"] = "Maximum redeemed rate",
                ["Admin.Configuration.Settings.RewardPoints.MaximumRedeemedRate.Hint"] = "Limit the maximum order total that could be paid by reward points (in percent). For example, if set to 0.6, then only 60% of order total could be paid using reward points, but not more than the 'Maximum reward points to use per order'. Set to 0 if you do not want to use this setting.",
                ["Checkout.UseRewardPoints"] = "Use my reward points, {0} of {1} reward points ({2}) available for this order",

                //#6007
                ["Admin.System.Templates.NotDeleteOnlyOne"] = "It's not allowed to delete the only one template.",

                //#6034
                ["Checkout.EditAddress"] = "Edit address",

                //#6052
                ["Account.Login.NewCustomerText"] = "By creating an account on our website, you will be able to shop faster, be up to date on an order's status, and keep track of the orders you have previously made.",

                ["Permission.SalesSummaryReport"] = "Admin area. Access sales summary report",
                ["Permission.ManageAppSettings"] = "Admin area. Manage App Setting",
                ["Permission.ManageExternalAuthenticationMethods"] = "Admin area. Manage External Authentication Methods",
                ["Permission.ManageMultifactorAuthenticationMethods"] = "Admin area. Manage Multifactor Authentication Methods",
                ["Permission.AccessProfiling"] = "Public store. Access MiniProfiler results"
            }, languageId).Wait();

            // rename locales
            var localesToRename = new[]
            {
                //#5834
                new { Name = "Admin.Configuration.Settings.GeneralCommon.EnableJsBundling", NewName = "Admin.Configuration.AppSettings.WebOptimizer.EnableJavaScriptBundling" },
                new { Name = "Admin.Configuration.Settings.GeneralCommon.EnableJsBundling.Hint", NewName = "Admin.Configuration.AppSettings.WebOptimizer.EnableJavaScriptBundling.Hint" },
                new { Name = "Admin.Configuration.Settings.GeneralCommon.EnableCssBundling", NewName = "Admin.Configuration.AppSettings.WebOptimizer.EnableCssBundling" },
                new { Name = "Admin.Configuration.Settings.GeneralCommon.EnableCssBundling.Hint", NewName = "Admin.Configuration.AppSettings.WebOptimizer.EnableCssBundling.Hint" }
            };

            foreach (var lang in languages)
            {
                foreach (var locale in localesToRename)
                {
                    var lsr = localizationService.GetLocaleStringResourceByNameAsync(locale.Name, lang.Id, false).Result;
                    if (lsr != null)
                    {
                        lsr.ResourceName = locale.NewName;
                        localizationService.UpdateLocaleStringResourceAsync(lsr).Wait();
                    }
                }
            }
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
