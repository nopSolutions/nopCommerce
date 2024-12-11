using Nop.Core.Domain.Customers;
using static Nop.Services.Security.StandardPermission;

namespace Nop.Services.Security;

/// <summary>
/// Default permission config manager
/// </summary>
public partial class DefaultPermissionConfigManager : IPermissionConfigManager
{
    public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
    {
        #region Security
        
        new ("Security. Enable Multi-factor authentication", StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION, nameof(StandardPermission.Security), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName),
        new ("Access admin area", StandardPermission.Security.ACCESS_ADMIN_PANEL, nameof(StandardPermission.Security), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),

        #endregion

        #region Customers
        
        new ("Admin area. Customers. View", StandardPermission.Customers.CUSTOMERS_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Customers. Create, edit, delete", StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Customers. Import and export", StandardPermission.Customers.CUSTOMERS_IMPORT_EXPORT, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Customers. Allow impersonation", StandardPermission.Customers.CUSTOMERS_IMPERSONATION, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Customer roles. View", StandardPermission.Customers.CUSTOMER_ROLES_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Customer roles. Create, edit, delete", StandardPermission.Customers.CUSTOMER_ROLES_CREATE_EDIT_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Vendors. View", StandardPermission.Customers.VENDORS_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Vendors. Create, edit, delete", StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Activity Log. View", StandardPermission.Customers.ACTIVITY_LOG_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Activity Log. Delete", StandardPermission.Customers.ACTIVITY_LOG_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Activity Log. Manage types", StandardPermission.Customers.ACTIVITY_LOG_MANAGE_TYPES, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. GDPR. Manage", StandardPermission.Customers.GDPR_MANAGE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Orders
        
        new ("Admin area. Current Carts. Manage", StandardPermission.Orders.CURRENT_CARTS_MANAGE, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Orders. View", StandardPermission.Orders.ORDERS_VIEW, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Orders. Create, edit, delete", StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Orders. Import and export", StandardPermission.Orders.ORDERS_IMPORT_EXPORT, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Shipments. View", StandardPermission.Orders.SHIPMENTS_VIEW, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Shipments. Create, edit, delete", StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Return requests. View", StandardPermission.Orders.RETURN_REQUESTS_VIEW, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Return requests. Create, edit, delete", StandardPermission.Orders.RETURN_REQUESTS_CREATE_EDIT_DELETE, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Recurring payments. View", StandardPermission.Orders.RECURRING_PAYMENTS_VIEW, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Recurring payments. Create, edit, delete", StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Gift cards. View", StandardPermission.Orders.GIFT_CARDS_VIEW, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Gift cards. Create, edit, delete", StandardPermission.Orders.GIFT_CARDS_CREATE_EDIT_DELETE, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Reports
        
        new ("Admin area. Reports. Sales summary", Reports.SALES_SUMMARY, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Reports. Country sales", Reports.COUNTRY_SALES, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Reports. Low stock", Reports.LOW_STOCK, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Reports. Bestsellers", Reports.BESTSELLERS, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Reports. Products never purchased", Reports.PRODUCTS_NEVER_PURCHASED, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Reports. Registered customers", Reports.REGISTERED_CUSTOMERS, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Reports. Customers by order total", Reports.CUSTOMERS_BY_ORDER_TOTAL, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Reports. Customers by number of orders", Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Catalog
        
        new ("Admin area. Products. View", StandardPermission.Catalog.PRODUCTS_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Products. Create, edit, delete", StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Products. Import and export", StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Categories. View", StandardPermission.Catalog.CATEGORIES_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Categories. Create, edit, delete", StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Categories. Import and export", StandardPermission.Catalog.CATEGORIES_IMPORT_EXPORT, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Manufacturer. View", StandardPermission.Catalog.MANUFACTURER_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Manufacturer. Create, edit, delete", StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Manufacturer. Import and export", StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Product reviews. View", StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Product reviews. Create, edit, delete", StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Product tags. View", StandardPermission.Catalog.PRODUCT_TAGS_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Product tags. Create, edit, delete", StandardPermission.Catalog.PRODUCT_TAGS_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Product attributes. View", StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Product attributes. Create, edit, delete", StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Specification attributes. View", StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Specification attributes. Create, edit, delete", StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Checkout attributes. View", StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Checkout attributes. Create, edit, delete", StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Promotions
        
        new ("Admin area. Discounts. View", StandardPermission.Promotions.DISCOUNTS_VIEW, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Discounts. Create, edit, delete", StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Admin area. Affiliates. View", StandardPermission.Promotions.AFFILIATES_VIEW, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Affiliates. Create, edit, delete", StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Newsletter Subscribers. View", StandardPermission.Promotions.SUBSCRIBERS_VIEW, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Newsletter Subscribers. Create, edit, delete", StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Newsletter Subscribers. Import and export", StandardPermission.Promotions.SUBSCRIBERS_IMPORT_EXPORT, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Campaigns. View", StandardPermission.Promotions.CAMPAIGNS_VIEW, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Campaigns. Create and Edit", StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Campaigns. Delete", StandardPermission.Promotions.CAMPAIGNS_DELETE, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Campaigns. Send emails", StandardPermission.Promotions.CAMPAIGNS_SEND_EMAILS, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Content management
        
        new ("Admin area. Topics. View", StandardPermission.ContentManagement.TOPICS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Topics. Create, edit, delete", StandardPermission.ContentManagement.TOPICS_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Message Templates. View", StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Message Templates. Create, edit, delete", StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. News. View", StandardPermission.ContentManagement.NEWS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. News. Create, edit, delete", StandardPermission.ContentManagement.NEWS_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. News comments. View", StandardPermission.ContentManagement.NEWS_COMMENTS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. News comments. Create, edit, delete", StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Blog. View", StandardPermission.ContentManagement.BLOG_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Blog. Create, edit, delete", StandardPermission.ContentManagement.BLOG_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Blog comments. View", StandardPermission.ContentManagement.BLOG_COMMENTS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Blog comments. Create, edit, delete", StandardPermission.ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Polls. View", StandardPermission.ContentManagement.POLLS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Polls. Create, edit, delete", StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Forums. View", StandardPermission.ContentManagement.FORUMS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Forums. Create, edit, delete", StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Configuration
        
        new ("Admin area. Widgets. Manage", StandardPermission.Configuration.MANAGE_WIDGETS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Countries. Manage", StandardPermission.Configuration.MANAGE_COUNTRIES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Languages. Manage", StandardPermission.Configuration.MANAGE_LANGUAGES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Settings. Manage", StandardPermission.Configuration.MANAGE_SETTINGS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Payment Methods. Manage", StandardPermission.Configuration.MANAGE_PAYMENT_METHODS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. External Authentication Methods. Manage", StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Multi-factor Authentication Methods. Manage", StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Tax Settings. Manage", StandardPermission.Configuration.MANAGE_TAX_SETTINGS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Shipping Settings. Manage", StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Currencies. Manage", StandardPermission.Configuration.MANAGE_CURRENCIES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. ACL. Manage", StandardPermission.Configuration.MANAGE_ACL, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Email Accounts. Manage", StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Stores. Manage", StandardPermission.Configuration.MANAGE_STORES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Plugins. Manage", StandardPermission.Configuration.MANAGE_PLUGINS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        
        #endregion

        #region System
        
        new ("Admin area. System Log. Manage", StandardPermission.System.MANAGE_SYSTEM_LOG, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Message Queue. Manage", StandardPermission.System.MANAGE_MESSAGE_QUEUE, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Maintenance. Manage", StandardPermission.System.MANAGE_MAINTENANCE, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. HTML Editor. Manage pictures", StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Schedule Tasks. Manage", StandardPermission.System.MANAGE_SCHEDULE_TASKS, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. App Settings. Manage", StandardPermission.System.MANAGE_APP_SETTINGS, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),

        #endregion
        
        #region Public store
        
        new ("Public store. Display Prices", StandardPermission.PublicStore.DISPLAY_PRICES, nameof(StandardPermission.PublicStore), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName, NopCustomerDefaults.GuestsRoleName, NopCustomerDefaults.ForumModeratorsRoleName),
        new ("Public store. Enable shopping cart", StandardPermission.PublicStore.ENABLE_SHOPPING_CART, nameof(StandardPermission.PublicStore), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName, NopCustomerDefaults.GuestsRoleName, NopCustomerDefaults.ForumModeratorsRoleName),
        new ("Public store. Enable wishlist", StandardPermission.PublicStore.ENABLE_WISHLIST, nameof(StandardPermission.PublicStore), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName, NopCustomerDefaults.GuestsRoleName, NopCustomerDefaults.ForumModeratorsRoleName),
        new ("Public store. Allow navigation", StandardPermission.PublicStore.PUBLIC_STORE_ALLOW_NAVIGATION, nameof(StandardPermission.PublicStore), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName, NopCustomerDefaults.GuestsRoleName, NopCustomerDefaults.ForumModeratorsRoleName),
        new ("Public store. Access a closed store", StandardPermission.PublicStore.ACCESS_CLOSED_STORE, nameof(StandardPermission.PublicStore), NopCustomerDefaults.AdministratorsRoleName),

        #endregion
    };
}