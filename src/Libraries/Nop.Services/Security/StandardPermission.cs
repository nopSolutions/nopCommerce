namespace Nop.Services.Security;

/// <summary>
/// Standard permission
/// </summary>
public partial class StandardPermission
{
    public partial class Customers
    {
        public const string GDPR_MANAGE = $"{nameof(Customers)}.GDPRManage";
        public const string CUSTOMERS_VIEW = $"{nameof(Customers)}.CustomersView";
        public const string CUSTOMERS_CREATE_EDIT_DELETE = $"{nameof(Customers)}.CustomersCreateEditDelete";
        public const string CUSTOMERS_IMPORT_EXPORT = $"{nameof(Customers)}.CustomersImportExport";
        public const string CUSTOMERS_IMPERSONATION = $"{nameof(Customers)}.CustomersImpersonation";
        public const string CUSTOMER_ROLES_VIEW = $"{nameof(Customers)}.CustomerRolesView";
        public const string CUSTOMER_ROLES_CREATE_EDIT_DELETE = $"{nameof(Customers)}.CustomerRolesCreateEditDelete";
        public const string VENDORS_VIEW = $"{nameof(Customers)}.VendorsView";
        public const string VENDORS_CREATE_EDIT_DELETE = $"{nameof(Customers)}.VendorsCreateEditDelete";
        public const string ACTIVITY_LOG_VIEW = $"{nameof(Customers)}.ActivityLogView";
        public const string ACTIVITY_LOG_DELETE = $"{nameof(Customers)}.ActivityLogDelete";
        public const string ACTIVITY_LOG_MANAGE_TYPES = $"{nameof(Customers)}.ActivityLogManageTypes";
    }

    public partial class Orders
    {
        public const string CURRENT_CARTS_MANAGE = $"{nameof(Orders)}.CurrentCartsManage";
        public const string ORDERS_VIEW = $"{nameof(Orders)}.OrdersView";
        public const string ORDERS_CREATE_EDIT_DELETE = $"{nameof(Orders)}.OrdersCreateEditDelete";
        public const string ORDERS_IMPORT_EXPORT = $"{nameof(Orders)}.OrdersImportExport";
        public const string SHIPMENTS_VIEW = $"{nameof(Orders)}.ShipmentsView";
        public const string SHIPMENTS_CREATE_EDIT_DELETE = $"{nameof(Orders)}.ShipmentsCreateEditDelete";
        public const string RETURN_REQUESTS_VIEW = $"{nameof(Orders)}.ReturnRequestsView";
        public const string RETURN_REQUESTS_CREATE_EDIT_DELETE = $"{nameof(Orders)}.ReturnRequestsCreateEditDelete";
        public const string RECURRING_PAYMENTS_VIEW = $"{nameof(Orders)}.RecurringPaymentsView";
        public const string RECURRING_PAYMENTS_CREATE_EDIT_DELETE = $"{nameof(Orders)}.RecurringPaymentsCreateEditDelete";
        public const string GIFT_CARDS_VIEW = $"{nameof(Orders)}.GiftCardsView";
        public const string GIFT_CARDS_CREATE_EDIT_DELETE = $"{nameof(Orders)}.GiftCardsCreateEditDelete";
    }

    public partial class Reports
    {
        public const string SALES_SUMMARY = $"{nameof(Reports)}.SalesSummary";
        public const string COUNTRY_SALES = $"{nameof(Reports)}.CountrySales";
        public const string LOW_STOCK = $"{nameof(Reports)}.LowStock";
        public const string BESTSELLERS = $"{nameof(Reports)}.Bestsellers";
        public const string PRODUCTS_NEVER_PURCHASED = $"{nameof(Reports)}.ProductsNeverPurchased";
        public const string REGISTERED_CUSTOMERS = $"{nameof(Reports)}.RegisteredCustomers";
        public const string CUSTOMERS_BY_ORDER_TOTAL = $"{nameof(Reports)}.CustomersByOrderTotal";
        public const string CUSTOMERS_BY_NUMBER_OF_ORDERS = $"{nameof(Reports)}.CustomersByNumberOfOrders";
    }

