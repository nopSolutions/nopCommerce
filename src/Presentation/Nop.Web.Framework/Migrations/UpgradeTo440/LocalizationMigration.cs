using System.Collections.Generic;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Migrations.UpgradeTo440
{
    [NopMigration("2020-06-10 00:00:00", "4.40.0", UpdateMigrationType.Localization)]
    [SkipMigrationOnInstall]
    public class LocalizationMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            //do not use DI, because it produces exception on the installation process
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            //use localizationService to add, update and delete localization resources
            localizationService.DeleteLocaleResources(new List<string>
            {
                "Account.Fields.VatNumber.Status",
                "Account.Fields.VatNumberStatus",
                "Account.PasswordRecovery.OldPassword",
                "Account.PasswordRecovery.OldPassword.Required",
                "Account.Register.Unsuccessful",
                "Account.ShoppingCart",
                "ActivityLog.AddNewWidget",
                "ActivityLog.DeleteWidget",
                "ActivityLog.EditWidget",
                "Admin.Address.Fields.StateProvince.Required",
                "Admin.Catalog.AdditionalProductReviews.Fields.Description",
                "Admin.Catalog.Categories.Breadcrumb",
                "Admin.Catalog.Categories.Fields.CreatedOn",
                "Admin.Catalog.Categories.SwitchToListView",
                "Admin.Catalog.Manufacturers.Fields.CreatedOn",
                "Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.ViewLink",
                "Admin.Catalog.Products.ProductAttributes.Attributes.Values.EditAttributeDetails",
                "Admin.Catalog.Products.SpecificationAttributes.NoAttributeOptions",
                "Admin.Catalog.Products.SpecificationAttributes.SelectOption",
                "Admin.Common.CancelChanges",
                "Admin.Common.Check",
                "Admin.Common.DeleteConfirmationParam",
                "Admin.Common.List",
                "Admin.Common.LoseUnsavedChanges",
                "Admin.Common.SaveChanges",
                "Admin.Configuration.Currencies.Localization",
                "Admin.Configuration.Currencies.Select",
                "Admin.Configuration.EmailAccounts.Fields.SendTestEmailTo.Button",
                "Admin.Configuration.PaymentMethods",
                "Admin.Configuration.PaymentMethodsAndRestrictions",
                "Admin.Configuration.Settings.CustomerUser.BlockTitle.DefaultFields",
                "Admin.Configuration.Settings.CustomerUser.BlockTitle.ExternalAuthentication",
                "Admin.Configuration.Settings.CustomerUser.BlockTitle.TimeZone",
                "Admin.Configuration.Settings.CustomerUser.CustomerSettings",
                "Admin.Configuration.Settings.Order.OrderSettings",
                "Admin.Configuration.Settings.ProductEditor.BlockTitle.LinkedProducts",
                "Admin.Configuration.Settings.ProductEditor.Id",
                "Admin.Configuration.Shipping.Measures.Dimensions.Description",
                "Admin.Configuration.Shipping.Measures.Weights.Description",
                "Admin.Configuration.SMSProviders",
                "Admin.Configuration.SMSProviders.BackToList",
                "Admin.Configuration.SMSProviders.Configure",
                "Admin.Configuration.SMSProviders.Fields.FriendlyName",
                "Admin.Configuration.SMSProviders.Fields.IsActive",
                "Admin.Configuration.SMSProviders.Fields.SystemName",
                "Admin.ContentManagement.Topics.Fields.Store.AllStores",
                "Admin.ContentManagement.Widgets.ChooseZone",
                "Admin.ContentManagement.Widgets.ChooseZone.Hint",
                "Admin.Customers.Customers.Fields.Email.Required",
                "Admin.Customers.Customers.Fields.FirstName.Required",
                "Admin.Customers.Customers.Fields.LastName.Required",
                "Admin.Customers.Customers.Fields.SystemName",
                "Admin.Customers.Customers.Fields.SystemName.Hint",
                "Admin.Customers.Customers.Fields.Username.Required",
                "Admin.Customers.Customers.RewardPoints.Alert.HistoryAdd",
                "Admin.DT.Processing",
                "Admin.NopCommerceNews.HideAdv",
                "Admin.NopCommerceNews.ShowAdv",
                "Admin.Orders.OrderNotes.Alert.Add",
                "Admin.Promotions.Discounts.Fields.AppliedToCategories",
                "Admin.Promotions.Discounts.Fields.AppliedToCategories.Hint",
                "Admin.Promotions.Discounts.Fields.AppliedToCategories.NoRecords",
                "Admin.System.QueuedEmails.Fields.Priority.Required",
                "Common.DeleteConfirmationParam",
                "Common.Extensions.RelativeFormat",
                "Common.Home",
                "EUCookieLaw.CannotBrowse",
                "EUCookieLaw.Title",
                "Filtering.FilterResults",
                "Forum.Replies.Count",
                "Forum.Topics.Count",
                "News.Archive",
                "Newsletter.ResultAlreadyDeactivated",
                "PageTitle.EmailRevalidation",
                "PDFInvoice.CreatedOn",
                "PDFInvoice.Note",
                "PrivateMessages.Send.Subject.Required",
                "PrivateMessages.Sent.DateColumn",
                "PrivateMessages.Sent.DeleteSelected",
                "PrivateMessages.Sent.SubjectColumn",
                "PrivateMessages.Sent.ToColumn",
                "Profile.FullName",
                "RewardPoints.Message.Expired",
                "ShoppingCart.AddToWishlist.Update",
                "ShoppingCart.UpdateCartItem",
                "Tax.SelectType"
            });

            localizationService.AddLocaleResource(new Dictionary<string, string>
            {
                ["Admin.System.Warnings.PluginNotEnabled.AutoFixAndRestart"] = "Uninstall and delete all not used plugins automatically (site will be restarted)",
                ["Admin.Configuration.AppSettings"] = "App settings",
                ["Admin.Configuration.AppSettings.Cache"] = "Cache configuration",
                ["Admin.Configuration.AppSettings.Cache.DefaultCacheTime"] = "Default cache time",
                ["Admin.Configuration.AppSettings.Cache.DefaultCacheTime.Hint"] = "Set default cache time (in minutes).",
                ["Admin.Configuration.AppSettings.Cache.ShortTermCacheTime"] = "Short term cache time",
                ["Admin.Configuration.AppSettings.Cache.ShortTermCacheTime.Hint"] = "Set short term cache time (in minutes).",
                ["Admin.Configuration.AppSettings.Cache.BundledFilesCacheTime"] = "Bundled files cache time",
                ["Admin.Configuration.AppSettings.Cache.BundledFilesCacheTime.Hint"] = "Set bundled files cache time (in minutes).",
                ["Admin.Configuration.AppSettings.Hosting"] = "Hosting configuration",
                ["Admin.Configuration.AppSettings.Hosting.UseHttpClusterHttps"] = "Use HTTP_CLUSTER_HTTPS",
                ["Admin.Configuration.AppSettings.Hosting.UseHttpClusterHttps.Hint"] = "Enable this setting if your hosting uses a load balancer. It'll be used to determine whether the current request is HTTPS.",
                ["Admin.Configuration.AppSettings.Hosting.UseHttpXForwardedProto"] = "Use HTTP_X_FORWARDED_PROTO",
                ["Admin.Configuration.AppSettings.Hosting.UseHttpXForwardedProto.Hint"] = "Enable this setting if you use a reverse proxy server (for example, if you host your site on Linux with Nginx/Apache and SSL).",
                ["Admin.Configuration.AppSettings.Hosting.ForwardedHttpHeader"] = "Forwarded HTTP header",
                ["Admin.Configuration.AppSettings.Hosting.ForwardedHttpHeader.Hint"] = "Use this setting if your hosting doesn't use 'X-FORWARDED-FOR' header to determine IP address. You can specify a custom HTTP header (e.g. CF-Connecting-IP, X-FORWARDED-PROTO, etc).",
                ["Admin.Configuration.AppSettings.Redis"] = "Redis configuration",
                ["Admin.Configuration.AppSettings.Redis.Enabled"] = "Use Redis",
                ["Admin.Configuration.AppSettings.Redis.Enabled.Hint"] = "Enable this setting to use Redis server.",
                ["Admin.Configuration.AppSettings.Redis.ConnectionString"] = "Connection string",
                ["Admin.Configuration.AppSettings.Redis.ConnectionString.Hint"] = "Specify Redis connection string.",
                ["Admin.Configuration.AppSettings.Redis.DatabaseId"] = "Database ID",
                ["Admin.Configuration.AppSettings.Redis.DatabaseId.Hint"] = "Set the specific Redis database. Leave empty to use the different database for each data type (used by default).",
                ["Admin.Configuration.AppSettings.Redis.UseCaching"] = "Use caching",
                ["Admin.Configuration.AppSettings.Redis.UseCaching.Hint"] = "Enable this setting to use Redis server for caching (instead of default in-memory caching).",
                ["Admin.Configuration.AppSettings.Redis.StoreDataProtectionKeys"] = "Store Data Protection keys",
                ["Admin.Configuration.AppSettings.Redis.StoreDataProtectionKeys.Hint"] = "Enable this setting to store the Data Protection keys in Redis database.",
                ["Admin.Configuration.AppSettings.Redis.StorePluginsInfo"] = "Store plugins info",
                ["Admin.Configuration.AppSettings.Redis.StorePluginsInfo.Hint"] = "Enable this setting to store the plugins info in Redis database (instead of default file in directory).",
                ["Admin.Configuration.AppSettings.Redis.IgnoreTimeoutException"] = "Ignore timeout exception",
                ["Admin.Configuration.AppSettings.Redis.IgnoreTimeoutException.Hint"] = "Enable this setting to ignore timeout exception (this increases cache stability but may slightly decrease site performance).",
                ["Admin.Configuration.AppSettings.AzureBlob"] = "Azure Blob storage configuration",
                ["Admin.Configuration.AppSettings.AzureBlob.ConnectionString"] = "Connection string",
                ["Admin.Configuration.AppSettings.AzureBlob.ConnectionString.Hint"] = "Specify the connection string for Azure Blob storage.",
                ["Admin.Configuration.AppSettings.AzureBlob.ContainerName"] = "Container name",
                ["Admin.Configuration.AppSettings.AzureBlob.ContainerName.Hint"] = "Specify the container name for Azure Blob storage.",
                ["Admin.Configuration.AppSettings.AzureBlob.EndPoint"] = "Endpoint",
                ["Admin.Configuration.AppSettings.AzureBlob.EndPoint.Hint"] = "Specify the endpoint for Azure Blob storage.",
                ["Admin.Configuration.AppSettings.AzureBlob.AppendContainerName"] = "Append container name",
                ["Admin.Configuration.AppSettings.AzureBlob.AppendContainerName.Hint"] = "Enable this setting to append the endpoint with the container name when constructing the URL.",
                ["Admin.Configuration.AppSettings.AzureBlob.StoreDataProtectionKeys"] = "Store Data Protection keys",
                ["Admin.Configuration.AppSettings.AzureBlob.StoreDataProtectionKeys.Hint"] = "Enable this setting to store the Data Protection keys in Azure Blob Storage.",
                ["Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysContainerName"] = "Container name for Data Protection keys",
                ["Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysContainerName.Hint"] = "Specify the container name for the Data Protection keys. This should be a private container separate from the Blob container used for media storage.",
                ["Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysVaultId"] = "Key vault ID",
                ["Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysVaultId.Hint"] = "Specify the Azure key vault ID used to encrypt the Data Protection keys.",
                ["Admin.Configuration.AppSettings.Installation"] = "Installation configuration",
                ["Admin.Configuration.AppSettings.Installation.DisableSampleData"] = "Disable sample data",
                ["Admin.Configuration.AppSettings.Installation.DisableSampleData.Hint"] = "Enable this setting to disable sample data for installation.",
                ["Admin.Configuration.AppSettings.Installation.DisabledPlugins"] = "Disabled plugins",
                ["Admin.Configuration.AppSettings.Installation.DisabledPlugins.Hint"] = "Specify a list of plugins (comma separated) ignored during installation.",
                ["Admin.Configuration.AppSettings.Plugin"] = "Plugin configuration",
                ["Admin.Configuration.AppSettings.Plugin.ClearPluginShadowDirectoryOnStartup"] = "Clear plugin shadow directory on startup",
                ["Admin.Configuration.AppSettings.Plugin.ClearPluginShadowDirectoryOnStartup.Hint"] = "Enable this setting to clear the plugin shadow directory (/Plugins/bin) on application startup.",
                ["Admin.Configuration.AppSettings.Plugin.CopyLockedPluginAssembilesToSubdirectoriesOnStartup"] = "Copy locked plugins to subdirectories on startup",
                ["Admin.Configuration.AppSettings.Plugin.CopyLockedPluginAssembilesToSubdirectoriesOnStartup.Hint"] = "Enable this seting to copy 'locked' assemblies from the plugin shadow directory (/Plugins/bin) to temporary subdirectories on application startup.",
                ["Admin.Configuration.AppSettings.Plugin.UseUnsafeLoadAssembly"] = "Use unsafe load assembly",
                ["Admin.Configuration.AppSettings.Plugin.UseUnsafeLoadAssembly.Hint"] = "Enable this seting to load an assembly into the load-from context, bypassing some security checks.",
                ["Admin.Configuration.AppSettings.Plugin.UsePluginsShadowCopy"] = "Use plugins shadow copy",
                ["Admin.Configuration.AppSettings.Plugin.UsePluginsShadowCopy.Hint"] = "Enable this seting to copy plugins to the shadow directory (/Plugins/bin) on application startup.",
                ["Admin.Configuration.AppSettings.Common"] = "Common configuration",
                ["Admin.Configuration.AppSettings.Common.DisplayFullErrorStack"] = "Display full error",
                ["Admin.Configuration.AppSettings.Common.DisplayFullErrorStack.Hint"] = "Enable this setting to display the full error in production environment. It's ignored (always enabled) in development environment.",
                ["Admin.Configuration.AppSettings.Common.MiniProfilerEnabled"] = "Enable MiniProfiler",
                ["Admin.Configuration.AppSettings.Common.MiniProfilerEnabled.Hint"] = "Enable this setting to display the performance indicator by MiniProfiler. By default, the performance indicator can see only Administrators, to change this behavior set ACL rules in the admin area.",
                ["Admin.Configuration.AppSettings.Common.UserAgentStringsPath"] = "User agent strings path",
                ["Admin.Configuration.AppSettings.Common.UserAgentStringsPath.Hint"] = "Specify a path to the file with user agent strings.",
                ["Admin.Configuration.AppSettings.Common.CrawlerOnlyUserAgentStringsPath"] = "Crawler user agent strings path",
                ["Admin.Configuration.AppSettings.Common.CrawlerOnlyUserAgentStringsPath.Hint"] = "Specify a path to the file with crawler only user agent strings. Leave empty to always use full version of user agent strings file.",
                ["Admin.Configuration.AppSettings.Common.UseSessionStateTempDataProvider"] = "Use session state for TempData",
                ["Admin.Configuration.AppSettings.Common.UseSessionStateTempDataProvider.Hint"] = "Enable this seting to store TempData in the session state. By default the cookie-based TempData provider is used to store TempData in cookies."
            });
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
