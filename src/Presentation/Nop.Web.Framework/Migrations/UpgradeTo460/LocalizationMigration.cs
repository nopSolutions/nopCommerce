﻿using System.Collections.Generic;
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

                //#7
                "Admin.Catalog.Products.Pictures.SaveBeforeEdit",
                "Admin.Catalog.Products.Pictures.AddButton",

                "Admin.Configuration.AppSettings.Common.SupportPreviousNopcommerceVersions",
                "Admin.Configuration.AppSettings.Common.SupportPreviousNopcommerceVersions.Hint",
            }).Wait();

            #endregion

            #region Rename locales

            var localesToRename = new[]
            {
                //#6255
                new { Name = "Forum.BreadCrumb.HomeTitle", NewName = "Forum.Breadcrumb.HomeTitle" },
                new { Name = "Forum.BreadCrumb.ForumHomeTitle", NewName = "Forum.Breadcrumb.ForumHomeTitle" },
                new { Name = "Forum.BreadCrumb.ForumGroupTitle", NewName = "Forum.Breadcrumb.ForumGroupTitle" },
                new { Name = "Forum.BreadCrumb.ForumTitle", NewName = "Forum.Breadcrumb.ForumTitle" },
                new { Name = "Forum.BreadCrumb.TopicTitle", NewName = "Forum.Breadcrumb.TopicTitle" },

                
                //#3511
                new { Name = "Admin.Configuration.Settings.Catalog.NewProductsNumber", NewName = "Admin.Configuration.Settings.Catalog.NewProductsPageSize" },
                new { Name = "Admin.Configuration.Settings.Catalog.NewProductsNumber.Hint", NewName = "Admin.Configuration.Settings.Catalog.NewProductsPageSize.Hint" },

                //#7
                new { Name = "Admin.Catalog.Products.Pictures", NewName =  "Admin.Catalog.Products.Multimedia.Pictures"},
                new { Name = "Admin.Catalog.Products.Pictures.AddNew", NewName = "Admin.Catalog.Products.Multimedia.Pictures.AddNew"},
                new { Name = "Admin.Catalog.Products.Pictures.Alert.PictureAdd", NewName = "Admin.Catalog.Products.Multimedia.Pictures.Alert.PictureAdd"},
                new { Name = "Admin.Catalog.Products.Pictures.Fields.DisplayOrder", NewName = "Admin.Catalog.Products.Multimedia.Pictures.Fields.DisplayOrder"},
                new { Name = "Admin.Catalog.Products.Pictures.Fields.DisplayOrder.Hint", NewName = "Admin.Catalog.Products.Multimedia.Pictures.Fields.DisplayOrder.Hint"},
                new { Name = "Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute", NewName = "Admin.Catalog.Products.Multimedia.Pictures.Fields.OverrideAltAttribute"},
                new { Name = "Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute.Hint", NewName = "Admin.Catalog.Products.Multimedia.Pictures.Fields.OverrideAltAttribute.Hint"},
                new { Name = "Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute", NewName = "Admin.Catalog.Products.Multimedia.Pictures.Fields.OverrideTitleAttribute"},
                new { Name = "Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute.Hint", NewName = "Admin.Catalog.Products.Multimedia.Pictures.Fields.OverrideTitleAttribute.Hint"},
                new { Name = "Admin.Catalog.Products.Pictures.Fields.Picture", NewName = "Admin.Catalog.Products.Multimedia.Pictures.Fields.Picture"},
                new { Name = "Admin.Catalog.Products.Pictures.Fields.Picture.Hint", NewName = "Admin.Catalog.Products.Multimedia.Pictures.Fields.Picture.Hint"},
                new { Name = "Admin.Catalog.Products.Copy.CopyImages", NewName = "Admin.Catalog.Products.Copy.CopyMultimedia"},
                new { Name = "Admin.Catalog.Products.Copy.CopyImages.Hint", NewName = "Admin.Catalog.Products.Copy.CopyMultimedia.Hint"},
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

            #region Add or update locales

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

                //#5313
                ["ActivityLog.ImportOrders"] = "{0} orders were imported",
                ["Admin.Orders.Import.CustomersDontExist"] = "Customers with the following guids don't exist: {0}",
                ["Admin.Orders.Import.ProductsDontExist"] = "Products with the following SKUs don't exist: {0}",
                ["Admin.Orders.Imported"] = "Orders have been imported successfully.",
                ["Admin.Orders.List.ImportFromExcelTip"] = "Imported orders are distinguished by order guid. If the order guid already exists, then its corresponding information will be updated.",

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

                //#5809
                ["Admin.Configuration.Settings.Gdpr.DeleteInactiveCustomersAfterMonths"] = "Delete inactive customers after months",
                ["Admin.Configuration.Settings.Gdpr.DeleteInactiveCustomersAfterMonths.Hint"] = "Enter the number of months after which the customers and their personal data will be deleted.",

                //#29
                ["Admin.Configuration.Settings.Catalog.DisplayFromPrices"] = "Display 'From' prices",
                ["Admin.Configuration.Settings.Catalog.DisplayFromPrices.Hint"] = "Check to display 'From' prices on catalog pages. This will display the minimum possible price of a product based on price adjustments of attributes and combinations instead of the fixed base price. If enabled, it is also recommended to enable setting 'Cache product prices'. But please note that it can affect performance if you use some complex discounts, discount requirement rules, etc.",

                //#5089
                ["Products.Availability.LowStock"] = "Low stock",
                ["Products.Availability.LowStockWithQuantity"] = "{0} low stock",
                ["Admin.Catalog.Products.Fields.LowStockActivity.Hint"] = "Action to be taken when your current stock quantity falls below (reaches) the 'Minimum stock quantity'. Activation of the action will occur only after an order is placed. If the value is 'Nothing', the product detail page will display a low-stock message in public store.",

                //#6101
                ["Admin.System.Warnings.PluginNotInstalled.HelpText"] = "You may delete the plugins you don't use in order to decrease startup time",

                //#6182
                ["Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnCheckoutPageForGuests"] = "Show on checkout page for guests",
                ["Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnCheckoutPageForGuests.Hint"] = "Check to show CAPTCHA on checkout page for guests.",

                //#6111
                ["Admin.ReturnRequests.Fields.ReturnedQuantity.Hint"] = "The quantity to be returned to the stock.",

                //#7
                ["Admin.Catalog.Products.Multimedia"] = "Multimedia",
                ["Admin.Catalog.Products.Multimedia.Videos"] = "Videos",
                ["Admin.Catalog.Products.Multimedia.Videos.SaveBeforeEdit"] = "You need to save the product before you can upload videos for this product page.",
                ["Admin.Catalog.Products.Multimedia.Videos.AddNew"] = "Add a new video",
                ["Admin.Catalog.Products.Multimedia.Videos.Alert.VideoAdd"] = "Failed to add product video.",
                ["Admin.Catalog.Products.Multimedia.Videos.Fields.DisplayOrder"] = "Display order",
                ["Admin.Catalog.Products.Multimedia.Videos.Fields.DisplayOrder.Hint"] = "Display order of the video. 1 represents the top of the list.",
                ["Admin.Catalog.Products.Multimedia.Videos.Fields.Preview"] = "Preview",
                ["Admin.Catalog.Products.Multimedia.Videos.Fields.VideoUrl"] = "Embed video URL",
                ["Admin.Catalog.Products.Multimedia.Videos.Fields.VideoUrl.Hint"] = "Specify the URL path to the video.",
                ["Admin.Catalog.Products.Multimedia.Videos.AddButton"] = "Add product video",
                ["Admin.Catalog.Products.Copy.CopyMultimedia"] = "Copy multimedia",
                ["Admin.Catalog.Products.Copy.CopyMultimedia.Hint"] = "Check to copy the images and videos.",

                //#6115
                ["Admin.Configuration.Settings.Catalog.ShowShortDescriptionOnCatalogPages"] = "Show short description on catalog pages",
                ["Admin.Configuration.Settings.Catalog.ShowShortDescriptionOnCatalogPages.Hint"] = "Check to show product short description on catalog pages.",

                //#5905
                ["Admin.ContentManagement.MessageTemplates.List.IsActive"] = "Is active",
                ["Admin.ContentManagement.MessageTemplates.List.IsActive.ActiveOnly"] = "Active only",
                ["Admin.ContentManagement.MessageTemplates.List.IsActive.All"] = "All",
                ["Admin.ContentManagement.MessageTemplates.List.IsActive.Hint"] = "Search by a \"IsActive\" property.",
                ["Admin.ContentManagement.MessageTemplates.List.IsActive.InactiveOnly"] = "Inactive only",

                //#6062
                ["Account.CustomerAddresses.Added"] = "The new address has been added successfully.",
                ["Account.CustomerAddresses.Updated"] = "The address has been updated successfully.",
                ["Account.CustomerInfo.Updated"] = "The customer info has been updated successfully.",

                //#385
                ["Admin.Configuration.Settings.Catalog.ProductUrlStructureType"] = "Product URL structure type",
                ["Admin.Configuration.Settings.Catalog.ProductUrlStructureType.Hint"] = "Select the product URL structure type (e.g. '/product-seo-name' or '/category-seo-name/product-seo-name' or '/manufacturer-seo-name/product-seo-name').",
                ["Enums.Nop.Core.Domain.Catalog.ProductUrlStructureType.CategoryProduct"] = "/Category/Product",
                ["Enums.Nop.Core.Domain.Catalog.ProductUrlStructureType.ManufacturerProduct"] = "/Manufacturer/Product",
                ["Enums.Nop.Core.Domain.Catalog.ProductUrlStructureType.Product"] = "/Product",

                //#5261
                ["Admin.Configuration.Settings.GeneralCommon.BlockTitle.RobotsTxt"] ="robots.txt",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsInstruction"] = "You also may extend the robots.txt data by adding the {0} file on the wwwroot directory of your site.",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsRules"] = "Additions rules",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsRules.Hint"] = "Put here an additional rules for robots.txt file",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsAllowSitemapXml"] = "Allow sitemap.xml",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsAllowSitemapXml.Hint"] = "Check to allow robots use the sitemap.xml file",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsCustomFileExists"] = "robots.txt file data overridden by {0} file in site root.",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowLanguages"] = "Disallow languages",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowLanguages.Hint"] = "The list of languages which prohibit to use by robots",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowPaths"] = "Disallow paths",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowPaths.Hint"] = "The list of paths which prohibit to use by robots",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsLocalizableDisallowPaths"] = "Localizable disallow paths",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsLocalizableDisallowPaths.Hint"] = "The list of localizable paths which prohibit to use by robots",

                //#5753
                ["Admin.Configuration.Settings.Media.ProductDefaultImage"] = "Default image",
                ["Admin.Configuration.Settings.Media.ProductDefaultImage.Hint"] = "Upload a picture to be used as the default image. If nothing is uploaded, {0} will be used.",

                ["Admin.Help.Training"] = "Training",
                
                //5607
                ["Admin.Configuration.Settings.CustomerUser.ForceMultifactorAuthentication.Hint"] = "Force activation of multi-factor authentication for customer roles specified in Access control list (at least one MFA provider must be active).",
                ["Permission.Authentication.EnableMultiFactorAuthentication"] = "Security. Enable Multi-factor authentication",

                //#3651
                ["Admin.ContentManagement.MessageTemplates.Description.OrderProcessing.CustomerNotification"] = "This message template is used to notify a customer that the certain order is processing. Orders can be viewed by a customer on the account page.",
                ["Admin.Configuration.Settings.Order.AttachPdfInvoiceToOrderProcessingEmail"] = "Attach PDF invoice (\"order processing\" email)",
                ["Admin.Configuration.Settings.Order.AttachPdfInvoiceToOrderProcessingEmail.Hint"] = "Check to attach PDF invoice to the \"order processing\" email sent to a customer.",

                //5705
                ["Admin.Promotions.Discounts.Fields.IsActive"] = "Is active",
                ["Admin.Promotions.Discounts.Fields.IsActive.Hint"] = "Indicating whether the discount is active.",
                ["Admin.Promotions.Discounts.List.IsActive"] = "Is Active",
                ["Admin.Promotions.Discounts.List.IsActive.ActiveOnly"] = "Active only",
                ["Admin.Promotions.Discounts.List.IsActive.All"] = "All",
                ["Admin.Promotions.Discounts.List.IsActive.Hint"] = "Search by \"IsActive\" property.",
                ["Admin.Promotions.Discounts.List.IsActive.InactiveOnly"] = "Inactive only",

                //#1961
                ["Admin.Configuration.Settings.Tax.EuVatEnabledForGuests"] = "EU VAT enabled for guests",
                ["Admin.Configuration.Settings.Tax.EuVatEnabledForGuests.Hint"] = "Check to enable EU VAT (the European Union Value Added Tax) for guest customers.",
                ["Address.Fields.VatNumber"] = "VAT number",
                ["Address.Fields.VatNumber.Warning"] = "VAT number can be entered and used only after <a href=\"{0}\">registration</a>.",

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