    public partial class Catalog
    {
        public const string PRODUCTS_VIEW = $"{nameof(Catalog)}.ProductsView";
        public const string PRODUCTS_CREATE_EDIT_DELETE = $"{nameof(Catalog)}.ProductsCreateEditDelete";
        public const string PRODUCTS_IMPORT_EXPORT = $"{nameof(Catalog)}.ProductsImportExport";
        public const string CATEGORIES_VIEW = $"{nameof(Catalog)}.CategoriesView";
        public const string CATEGORIES_CREATE_EDIT_DELETE = $"{nameof(Catalog)}.CategoriesCreateEditDelete";
        public const string CATEGORIES_IMPORT_EXPORT = $"{nameof(Catalog)}.CategoriesImportExport";
        public const string MANUFACTURER_VIEW = $"{nameof(Catalog)}.ManufacturerView";
        public const string MANUFACTURER_CREATE_EDIT_DELETE = $"{nameof(Catalog)}.ManufacturerCreateEditDelete";
        public const string MANUFACTURER_IMPORT_EXPORT = $"{nameof(Catalog)}.ManufacturerImportExport";
        public const string PRODUCT_REVIEWS_VIEW = $"{nameof(Catalog)}.ProductReviewsView";
        public const string PRODUCT_REVIEWS_CREATE_EDIT_DELETE = $"{nameof(Catalog)}.ProductReviewsCreateEditDelete";
        public const string PRODUCT_TAGS_VIEW = $"{nameof(Catalog)}.ProductTagsView";
        public const string PRODUCT_TAGS_CREATE_EDIT_DELETE = $"{nameof(Catalog)}.ProductTagsCreateEditDelete";
        public const string PRODUCT_ATTRIBUTES_VIEW = $"{nameof(Catalog)}.ProductAttributesView";
        public const string PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE = $"{nameof(Catalog)}.ProductAttributesCreateEditDelete";
        public const string SPECIFICATION_ATTRIBUTES_VIEW = $"{nameof(Catalog)}.SpecificationAttributesView";
        public const string SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE = $"{nameof(Catalog)}.SpecificationAttributesCreateEditDelete";
        public const string CHECKOUT_ATTRIBUTES_VIEW = $"{nameof(Catalog)}.CheckoutAttributesView";
        public const string CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE = $"{nameof(Catalog)}.CheckoutAttributesCreateEditDelete";
    }

    public partial class Promotions
    {
        public const string DISCOUNTS_VIEW = $"{nameof(Promotions)}.DiscountsView";
        public const string DISCOUNTS_CREATE_EDIT_DELETE = $"{nameof(Promotions)}.DiscountsCreateEditDelete";
        public const string AFFILIATES_VIEW = $"{nameof(Promotions)}.AffiliatesView";
        public const string AFFILIATES_CREATE_EDIT_DELETE = $"{nameof(Promotions)}.AffiliatesCreateEditDelete";
        public const string SUBSCRIBERS_VIEW = $"{nameof(Promotions)}.SubscribersView";
        public const string SUBSCRIBERS_CREATE_EDIT_DELETE = $"{nameof(Promotions)}.SubscribersCreateEditDelete";
        public const string SUBSCRIBERS_IMPORT_EXPORT = $"{nameof(Promotions)}.SubscribersImportExport";
        public const string CAMPAIGNS_VIEW = $"{nameof(Promotions)}.CampaignsView";
        public const string CAMPAIGNS_CREATE_EDIT = $"{nameof(Promotions)}.CampaignsCreateEdit";
        public const string CAMPAIGNS_DELETE = $"{nameof(Promotions)}.CampaignsDelete";
        public const string CAMPAIGNS_SEND_EMAILS = $"{nameof(Promotions)}.CampaignsSendEmails";
    }

