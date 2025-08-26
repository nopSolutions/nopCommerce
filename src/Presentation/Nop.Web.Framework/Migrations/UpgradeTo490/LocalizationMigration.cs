using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopUpdateMigration("2024-12-02 00:00:00", "4.90", UpdateMigrationType.Localization)]
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
            //#7569
            "Admin.Configuration.AppSettings.Common.PluginStaticFileExtensionsBlacklist",
            "Admin.Configuration.AppSettings.Common.PluginStaticFileExtensionsBlacklist.Hint",
            //#7590
            "Checkout.RedirectMessage",

            //#6874
            "Account.Fields.Newsletter",
            "Admin.Configuration.Settings.CustomerUser.NewsletterTickedByDefault",
            "Admin.Configuration.Settings.CustomerUser.NewsletterTickedByDefault.Hint",
            "Admin.Customers.Customers.Fields.Newsletter",
            "Admin.Customers.Customers.Fields.Newsletter.Hint",
            "Checkout.RedirectMessage",
            
            //#1779
            "ActivityLog.PublicStore.Login",

            //#7390
            "Footer.Information",
            "Footer.MyAccount",
            "Footer.CustomerService",
            "Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn1",
            "Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn1.Hint",
            "Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn2",
            "Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn2.Hint",
            "Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn3",
            "Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn3.Hint",
            "Admin.ContentManagement.Topics.Fields.IncludeInTopMenu",
            "Admin.ContentManagement.Topics.Fields.IncludeInTopMenu.Hint",
            "Admin.ConfigurationSteps.TopicList.Location.Title",
            "Admin.ConfigurationSteps.TopicList.Location.Text",
            "Admin.Configuration.Settings.GeneralCommon.BlockTitle.FooterItems",
            "Admin.Configuration.Settings.GeneralCommon.BlockTitle.TopMenuItems",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayApplyVendorAccountFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayApplyVendorAccountFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayBlogFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayBlogFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCompareProductsFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCompareProductsFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayContactUsFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayContactUsFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerAddressesFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerAddressesFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerInfoFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerInfoFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerOrdersFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerOrdersFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayForumsFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayForumsFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewProductsFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewProductsFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewsFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewsFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayProductSearchFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayProductSearchFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayRecentlyViewedProductsFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayRecentlyViewedProductsFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayShoppingCartFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayShoppingCartFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplaySitemapFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplaySitemapFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayWishlistFooterItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayWishlistFooterItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomepageMenuItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomepageMenuItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem.Hint",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem",
            "Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem.Hint",
            "MainMenu.AjaxFailure",
            "Admin.Catalog.Categories.Fields.IncludeInTopMenu",
            "Admin.Catalog.Categories.Fields.IncludeInTopMenu.Hint",
        });

        #endregion

        #region Rename locales

        this.RenameLocales(new Dictionary<string, string>
        {
            //#6874
            ["Admin.Promotions.NewsLetterSubscriptions.Fields.Active"] = "Admin.Promotions.NewsLetterSubscription.Fields.Active",
            ["Admin.Promotions.NewsLetterSubscriptions.Fields.CreatedOn"] = "Admin.Promotions.NewsLetterSubscription.Fields.CreatedOn",
            ["Admin.Promotions.NewsLetterSubscriptions.Fields.Email"] = "Admin.Promotions.NewsLetterSubscription.Fields.Email",
            ["Admin.Promotions.NewsLetterSubscriptions.Fields.Email.Required"] = "Admin.Promotions.NewsLetterSubscription.Fields.Email.Required",
            ["Admin.Promotions.NewsLetterSubscriptions.Fields.Language"] = "Admin.Promotions.NewsLetterSubscription.Fields.Language",
            ["Admin.Promotions.NewsLetterSubscriptions.Fields.Store"] = "Admin.Promotions.NewsLetterSubscription.Fields.Store",
        }, languages, localizationService);

        localizationService.DeleteLocaleResources(new[]
        {
            "Admin.Configuration.AppSettings.AzureBlob",
            "Admin.Configuration.AppSettings.AzureBlob.ConnectionString",
            "Admin.Configuration.AppSettings.AzureBlob.ConnectionString.Hint",
            "Admin.Configuration.AppSettings.AzureBlob.ContainerName",
            "Admin.Configuration.AppSettings.AzureBlob.ContainerName.Hint",
            "Admin.Configuration.AppSettings.AzureBlob.EndPoint",
            "Admin.Configuration.AppSettings.AzureBlob.EndPoint.Hint",
            "Admin.Configuration.AppSettings.AzureBlob.AppendContainerName",
            "Admin.Configuration.AppSettings.AzureBlob.AppendContainerName.Hint",
            "Admin.Configuration.AppSettings.AzureBlob.StoreDataProtectionKeys",
            "Admin.Configuration.AppSettings.AzureBlob.StoreDataProtectionKeys.Hint",
            "Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysContainerName",
            "Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysContainerName.Hint",
            "Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysVaultId",
            "Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysVaultId.Hint",
            "Admin.System.SystemInfo.AzureBlobStorageEnabled",
            "Admin.System.SystemInfo.AzureBlobStorageEnabled.Hint",
        });

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

            //#7498
            ["Admin.Configuration.AppSettings.Common.PermitLimit.Hint"] = "Maximum number of permit counters that can be allowed in a window (1 minute). Must be set to a value > 0 by the time these options are passed to the constructor of FixedWindowRateLimiter. If set to 0 then the limitation is off.",
            ["Admin.Configuration.AppSettings.Common.QueueCount.Hint"] = "Maximum cumulative permit count of queued acquisition requests. Must be set to a value >= 0 by the time these options are passed to the constructor of FixedWindowRateLimiter. If set to 0 then the Queue is off.",

            //#4170
            ["Admin.Promotions.Campaigns.Copy.Name"] = "New campaign name",
            ["Admin.Promotions.Campaigns.Copy.Name.Hint"] = "The name of the new campaign.",
            ["Admin.Promotions.Campaigns.Copy.Name.New"] = "{0} - copy",
            ["Admin.Promotions.Campaigns.Copy"] = "Copy campaign",
            ["Admin.Promotions.Campaigns.Copied"] = "The campaign has been copied successfully",

            //#7477
            ["Pdf.Order"] = "Order #{0}",
            ["Pdf.Shipment"] = "Shipment #{0}",

            //#5279
            ["Search.SearchInTags"] = "Search in product tags",

            //#6407
            ["Admin.Catalog.ProductTags.Info"] = "Product tag info",
            ["Admin.Catalog.ProductTags.Seo"] = "SEO",
            ["Admin.Catalog.ProductTags.Fields.MetaKeywords"] = "Meta keywords",
            ["Admin.Catalog.ProductTags.Fields.MetaKeywords.Hint"] = "Meta keywords to be added to product tag page header.",
            ["Admin.Catalog.ProductTags.Fields.MetaDescription"] = "Meta description",
            ["Admin.Catalog.ProductTags.Fields.MetaDescription.Hint"] = "Meta description to be added to product tag page header.",
            ["Admin.Catalog.ProductTags.Fields.MetaTitle"] = "Meta title",
            ["Admin.Catalog.ProductTags.Fields.MetaTitle.Hint"] = "Override the page title. The default is the name of the product tag.",

            //#7571
            ["Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnCheckGiftCardBalance"] = "Show on check gift card balance page",
            ["Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnCheckGiftCardBalance.Hint"] = "Check to show CAPTCHA on check gift card balance page.",

            //#5771
            ["Admin.Catalog.ProductTags.TaggedProducts"] = "Used by products",
            ["Admin.Catalog.ProductTags.TaggedProducts.Product"] = "Product",
            ["Admin.Catalog.ProductTags.TaggedProducts.Published"] = "Published",

            //#7405
            ["Admin.Configuration.Settings.Catalog.ExportImportCategoryUseLimitedToStores"] = "Export / Import categories with \"limited to stores\"",
            ["Admin.Configuration.Settings.Catalog.ExportImportCategoryUseLimitedToStores.Hint"] = "Check if categories should be exported / imported with \"limited to stores\" property.",

            //#6874
            ["Admin.Promotions.NewsLetterSubscriptionType"] = "Subscription types",
            ["Admin.Promotions.NewsLetterSubscriptionType.Fields.LimitedToStores"] = "Limited to stores",
            ["Admin.Promotions.NewsLetterSubscriptionType.Fields.LimitedToStores.Hint"] = "Option to limit this attribute to a certain store. If you have multiple stores, choose one or several from the list. If you don't use this option just leave this field empty.",
            ["Admin.Promotions.NewsLetterSubscriptionType.Fields.Name"] = "Name",
            ["Admin.Promotions.NewsLetterSubscriptionType.Fields.Name.Hint"] = "Enter the name of subscription type.",
            ["Admin.Promotions.NewsLetterSubscriptionType.Fields.Name.Required"] = "Please provide a name.",
            ["Admin.Promotions.NewsLetterSubscriptionType.Fields.TickedByDefault"] = "Ticked by default",
            ["Admin.Promotions.NewsLetterSubscriptionType.Fields.TickedByDefault.Hint"] = "Check to tick this newsletter subscription type by default.",
            ["Admin.Promotions.NewsLetterSubscriptionType.Fields.DisplayOrder"] = "Display order",
            ["Admin.Promotions.NewsLetterSubscriptionType.Fields.DisplayOrder.Hint"] = "Display order.",
            ["Admin.Promotions.NewsLetterSubscriptionType.Deleted"] = "The subscription type has been deleted successfully.",
            ["Admin.Promotions.NewsLetterSubscriptionType.NotDeleted"] = "You cannot delete the only subscription type that is configured. To completely disable the newsletter, deactivate the \"'Newsletter enabled\" setting on the <a href=\"{0}\">Customer settings</a> page",
            ["Admin.Promotions.NewsLetterSubscriptionType.Added"] = "The subscription type has been added successfully.",
            ["Admin.Promotions.NewsLetterSubscriptionType.Updated"] = "The subscription type has been updated successfully.",
            ["Admin.Promotions.NewsLetterSubscriptionType.EditSubscriptionTypeDetails"] = "Edit subscription type",
            ["Admin.Promotions.NewsLetterSubscriptionType.BackToList"] = "back to subscription types list",
            ["Admin.Promotions.NewsLetterSubscriptionType.AddNew"] = "Add a new subscription type",
            ["Admin.Promotions.NewsLetterSubscriptions.List.CustomerRoles"] = "Customer role",
            ["Admin.Promotions.NewsLetterSubscriptions.List.SubscriptionTypes"] = "Subscription type",
            ["Admin.Promotions.NewsLetterSubscriptions.List.SubscriptionTypes.Hint"] = "Search by a specific subscription type.",
            ["Admin.Promotions.Campaigns.Fields.NewsLetterSubscriptionType"] = "Limited to subscription types",
            ["Admin.Promotions.Campaigns.Fields.NewsLetterSubscriptionType.Hint"] = "Choose a subscription type to which this email will be sent.",
            ["Account.SubscribeToNewsletter"] = "Subscribe to newsletter",
            ["ActivityLog.AddSubscriptionType"] = "Added a new subscription type (ID = {0})",
            ["ActivityLog.EditSubscriptionType"] = "Edited a subscription type (ID = {0})",
            ["ActivityLog.DeleteSubscriptionType"] = "Deleted a subscription type (ID = {0})",
            ["Admin.Promotions.NewsLetterSubscription.EditSubscriptionDetails"] = "Edit subscription",
            ["Admin.Promotions.NewsLetterSubscription.BackToList"] = "back to subscriptions list",
            ["Admin.Promotions.NewsLetterSubscription.AddNew"] = "Add a new subscription",
            ["Admin.Promotions.NewsLetterSubscription.Deleted"] = "The subscription has been deleted successfully.",
            ["Admin.Promotions.NewsLetterSubscription.Added"] = "The subscription has been added successfully.",
            ["Admin.Promotions.NewsLetterSubscription.Updated"] = "The subscription has been updated successfully.",
            ["Admin.Promotions.NewsLetterSubscription.Warning"] = "An entity with this subscription type cannot be added or already exists.",

            ["Admin.Promotions.NewsLetterSubscription.Fields.SubscriptionType"] = "Subscription type",
            ["Admin.Promotions.NewsLetterSubscription.Fields.SubscriptionType.Hint"] = "Enter the type of subscription.",
            ["Admin.Promotions.NewsLetterSubscription.Fields.Active.Hint"] = "A value indicating whether the subscription is active.",
            ["Admin.Promotions.NewsLetterSubscription.Fields.Email.Hint"] = "Enter the email of subscription.",
            ["Admin.Promotions.NewsLetterSubscription.Fields.Store.Hint"] = "Choose store to subscribe to newsletter.",
            ["Admin.Promotions.NewsLetterSubscription.Fields.Language.Hint"] = "Choose language to subscribe to newsletter.",
            ["Admin.Promotions.NewsLetterSubscription.Fields.CreatedOn.Hint"] = "Date/Time the newsletter subscriptions entry was created.",

            //#820
            ["Currency.Selector.Text.Pattern"] = "{0}, {1}",

            //#5652
            ["Admin.System.SystemInfo.DatabaseCollation"] = "Database collation",
            ["Admin.System.SystemInfo.DatabaseCollation.Hint"] = "The collation defines the rules for sorting and comparing data.",

            //#2192
            ["Admin.System.QueuedEmails.RequeueSelected"] = "Requeue selected",

            //#1779
            ["ActivityLog.PublicStore.Login.Fail"] = "Public store. Customer has failed to log in: {0}",
            ["Admin.Configuration.Settings.CustomerUser.NotifyFailedLoginAttempt"] = "Notify customers about failed login attempts",
            ["Admin.Configuration.Settings.CustomerUser.NotifyFailedLoginAttempt.Hint"] = "Check to enable customer notifications on failed login attempts.",
            ["ActivityLog.PublicStore.Login.Success"] = "Public store. Customer has logged in",

            //2921
            ["Admin.System.Maintenance.ShrinkDatabase"] = "Shrink database",
            ["Admin.System.Maintenance.ShrinkDatabase.Complete"] = "Database shrinking completed",
            ["Admin.System.Maintenance.ShrinkDatabase.Progress"] = "Processing...",
            ["Admin.System.Maintenance.ShrinkDatabase.Text"] = "Reclaim disk space by reorganizing physical data storage",

            //#7515
            ["Admin.Catalog.Attributes.ProductAttributes.List.SearchProductAttributeName"] = "Product attribute name",
            ["Admin.Catalog.Attributes.ProductAttributes.List.SearchProductAttributeName.Hint"] = "A product attribute name.",

            //#1266
            ["Account.CustomerOrders.Period"] = "Orders from",
            ["Account.CustomerRecurringPayments"] = "Recurring payments",
            ["Account.CustomerRecurringPayments.NoPayments"] = "No payments",
            ["Enums.Nop.Core.Domain.Orders.OrderHistoryPeriods.All"] = "all time",
            ["Enums.Nop.Core.Domain.Orders.OrderHistoryPeriods.Day"] = "the past day",
            ["Enums.Nop.Core.Domain.Orders.OrderHistoryPeriods.Week"] = "the past week",
            ["Enums.Nop.Core.Domain.Orders.OrderHistoryPeriods.Month"] = "the past month",
            ["Enums.Nop.Core.Domain.Orders.OrderHistoryPeriods.HalfYear"] = "the past six months",
            ["Enums.Nop.Core.Domain.Orders.OrderHistoryPeriods.Year"] = "the past year",

            //#7545
            ["Admin.Catalog.Attributes.SpecificationAttributes.List.SearchName"] = "Name",
            ["Admin.Catalog.Attributes.SpecificationAttributes.List.SearchName.Hint"] = "Search by specification attribute name.",

            //#7630
            ["Admin.Configuration.Settings.Tax.HmrcApiUrl"] = "HMRC API URL",
            ["Admin.Configuration.Settings.Tax.HmrcApiUrl.Hint"] = "The base HMRC access API URL.",
            ["Admin.Configuration.Settings.Tax.HmrcClientId"] = "HMRC API client ID",
            ["Admin.Configuration.Settings.Tax.HmrcClientId.Hint"] = "Your HMRC API client ID is a unique identifier which created when you added your application.",
            ["Admin.Configuration.Settings.Tax.HmrcClientSecret"] = "HMRC API client secret",
            ["Admin.Configuration.Settings.Tax.HmrcClientSecret.Hint"] = "Your client secret is a unique passphrase that you generate to authorise your application.",

            //#7694
            ["Account.BackInStockSubscriptions.Description"] = "You will receive an email when a particular product is back in stock.",
            ["Account.EmailUsernameErrors.EmailAlreadyExists"] = "The email address is already in use",
            ["Account.EmailUsernameErrors.EmailTooLong"] = "Email address is too long",
            ["Account.ForumSubscriptions.Description"] = "You will receive an email when a new forum topic/post is created.",
            ["Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses.Hint"] = "The blind carbon copy (BCC) recipients for this email message.",
            ["BackInStockSubscriptions.Tooltip"] = "You'll receive a onetime email when this product is available for ordering again. We will not send you any other emails or add you to our newsletter; you will only be emailed about this product!",

            //#7390
            ["ActivityLog.AddNewMenu"] = "Added a new menu ('{0}')",
            ["ActivityLog.AddNewMenuItem"] = "Added a new menu item ('{0}')",
            ["ActivityLog.DeleteMenu"] = "Deleted a menu ('{0}')",
            ["ActivityLog.DeleteMenuItem"] = "Deleted a menu item ('{0}')",
            ["ActivityLog.EditMenu"] = "Edited a menu ('{0}')",
            ["ActivityLog.EditMenuItem"] = "Edited a menu item ('{0}')",
            ["Admin.ContentManagement.Menus"] = "Menus",
            ["Admin.ContentManagement.Menus.Added"] = "The new menu has been added successfully.",
            ["Admin.ContentManagement.Menus.AddNew"] = "Add a new menu",
            ["Admin.ContentManagement.Menus.BackToList"] = "back to menu list",
            ["Admin.ContentManagement.Menus.Deleted"] = "The menu has been deleted successfully.",
            ["Admin.ContentManagement.Menus.EditMenuDetails"] = "Edit menu details",
            ["Admin.ContentManagement.Menus.Info"] = "Info",
            ["Admin.ContentManagement.Menus.Fields.DisplayAllCategories"] = "Display all categories",
            ["Admin.ContentManagement.Menus.Fields.DisplayAllCategories.Hint"] = "Check to automatically display all categories in the menu. Otherwise, you may add appropriate menu items manually for all categories you want to be displayed in the menu.",
            ["Admin.ContentManagement.Menus.Fields.DisplayOrder"] = "Display order",
            ["Admin.ContentManagement.Menus.Fields.DisplayOrder.Hint"] = "The display order of the menu item. 1 represents the top of the list.",
            ["Admin.ContentManagement.Menus.Fields.LimitedToStores"] = "Limited to stores",
            ["Admin.ContentManagement.Menus.Fields.LimitedToStores.Hint"] = "Option to limit this menu to a certain store. If you have multiple stores, choose one or several from the list. If you don't use this option just leave this field empty.",
            ["Admin.ContentManagement.Menus.Fields.Name"] = "Name",
            ["Admin.ContentManagement.Menus.Fields.Name.Hint"] = "The name of the menu.",
            ["Admin.ContentManagement.Menus.Fields.Name.Required"] = "The name is required.",
            ["Admin.ContentManagement.Menus.Fields.MenuType"] = "Menu type",
            ["Admin.ContentManagement.Menus.Fields.MenuType.Hint"] = "The type of the menu.",
            ["Admin.ContentManagement.Menus.Fields.CssClass"] = "CSS class",
            ["Admin.ContentManagement.Menus.Fields.CssClass.Hint"] = "Additional CSS class will be added to the menu element. It can be useful for styling purposes.",
            ["Admin.ContentManagement.Menus.Fields.Published"] = "Published",
            ["Admin.ContentManagement.Menus.Fields.Published.Hint"] = "Check to publish this menu (visible in store). Uncheck to unpublish (menu not available in store).",
            ["Admin.ContentManagement.Menus.MenuItem.BackToMenu"] = "back to menu details",
            ["Admin.ContentManagement.Menus.MenuItem.EditDetails"] = "Edit menu item details",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Category"] = "Category",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Category.Hint"] = "Select one of the available categories.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Manufacturer"] = "Manufacturer",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Manufacturer.Hint"] = "Select one of the available manufacturers.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.MenuItemType"] = "Menu item type",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.MenuItemType.Hint"] = "The type of the menu item.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Published"] = "Published",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Published.Hint"] = "Check to publish this menu item (visible in store). Uncheck to unpublish (menu item not available in store).",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Title"] = "Title",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Title.Hint"] = "The title of this menu item.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Title.Required"] = "The title is required.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Topic"] = "Topic",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Topic.Hint"] = "Select one of the available topics.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Topic.Required"] = "The topic is required.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Url"] = "URL",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Url.Hint"] = "Specify the URL for menu item link.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Url.Required"] = "The URL is required.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Vendor"] = "Vendor",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Vendor.Hint"] = "Select one of the available vendors.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.DisplayOrder"] = "Display order",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.DisplayOrder.Hint"] = "The display order of the menu item. 1 represents the top of the list.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.CssClass"] = "CSS class",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.CssClass.Hint"] = "Additional CSS class will be added to the menu item element. It can be useful for styling purposes.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.LimitedToStores"] = "Limited to stores",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.LimitedToStores.Hint"] = "Option to limit this menu item to a certain store. If you have multiple stores, choose one or several from the list. If you don't use this option just leave this field empty.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.MaximumNumberEntities"] = "Maximum number of elements",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.MaximumNumberEntities.Hint"] = "The maximum number of elements to display in the list or grid.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.NumberOfItemsPerGridRow"] = "Number of elements per row",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.NumberOfItemsPerGridRow.Hint"] = "The maximum number of elements that can be shown in each row of the grid.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.NumberOfSubItemsPerGridElement"] = "Number of subcategories per category item",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.NumberOfSubItemsPerGridElement.Hint"] = "Number of subcategories that can be shown for each category item in the grid.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Parent"] = "Parent menu item",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Parent.Hint"] = "Select one of the parent menu items.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Product"] = "Select a product",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Product.Hint"] = "Select one of the available products.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Product.Remove"] = "Remove",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Product.Select"] = "Select",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Product.Required"] = "The product is required",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.RouteName"] = "Standard page",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.RouteName.Hint"] = "Select one of the available pages.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.RouteName.Required"] = "The standard page is required.",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Template"] = "Template",
            ["Admin.ContentManagement.Menus.MenuItem.Fields.Template.Hint"] = "Select one of the available templates.",
            ["Admin.ContentManagement.Menus.MenuItem.Template.List.Description"] = "List - elements are displayed as a list of simple links.",
            ["Admin.ContentManagement.Menus.MenuItem.Template.Grid.Description"] = "Grid - elements are displayed in a grid layout, each genedated with an image and their associated children.",
            ["Admin.ContentManagement.Menus.MenuItem.Template.Simple.Description"] = "Simple - the selected entity is displayed as a simple link.",
            ["Admin.ContentManagement.Menus.MenuItems"] = "Menu items",
            ["Admin.ContentManagement.Menus.MenuItems.SaveBeforeEdit"] = "You need to save the menu before you can add menu items for this menu page.",
            ["Admin.ContentManagement.Menus.MenuItems.AddNew"] = "Add a new menu item",
            ["Admin.ContentManagement.Menus.MenuItems.Updated"] = "The menu item has been updated successfully.",
            ["Admin.ContentManagement.Menus.MenuItems.Deleted"] = "The menu item has been deleted successfully.",
            ["Admin.ContentManagement.Menus.MenuItems.SelectProduct"] = "Select product",
            ["Admin.ContentManagement.Menus.MenuItems.SelectProduct.Fields.ProductName"] = "Product name",
            ["Admin.ContentManagement.Menus.MenuItems.SelectProduct.Fields.Published"] = "Published",
            ["Admin.ContentManagement.Menus.MenuItems.SelectProduct.SearchKeywords"] = "Search keywords",
            ["Admin.ContentManagement.Menus.MenuItems.SelectProduct.SearchKeywords.Hint"] = "Search products by specific keywords.",
            ["Admin.ContentManagement.Menus.MenuItems.SelectProduct.SearchStore"] = "Store",
            ["Admin.ContentManagement.Menus.MenuItems.SelectProduct.SearchStore.Hint"] = "Search by a specific store.",
            ["Admin.ContentManagement.Menus.SelectRoute"] = "Select page",
            ["Admin.ContentManagement.Menus.Updated"] = "The menu has been updated successfully.",
            ["Enums.Nop.Core.Domain.Menus.MenuType.Main"] = "Main menu",
            ["Enums.Nop.Core.Domain.Menus.MenuType.Footer"] = "Footer menu",
            ["Enums.Nop.Core.Domain.Menus.MenuItemType.StandardPage"] = "Standard page",
            ["Enums.Nop.Core.Domain.Menus.MenuItemType.CustomLink"] = "Custom link",
            ["Enums.Nop.Core.Domain.Menus.MenuItemType.TopicPage"] = "Topic (page)",
            ["Enums.Nop.Core.Domain.Menus.MenuItemType.Category"] = "Category",
            ["Enums.Nop.Core.Domain.Menus.MenuItemType.Manufacturer"] = "Manufacturer",
            ["Enums.Nop.Core.Domain.Menus.MenuItemType.Vendor"] = "Vendor",
            ["Enums.Nop.Core.Domain.Menus.MenuItemType.Product"] = "Product",
            ["Enums.Nop.Core.Domain.Menus.MenuItemType.Text"] = "Text without link",
            ["Literals.Nop.Core.Http.NopRouteNames.General.ApplyVendorAccount"] = "Apply for vendor account",
            ["Literals.Nop.Core.Http.NopRouteNames.General.CheckGiftCardBalance"] = "Check gift card balance",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Homepage"] = "Home page",
            ["Literals.Nop.Core.Http.NopRouteNames.General.CompareProducts"] = "Compare products list",
            ["Literals.Nop.Core.Http.NopRouteNames.General.CustomerAddresses"] = "Addresses",
            ["Literals.Nop.Core.Http.NopRouteNames.General.CustomerInfo"] = "My account",
            ["Literals.Nop.Core.Http.NopRouteNames.General.CustomerOrders"] = "Orders",
            ["Literals.Nop.Core.Http.NopRouteNames.General.ContactUs"] = "Contact us",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Login"] = "Login",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Search"] = "Search",
            ["Literals.Nop.Core.Http.NopRouteNames.General.RecentlyViewedProducts"] = "Recently viewed products",
            ["Literals.Nop.Core.Http.NopRouteNames.General.NewProducts"] = "New products",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Blog"] = "Blog",
            ["Literals.Nop.Core.Http.NopRouteNames.General.News"] = "News archive",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Boards"] = "Forums",
            ["Literals.Nop.Core.Http.NopRouteNames.General.ProductTags"] = "Product tags",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Manufacturers"] = "Manufacturers",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Vendors"] = "Vendors",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Sitemap"] = "Sitemap",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Cart"] = "Shopping cart",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Wishlist"] = "Wishlist",

            //#5199
            ["Enums.Nop.Core.Domain.Forums.EditorType.MarkdownEditor"] = "Markdown editor",
            ["MarkdownEditor.TabWrite"] = "Write",
            ["MarkdownEditor.TabPreview"] = "Preview",

            //#7747
            ["Forum.TruncatePostfix"] = "...",

            //#7388
            ["Admin.Configuration.Settings.GeneralCommon.BlockTitle.Translation"] = "Translation",
            ["Admin.Configuration.Settings.GeneralCommon.AllowPreTranslate"] = "Allow to pre-translate",
            ["Admin.Configuration.Settings.GeneralCommon.AllowPreTranslate.Hint"] = "Check to allow pre-translate functionality.",
            ["Admin.Configuration.Settings.GeneralCommon.TranslateFromLanguage"] = "Translate from the language",
            ["Admin.Configuration.Settings.GeneralCommon.TranslateFromLanguage.Hint"] = "Set the base language to translate from.",
            ["Admin.Configuration.Settings.GeneralCommon.NotTranslateLanguages"] = "Languages to ignore",
            ["Admin.Configuration.Settings.GeneralCommon.NotTranslateLanguages.Hint"] = "You may specify the languages that will be ignored when pre-translating content.",
            ["Admin.Configuration.Settings.GeneralCommon.GoogleTranslateApiKey"] = "API key",
            ["Admin.Configuration.Settings.GeneralCommon.GoogleTranslateApiKey.Hint"] = "Set the Google Translate API key.",
            ["Admin.Configuration.Settings.GeneralCommon.DeepLAuthKey"] = "Auth key",
            ["Admin.Configuration.Settings.GeneralCommon.DeepLAuthKey.Hint"] = "Set the DeepL auth key.",
            ["Admin.Configuration.Settings.GeneralCommon.TranslationService"] = "Translation service",
            ["Admin.Configuration.Settings.GeneralCommon.TranslationService.Hint"] = "Choose the translation service which will be used for pre-translate functionality.",
            ["Admin.Configuration.Settings.GeneralCommon.Translation.Info"] = "<p>Pre-translate functionality allows you to use third-party translation services such as <a href='https://cloud.google.com/translate' target='_blank'>Google's Cloud Translation</a> or <a href='https://www.deepl.com/products/api' target='_blank'>DeepL</a> to automatically translate localizable fields of entities such as products and their attributes, categories, manufacturers, etc.</p><p><strong>Note:</strong> This functionality will not change any translations that you have added, or edited, manually.</p>",
            ["Admin.Common.PreTranslate"] = "Pre-translate",
            ["Enums.Nop.Core.Domain.Translation.TranslationServiceType.GoogleTranslate"] = "Google Cloud Translate",
            ["Enums.Nop.Core.Domain.Translation.TranslationServiceType.DeepL"] = "DeepL",
            ["Admin.Translation.Translated.Success"] = "Pre-translation has been completed (all possible fields are pre-filled)",
            ["Admin.Translation.Translated.Warning"] = "Errors occurred during pre-translation, see details in the log",

            //#1921
            ["Admin.Configuration.Settings.ShoppingCart.AllowMultipleWishlist"] = "Allow multiple wishlists",
            ["Admin.Configuration.Settings.ShoppingCart.AllowMultipleWishlist.Hint"] = "A value indicating whether customers can use multiple wishlists.",
            ["Admin.Configuration.Settings.ShoppingCart.MaximumNumberOfCustomWishlist"] = "Maximum number of custom wishlists",
            ["Admin.Configuration.Settings.ShoppingCart.MaximumNumberOfCustomWishlist.Hint"] = "Specify the maximum number of custom wishlists a customer can use.",
            ["Wishlist.AddNewWishlist"] = "Add new wishlist",
            ["Wishlist.AddCustomWishlist"] = "Custom wishlist",
            ["Wishlist.EnterWishlistName"] = "Enter wishlist name",
            ["Wishlist.Default"] = "Wishlist",
            ["Wishlist.MoveToCustomWishlist"] = "Move to wishlist",
            ["Wishlist.NotFound"] = "Wishlist not found.",
            ["Wishlist.DeleteWishlist"] = "Delete wishlist",
            ["Wishlist.SelectWishlist"] = "Specify your wishlist",
            ["Wishlist.MaximumNumberReached"] = "You cannot create more than {0} custom wishlists.",
            ["Wishlist.NameRequired"] = "A wishlist name is required.",
            ["Products.ProductHasBeenAddedToTheCustomWishlist.Link"] = "The product has been added to your <a href=\"{0}\">{1}</a>",
            ["Products.ProductHasBeenAddedToTheWishlistAndMoved.Link"] = "The product has been added to your <a href=\"{0}\">wishlist</a>. Want to move it to a <a href=\"#\" onclick=\"{1}\">custom wishlist</a>?",
            ["Wishlist.MultipleWishlistNotForGuest"] = "The multiple wishlist functionality is only available to registered customers.",
            ["Wishlist.NotAllowMultipleWishlist"] = "Multiple wishlist functionality is disabled.",

            //#7739
            ["Admin.System.Maintenance.DeleteThumbFiles.FilesCount"] = "Total files: {0}",
            ["Admin.System.Maintenance.DeleteThumbFiles"] = "Delete image thumbs",
            ["Admin.System.Maintenance.DeleteThumbFiles.FilesSize"] = "Total file size: {0} MB",
            ["Admin.System.Maintenance.DeleteThumbFiles.Text"] = "Delete image thumbs from the thumbs directory. All files except the placeholder.txt file.",
            ["Admin.System.Maintenance.DeleteThumbFiles.IsNotSupported"] = "Delete image thumbs is not supported by current picture thumb service.",
            ["Admin.System.Maintenance.DeleteThumbFiles.Deleted"] = "All image thumbs were deleted.",

            //#7730
            ["Admin.Catalog.Products.AiGenerateFullDescription"] = "Generate product description with AI",
            ["Admin.Catalog.Products.AiFullDescription.Generate"] = "Generate description with AI",
            ["Admin.Catalog.Products.AiFullDescription.ProductName"] = "Product name",
            ["Admin.Catalog.Products.AiFullDescription.ProductName.Placeholder"] = "Enter the product name",
            ["Admin.Catalog.Products.AiFullDescription.Keywords"] = "Features and keywords",
            ["Admin.Catalog.Products.AiFullDescription.Keywords.Placeholder"] = "Enter some features or keywords that will be used to generate description",
            ["Admin.Catalog.Products.AiFullDescription.ToneOfVoice"] = "Tone of voice",
            ["Admin.Catalog.Products.AiFullDescription.CustomToneOfVoice"] = "Custom tone of voice",
            ["Admin.Catalog.Products.AiFullDescription.CustomToneOfVoice.Placeholder"] = "e.g. Smart and funny tone of voice",
            ["Admin.Catalog.Products.AiFullDescription.Instructions"] = "Special instruction (optional)",
            ["Admin.Catalog.Products.AiFullDescription.Instructions.Placeholder"] = "e.g. Add a laptop icon at the beginning of description",
            ["Admin.Catalog.Products.AiFullDescription.GeneratedDescription"] = "Generated description",
            ["Admin.Catalog.Products.AiFullDescription.CopyToClipboard"] = "Copy to clipboard",
            ["Admin.Catalog.Products.AiFullDescription.Copied"] = "Generated description has been copied into the clipboard",
            ["ArtificialIntelligence.ToneOfVoice.Expert"] = "strong expert tone of voice",
            ["ArtificialIntelligence.ToneOfVoice.Supportive"] = "supportive and polite tone of voice",
            ["ArtificialIntelligence.CreateProductFailed"] = "<p style='color:red'>The request to the artificial intelligence service ended with the \"<strong>{0}</strong>\" error, you can see the details in the <a href='/Admin/Log/List' target='_blank'>logs</a><p>",
            ["Admin.Configuration.Settings.Catalog.BlockTitle.ArtificialIntelligence"] = "Artificial Intelligence",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.Enable"] = "Enable artificial intelligence",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.Enable.Hint"] = "Check to enable artificial intelligence services.",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.ProviderType"] = "Provider type",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.ProviderType.Hint"] = "Select artificial intelligence provider service.",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.GeminiApiKey"] = "API key",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.GeminiApiKey.Hint"] = "Set the Gemini API key.",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.ChatGptApiKey"] = "API key",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.ChatGptApiKey.Hint"] = "Set the ChatGPT API key.",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.DeepSeekApiKey"] = "API key",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.DeepSeekApiKey.Hint"] = "Set the DeepSeek API key.",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.Info"] = "<p>Artificial intelligence functionality allows you to use third-party AI services such as <a href='https://www.deepseek.com/' target='_blank'>DeepSeek</a>, <a href='https://gemini.google.com/' target='_blank'>Gemini</a> or <a href='https://chatgpt.com/' target='_blank'>ChatGPT</a> to automatically generate product descriptions.</p>",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.GeminiApiKey.Instruction"] = "<p>To use the Gemini API, you need an API key. You can create a key with a few clicks in <a href='https://aistudio.google.com/app/apikey' target='_blank'>Google AI Studio</a>.</p>",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.ChatGptApiKey.Instruction"] = "<p>Create, manage, and learn more about ChatGPT API keys in your <a href='https://platform.openai.com/settings/organization/api-keys' target='_blank'>organization settings</a>.</p>",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.DeepSeekApiKey.Instruction"] = "<p>To use the DeepSeek API, you need an API key. You can create a key <a href='https://platform.deepseek.com/api_keys' target='_blank'>here</a>.</p>",
            ["Enums.Nop.Core.Domain.ArtificialIntelligence.ToneOfVoiceType.Expert"] = "Expert",
            ["Enums.Nop.Core.Domain.ArtificialIntelligence.ToneOfVoiceType.Supportive"] = "Supportive",
            ["Enums.Nop.Core.Domain.ArtificialIntelligence.ToneOfVoiceType.Custom"] = "Custom",
            ["Enums.Nop.Core.Domain.ArtificialIntelligence.ArtificialIntelligenceProviderType.Gemini"] = "Gemini",
            ["Enums.Nop.Core.Domain.ArtificialIntelligence.ArtificialIntelligenceProviderType.ChatGpt"] = "ChatGPT (OpenAI)",
            ["Enums.Nop.Core.Domain.ArtificialIntelligence.ArtificialIntelligenceProviderType.DeepSeek"] = "DeepSeek",
            ["Admin.Catalog.Products.AiFullDescription.ProductName.Required"] = "Please provide a product name.",
            ["Admin.Catalog.Products.AiFullDescription.Keywords.Required"] = "Please provide a couple of features and keywords.",
            ["Admin.Catalog.Products.AiFullDescription.CustomToneOfVoice.Required"] = "Please choose the tone of voice instructions.",
            ["Admin.Catalog.Products.AiFullDescription.Language"] = "Target language",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.ProductDescriptionQuery"] = "Artificial intelligence query",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.ProductDescriptionQuery.Hint"] = "Set the query for create product description with artificial intelligence service.",

            //#5986
            ["Admin.Configuration.Settings.Media.PicturePath"] = "Path to the picture files",
            ["Admin.Configuration.Settings.Media.PicturePath.Hint"] = "Set up the path on the file system to store the picture files",
            ["Admin.Configuration.Settings.Media.PicturePath.Move"] = "Move pictures",
            ["Admin.Configuration.Settings.Media.PicturePath.NotGrantedPermission"] = "The '{0}' account is not granted with Modify permission on folder '{1}'. Please configure these permissions.",
            ["Admin.Configuration.Settings.Media.ChangePicturePath.Note"] = "You can use either an absolute or a relative path to the directory. However, the relative path will always be inside the wwwroot directory.<br /> <strong>Attention!</strong> We strongly recommend that you create a backup copy of your site before changing this setting.<br /> Also, note that after changing the directory, the site will be restarted.",
            ["Admin.Configuration.Settings.Media.ChangePicturePath.TryAgain"] = "If possible, please fix the problem and try again.",

            //7807
            ["Forum.Post.Text"] = "Post",
        }, languageId);

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
