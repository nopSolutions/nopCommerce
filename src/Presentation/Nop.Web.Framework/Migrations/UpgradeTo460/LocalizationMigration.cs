using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Migrations.UpgradeTo460
{
    [NopMigration("2022-02-07 00:00:00", "4.60.0", UpdateMigrationType.Localization, MigrationProcessType.Update)]
    public class LocalizationMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var languageService = EngineContext.Current.Resolve<ILanguageService>();

            var languages = languageService.GetAllLanguagesAsync(true).Result;
            var languageId = languages
                .Where(lang => lang.UniqueSeoCode == new CultureInfo(NopCommonDefaults.DefaultLanguageCulture).TwoLetterISOLanguageName)
                .Select(lang => lang.Id).FirstOrDefault();

            #region Delete locales

            localizationService.DeleteLocaleResourcesAsync(new List<string>
            {
                //#6102
                "Admin.Configuration.AppSettings.Plugin.ClearPluginShadowDirectoryOnStartup",
                "Admin.Configuration.AppSettings.Plugin.ClearPluginShadowDirectoryOnStartup.Hint",
                "Admin.Configuration.AppSettings.Plugin.CopyLockedPluginAssembilesToSubdirectoriesOnStartup",
                "Admin.Configuration.AppSettings.Plugin.CopyLockedPluginAssembilesToSubdirectoriesOnStartup.Hint",
                "Admin.Configuration.AppSettings.Plugin.UsePluginsShadowCopy",
                "Admin.Configuration.AppSettings.Plugin.UsePluginsShadowCopy.Hint",

                //#5123
                "Admin.Catalog.Products.Pictures.Alert.AddNew",
            }).Wait();

            #endregion

            #region Rename locales

            var localesToRename = new[]
            {
                //#3511
                new { Name = "Admin.Configuration.Settings.Catalog.NewProductsNumber", NewName = "Admin.Configuration.Settings.Catalog.NewProductsPageSize" },
                new { Name = "Admin.Configuration.Settings.Catalog.NewProductsNumber.Hint", NewName = "Admin.Configuration.Settings.Catalog.NewProductsPageSize.Hint" },
            };

            foreach (var lang in languages)
            {
                foreach (var locale in localesToRename)
                {
                    var lsr = localizationService.GetLocaleStringResourceByNameAsync(locale.Name, lang.Id, false).Result;
                    if (lsr is not null)
                    {
                        lsr.ResourceName = locale.NewName;
                        localizationService.UpdateLocaleStringResourceAsync(lsr).Wait();
                    }
                }
            }

            #endregion

            #region Add locales

            localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                //#3075
                ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithCategoryName"] = "Allow customers to search with category name",
                ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithCategoryName.Hint"] = "Check to allow customer to search with category name.",
                ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithManufacturerName"] = "Allow customers to search with manufacturer name",
                ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithManufacturerName.Hint"] = "Check to allow customer to search with manufacturer name.",

                //#3997
                ["Admin.Configuration.Settings.GeneralCommon.InstagramLink"] = "Instagram URL",
                ["Admin.Configuration.Settings.GeneralCommon.InstagramLink.Hint"] = "Specify your Instagram page URL. Leave empty if you have no such page.",

                ["Footer.FollowUs.Instagram"] = "Instagram",

                //#5802
                ["Admin.Configuration.Settings.GeneralCommon.BlockTitle.CustomHtml"] = "Custom HTML",
                ["Admin.Configuration.Settings.GeneralCommon.FooterCustomHtml"] = "Footer custom HTML",
                ["Admin.Configuration.Settings.GeneralCommon.FooterCustomHtml.Hint"] = "Enter custom HTML here for footer section.",
                ["Admin.Configuration.Settings.GeneralCommon.HeaderCustomHtml"] = "Header custom HTML",
                ["Admin.Configuration.Settings.GeneralCommon.HeaderCustomHtml.Hint"] = "Enter custom HTML here for header section.",

                //#5604
                ["Admin.Configuration.Settings.Order.ShowProductThumbnailInOrderDetailsPage"] = "Show product thumbnail in order details page",
                ["Admin.Configuration.Settings.Order.ShowProductThumbnailInOrderDetailsPage.Hint"] = "Check to show product thumbnail in order details page.",
                ["Admin.Configuration.Settings.Media.OrderThumbPictureSize"] = "Order thumbnail image size",
                ["Admin.Configuration.Settings.Media.OrderThumbPictureSize.Hint"] = "The default size (pixels) for product thumbnail images on the order details page.",
                ["Order.Product(s).Image"] = "Image",

                //#3777
                ["ActivityLog.ExportCategories"] = "{0} categories were exported",
                ["ActivityLog.ExportCustomers"] = "{0} customers were exported",
                ["ActivityLog.ExportManufacturers"] = "{0} manufacturers were exported",
                ["ActivityLog.ExportOrders"] = "{0} orders were exported",
                ["ActivityLog.ExportProducts"] = "{0} products were exported",
                ["ActivityLog.ExportStates"] = "{0} states and provinces were exported",
                ["ActivityLog.ExportNewsLetterSubscriptions"] = "{0} newsletter subscriptions were exported",
                ["ActivityLog.ImportNewsLetterSubscriptions"] = "{0} newsletter subscriptions were imported",

                //#5947
                ["Admin.Customers.Customers.List.SearchLastActivityFrom"] = "Last activity from",
                ["Admin.Customers.Customers.List.SearchLastActivityFrom.Hint"] = "The last activity from date for the search.",
                ["Admin.Customers.Customers.List.SearchLastActivityTo"] = "Last activity to",
                ["Admin.Customers.Customers.List.SearchLastActivityTo.Hint"] = "The last activity to date for the search.",
                ["Admin.Customers.Customers.List.SearchRegistrationDateFrom"] = "Registration date from",
                ["Admin.Customers.Customers.List.SearchRegistrationDateFrom.Hint"] = "The registration from date for the search.",
                ["Admin.Customers.Customers.List.SearchRegistrationDateTo"] = "Registration date to",
                ["Admin.Customers.Customers.List.SearchRegistrationDateTo.Hint"] = "The registration to date for the search.",

                //#1933
                ["Admin.Configuration.Settings.Catalog.DisplayAllPicturesOnCatalogPages"] = "Display all pictures on catalog pages",
                ["Admin.Configuration.Settings.Catalog.DisplayAllPicturesOnCatalogPages.Hint"] = "Check to display all pictures on catalog pages.",

                //#3511
                ["Admin.Configuration.Settings.Catalog.NewProductsAllowCustomersToSelectPageSize"] = "'New products' page. Allow customers to select page size",
                ["Admin.Configuration.Settings.Catalog.NewProductsAllowCustomersToSelectPageSize.Hint"] = "'New products' page. Check to allow customers to select the page size from a predefined list of options.",
                ["Admin.Configuration.Settings.Catalog.NewProductsPageSizeOptions"] = "'New products' page. Page size options",
                ["Admin.Configuration.Settings.Catalog.NewProductsPageSizeOptions.Hint"] = "'New products' page. Comma separated list of page size options (e.g. 10, 5, 15, 20). First option is the default page size if none are selected.",

                //#5123
                ["Admin.Catalog.Products.Pictures.Fields.Picture.Hint"] = "You can choose multiple images to upload at once. If the picture size exceeds your stores max image size setting, it will be automatically resized.",
                ["Common.FileUploader.Upload.Files"] = "Upload files",
            }, languageId).Wait();

            #endregion
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