    public partial class ContentManagement
    {
        public const string TOPICS_VIEW = $"{nameof(ContentManagement)}.TopicsView";
        public const string TOPICS_CREATE_EDIT_DELETE = $"{nameof(ContentManagement)}.TopicsCreateEditDelete";
        public const string MESSAGE_TEMPLATES_VIEW = $"{nameof(ContentManagement)}.MessageTemplatesView";
        public const string MESSAGE_TEMPLATES_CREATE_EDIT_DELETE = $"{nameof(ContentManagement)}.MessageTemplatesCreateEditDelete";
        public const string NEWS_VIEW = $"{nameof(ContentManagement)}.NewsView";
        public const string NEWS_CREATE_EDIT_DELETE = $"{nameof(ContentManagement)}.NewsCreateEditDelete";
        public const string NEWS_COMMENTS_VIEW = $"{nameof(ContentManagement)}.NewsCommentsView";
        public const string NEWS_COMMENTS_CREATE_EDIT_DELETE = $"{nameof(ContentManagement)}.NewsCommentsCreateEditDelete";
        public const string BLOG_VIEW = $"{nameof(ContentManagement)}.BlogView";
        public const string BLOG_CREATE_EDIT_DELETE = $"{nameof(ContentManagement)}.BlogCreateEditDelete";
        public const string BLOG_COMMENTS_VIEW = $"{nameof(ContentManagement)}.BlogCommentsView";
        public const string BLOG_COMMENTS_CREATE_EDIT_DELETE = $"{nameof(ContentManagement)}.BlogCommentsCreateEditDelete";
        public const string POLLS_VIEW = $"{nameof(ContentManagement)}.PollsView";
        public const string POLLS_CREATE_EDIT_DELETE = $"{nameof(ContentManagement)}.PollsCreateEditDelete";
        public const string FORUMS_VIEW = $"{nameof(ContentManagement)}.ForumsView";
        public const string FORUMS_CREATE_EDIT_DELETE = $"{nameof(ContentManagement)}.ForumsCreateEditDelete";
    }

    public partial class Configuration
    {
        public const string MANAGE_WIDGETS = $"{nameof(Configuration)}.ManageWidgets";
        public const string MANAGE_COUNTRIES = $"{nameof(Configuration)}.ManageCountries";
        public const string MANAGE_LANGUAGES = $"{nameof(Configuration)}.ManageLanguages";
        public const string MANAGE_SETTINGS = $"{nameof(Configuration)}.ManageSettings";
        public const string MANAGE_PAYMENT_METHODS = $"{nameof(Configuration)}.ManagePaymentMethods";
        public const string MANAGE_EXTERNAL_AUTHENTICATION_METHODS = $"{nameof(Configuration)}.ManageExternalAuthenticationMethods";
        public const string MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS = $"{nameof(Configuration)}.ManageMultifactorAuthenticationMethods";
        public const string MANAGE_TAX_SETTINGS = $"{nameof(Configuration)}.ManageTaxSettings";
        public const string MANAGE_SHIPPING_SETTINGS = $"{nameof(Configuration)}.ManageShippingSettings";
        public const string MANAGE_CURRENCIES = $"{nameof(Configuration)}.ManageCurrencies";
        public const string MANAGE_ACL = $"{nameof(Configuration)}.ManageACL";
        public const string MANAGE_EMAIL_ACCOUNTS = $"{nameof(Configuration)}.ManageEmailAccounts";
        public const string MANAGE_STORES = $"{nameof(Configuration)}.ManageStores";
        public const string MANAGE_PLUGINS = $"{nameof(Configuration)}.ManagePlugins";
    }

    public partial class System
    {
        public const string MANAGE_SYSTEM_LOG = $"{nameof(System)}.ManageSystemLog";
        public const string MANAGE_MESSAGE_QUEUE = $"{nameof(System)}.ManageMessageQueue";
        public const string MANAGE_MAINTENANCE = $"{nameof(System)}.ManageMaintenance";
        public const string HTML_EDITOR_MANAGE_PICTURES = $"{nameof(System)}.HtmlEditor.ManagePictures";
        public const string MANAGE_SCHEDULE_TASKS = $"{nameof(System)}.ManageScheduleTasks";
        public const string MANAGE_APP_SETTINGS = $"{nameof(System)}.ManageAppSettings";
    }

    public partial class Security
    {
        public const string ENABLE_MULTI_FACTOR_AUTHENTICATION = $"{nameof(Security)}.EnableMultiFactorAuthentication";
        public const string ACCESS_ADMIN_PANEL = $"{nameof(Security)}.AccessAdminPanel";
    }

    public partial class PublicStore
    {
        public const string DISPLAY_PRICES = $"{nameof(PublicStore)}.DisplayPrices";
        public const string ENABLE_SHOPPING_CART = $"{nameof(PublicStore)}.EnableShoppingCart";
        public const string ENABLE_WISHLIST = $"{nameof(PublicStore)}.EnableWishlist";
        public const string PUBLIC_STORE_ALLOW_NAVIGATION = $"{nameof(PublicStore)}.PublicStoreAllowNavigation";
        public const string ACCESS_CLOSED_STORE = $"{nameof(PublicStore)}.AccessClosedStore";
    }
}