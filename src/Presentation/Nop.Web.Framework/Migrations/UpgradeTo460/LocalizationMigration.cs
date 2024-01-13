using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo460;

[NopUpdateMigration("2023-07-26 14:00:10", "4.60", UpdateMigrationType.Localization)]
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
            
            //4622
            "PDFInvoice.OrderDate",
            "PDFInvoice.Company",
            "PDFInvoice.Name",
            "PDFInvoice.Phone",
            "PDFInvoice.Fax",
            "PDFInvoice.Address",
            "PDFInvoice.Address2",
            "PDFInvoice.VATNumber",
            "PDFInvoice.PaymentMethod",
            "PDFInvoice.ShippingMethod",
            "PDFInvoice.BillingInformation",
            "PDFInvoice.ShippingInformation",
            "PDFInvoice.OrderNotes",
            "PDFInvoice.OrderNotes.CreatedOn",
            "PDFInvoice.OrderNotes.Note",
            "PDFPackagingSlip.Shipment",
            "PDFInvoice.Order#",
            "PDFInvoice.Discount",
            "PDFInvoice.Sub-Total",
            "PDFInvoice.Shipping",
            "PDFInvoice.OrderTotal",
            "PDFInvoice.PaymentMethodAdditionalFee",
            "PDFInvoice.Pickup",
            "PDFInvoice.Product(s)",
            "PDFInvoice.Tax",
            "PDFPackagingSlip.Address",
            "PDFPackagingSlip.Address2",
            "PDFPackagingSlip.Company",
            "PDFPackagingSlip.Name",
            "PDFPackagingSlip.Order",
            "PDFPackagingSlip.Phone",
            "PDFPackagingSlip.ProductName",
            "PDFPackagingSlip.QTY",
            "PDFPackagingSlip.ShippingMethod",
            "PDFPackagingSlip.SKU",
            "PDFProductCatalog.Price",
            "PDFProductCatalog.SKU",

            //#6814
            "Admin.ConfigurationSteps.ShippingProviders.ShipStation.Title",
            "Admin.ConfigurationSteps.ShippingProviders.ShipStation.Text"
        });

        #endregion

        #region Add or update locales

        localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            //#3075
            ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithCategoryName"] = "Allow customers to search with category name",
            ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithCategoryName.Hint"] = "Check to allow customers to search with category name.",
            ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithManufacturerName"] = "Allow customers to search with manufacturer name",
            ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithManufacturerName.Hint"] = "Check to allow customers to search with manufacturer name.",

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
            ["Admin.Orders.Import.CustomersDontExist"] = "Customers with the following GUIDs don't exist: {0}",
            ["Admin.Orders.Import.ProductsDontExist"] = "Products with the following SKUs don't exist: {0}",
            ["Admin.Orders.Imported"] = "Orders have been imported successfully.",
            ["Admin.Orders.List.ImportFromExcelTip"] = "Imported orders are distinguished by order GUID. If the order GUID already exists, then its details will be updated.",

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
            ["Admin.Catalog.Products.Multimedia.Videos.Alert.VideoUpdate"] = "Failed to update product video.",
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
            ["Admin.Configuration.Settings.GeneralCommon.BlockTitle.RobotsTxt"] = "robots.txt",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsInstruction"] = "You also may extend the robots.txt data by adding the {0} file to the wwwroot directory of your site.",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsRules"] = "Additions rules",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsRules.Hint"] = "Enter additional rules for the robots.txt file.",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsAllowSitemapXml"] = "Allow sitemap.xml",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsAllowSitemapXml.Hint"] = "Check to allow robots to access the sitemap.xml file.",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsCustomFileExists"] = "robots.txt file data overridden by {0} file in site root.",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowLanguages"] = "Disallow languages",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowLanguages.Hint"] = "The list of languages to disallow.",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowPaths"] = "Disallow paths",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowPaths.Hint"] = "The list of paths to disallow.",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsLocalizableDisallowPaths"] = "Localizable disallow paths",
            ["Admin.Configuration.Settings.GeneralCommon.RobotsLocalizableDisallowPaths.Hint"] = "The list of localizable paths to disallow.",

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
            ["Admin.Configuration.Settings.Tax.EuVatEnabledForGuests.Hint"] = "Check to enable EU VAT (the European Union Value Added Tax) for guest customers. They will have to enter it during the checkout at the billing address step.",
            ["Checkout.VatNumber"] = "VAT number",
            ["Checkout.VatNumber.Disabled"] = "VAT number can be entered (on the <a href=\"{0}\">customer info page</a>) and used only after registration",
            ["Checkout.VatNumber.Warning"] = "VAT number is {0}",

            //#4591
            ["Admin.Configuration.Stores.Fields.SslEnabled"] = "SSL",
            ["Admin.Configuration.Stores.Fields.SslEnabled.Hint"] = "SSL (Secure Socket Layer) is the standard security technology for establishing an encrypted connection between a web server and the browser. This ensures that all data exchanged between web server and browser arrives unchanged.",
            ["Admin.Configuration.Stores.Ssl.Enable"] = "Enable SSL",
            ["Admin.Configuration.Stores.Ssl.Disable"] = "Disable SSL",
            ["Admin.Configuration.Stores.Ssl.Updated"] = "The SSL setting has been successfully changed. Do not forget to synchronize the store URL with the current HTTP protocol.",

            ["Admin.Reports.SalesSummary.Vendor"] = "Vendor",
            ["Admin.Reports.SalesSummary.Vendor.Hint"] = "Search by a specific vendor.",

            //#6353
            ["Admin.Promotions.Discounts.Fields.CouponCode.Reserved"] = "The entered coupon code is already reserved for the discount '{0}'",

            //#6378
            ["Admin.Configuration.Settings.Media.AllowSVGUploads"] = "Allow SVG uploads in admin area",
            ["Admin.Configuration.Settings.Media.AllowSVGUploads.Hint"] = "Check to allow uploading of SVG files in admin area.",

            //#6396
            ["Admin.Catalog.Products.Fields.MinStockQuantity.Hint"] = "If you track inventory, you can perform a number of different actions when the current stock quantity falls below (reaches) your minimum stock quantity.",
            ["Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.MinStockQuantity.Hint"] = "If you track inventory by product attributes, you can perform a number of different actions when the current stock quantity falls below (reaches) your minimum stock quantity (e.g. Low stock report).",
            //#6213
            ["Admin.System.Maintenance.DeleteMinificationFiles"] = "Delete minification files",
            ["Admin.System.Maintenance.DeleteMinificationFiles.Text"] = "Clear the bundles directory.",
            ["Admin.System.Maintenance.DeleteMinificationFiles.TotalDeleted"] = "{0} files were deleted",

            //#6336
            ["Admin.Customers.Customers.RewardPoints.Fields.AddNegativePointsValidity"] = "Points validity is not allowed for point reduction.",

            //#6411
            ["Admin.StockQuantityHistory.Messages.ReadyForPickupByCustomer"] = "The stock quantity has been reduced when an order item of the order #{0} became a ready for pickup by customer",

            //4622
            ["Pdf.OrderDate"] = "Date",
            ["Pdf.Address.Company"] = "Company",
            ["Pdf.Address.Name"] = "Name",
            ["Pdf.Address.Phone"] = "Phone",
            ["Pdf.Address.Fax"] = "Fax",
            ["Pdf.Address"] = "Address",
            ["Pdf.Address2"] = "Address 2",
            ["Pdf.Address.VATNumber"] = "VAT number",
            ["Pdf.BillingInformation"] = "Billing Information",
            ["Pdf.ShippingInformation"] = "Shipping Information",
            ["Pdf.OrderNotes"] = "Order notes",
            ["Pdf.Address.PaymentMethod"] = "Payment method",
            ["Pdf.Address.ShippingMethod"] = "Shipping method",
            ["Pdf.Shipment"] = "Shipment",
            ["Pdf.Order"] = "Order",
            ["Pdf.Shipping"] = "Shipping",
            ["Pdf.SubTotal"] = "Sub-total",
            ["Pdf.Discount"] = "Discount",
            ["Pdf.OrderTotal"] = "Order total",
            ["Pdf.PaymentMethodAdditionalFee"] = "Payment Method Additional Fee",
            ["Pdf.PickupPoint"] = "Pickup point",
            ["Pdf.Tax"] = "Tax",

            //#43
            ["Admin.Configuration.Stores.Info"] = "Info",

            //5701
            ["Admin.Configuration.AppSettings.Common.UseAutofac"] = "Use Autofac IoC",
            ["Admin.Configuration.AppSettings.Common.UseAutofac.Hint"] = "The value indicating whether to use Autofac IoC container. If disabled, then the default .Net IoC container will be used.",

            //6669
            ["Admin.Catalog.Products.SpecificationAttributes.NameFormat"] = "{0} >> {1}",

            //#6676
            ["RewardPoints.Expired"] = "Unused reward points from {0} have expired",
        }, languageId);

        #endregion

        #region Rename locales

        this.RenameLocales(new Dictionary<string, string>
        {
            //#6255
            ["Forum.BreadCrumb.HomeTitle"] = "Forum.Breadcrumb.HomeTitle",
            ["Forum.BreadCrumb.ForumHomeTitle"] = "Forum.Breadcrumb.ForumHomeTitle",
            ["Forum.BreadCrumb.ForumGroupTitle"] = "Forum.Breadcrumb.ForumGroupTitle",
            ["Forum.BreadCrumb.ForumTitle"] = "Forum.Breadcrumb.ForumTitle",
            ["Forum.BreadCrumb.TopicTitle"] = "Forum.Breadcrumb.TopicTitle",

            //#3511
            ["Admin.Configuration.Settings.Catalog.NewProductsNumber"] = "Admin.Configuration.Settings.Catalog.NewProductsPageSize",
            ["Admin.Configuration.Settings.Catalog.NewProductsNumber.Hint"] = "Admin.Configuration.Settings.Catalog.NewProductsPageSize.Hint",

            //#7
            ["Admin.Catalog.Products.Pictures"] = "Admin.Catalog.Products.Multimedia.Pictures",
            ["Admin.Catalog.Products.Pictures.AddNew"] = "Admin.Catalog.Products.Multimedia.Pictures.AddNew",
            ["Admin.Catalog.Products.Pictures.Alert.PictureAdd"] = "Admin.Catalog.Products.Multimedia.Pictures.Alert.PictureAdd",
            ["Admin.Catalog.Products.Pictures.Fields.DisplayOrder"] = "Admin.Catalog.Products.Multimedia.Pictures.Fields.DisplayOrder",
            ["Admin.Catalog.Products.Pictures.Fields.DisplayOrder.Hint"] = "Admin.Catalog.Products.Multimedia.Pictures.Fields.DisplayOrder.Hint",
            ["Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute"] = "Admin.Catalog.Products.Multimedia.Pictures.Fields.OverrideAltAttribute",
            ["Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute.Hint"] = "Admin.Catalog.Products.Multimedia.Pictures.Fields.OverrideAltAttribute.Hint",
            ["Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute"] = "Admin.Catalog.Products.Multimedia.Pictures.Fields.OverrideTitleAttribute",
            ["Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute.Hint"] = "Admin.Catalog.Products.Multimedia.Pictures.Fields.OverrideTitleAttribute.Hint",
            ["Admin.Catalog.Products.Pictures.Fields.Picture"] = "Admin.Catalog.Products.Multimedia.Pictures.Fields.Picture",
            ["Admin.Catalog.Products.Pictures.Fields.Picture.Hint"] = "Admin.Catalog.Products.Multimedia.Pictures.Fields.Picture.Hint",
            ["Admin.Catalog.Products.Copy.CopyImages"] = "Admin.Catalog.Products.Copy.CopyMultimedia",
            ["Admin.Catalog.Products.Copy.CopyImages.Hint"] = "Admin.Catalog.Products.Copy.CopyMultimedia.Hint",

            //#43
            ["Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription"] = "Admin.Configuration.Stores.Fields.DefaultMetaDescription",
            ["Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription.Hint"] = "Admin.Configuration.Stores.Fields.DefaultMetaDescription.Hint",
            ["Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords"] = "Admin.Configuration.Stores.Fields.DefaultMetaKeywords",
            ["Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords.Hint"] = "Admin.Configuration.Stores.Fields.DefaultMetaKeywords.Hint",
            ["Admin.Configuration.Settings.GeneralCommon.DefaultTitle"] = "Admin.Configuration.Stores.Fields.DefaultTitle",
            ["Admin.Configuration.Settings.GeneralCommon.DefaultTitle.Hint"] = "Admin.Configuration.Stores.Fields.DefaultTitle.Hint",
            ["Admin.Configuration.Settings.GeneralCommon.HomepageDescription"] = "Admin.Configuration.Stores.Fields.HomepageDescription",
            ["Admin.Configuration.Settings.GeneralCommon.HomepageDescription.Hint"] = "Admin.Configuration.Stores.Fields.HomepageDescription.Hint",
            ["Admin.Configuration.Settings.GeneralCommon.HomepageTitle"] = "Admin.Configuration.Stores.Fields.HomepageTitle",
            ["Admin.Configuration.Settings.GeneralCommon.HomepageTitle.Hint"] = "Admin.Configuration.Stores.Fields.HomepageTitle.Hint",

            //4622
            ["PDFInvoice.ProductName"] = "Pdf.Product.Name",
            ["PDFInvoice.SKU"] = "Pdf.Product.Sku",
            ["PDFInvoice.VendorName"] = "Pdf.Product.VendorName",
            ["PDFProductCatalog.Weight"] = "Pdf.Product.Weight",
            ["PDFInvoice.ProductPrice"] = "Pdf.Product.Price",
            ["PDFInvoice.ProductQuantity"] = "Pdf.Product.Quantity",
            ["PDFProductCatalog.StockQuantity"] = "Pdf.Product.StockQuantity",
            ["PDFInvoice.ProductTotal"] = "Pdf.Product.Total",
            ["PDFInvoice.RewardPoints"] = "Pdf.RewardPoints",
            ["PDFInvoice.TaxRate"] = "Pdf.TaxRate",
            ["PDFInvoice.GiftCardInfo"] = "Pdf.GiftCardInfo"
        }, languages, localizationService);

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}