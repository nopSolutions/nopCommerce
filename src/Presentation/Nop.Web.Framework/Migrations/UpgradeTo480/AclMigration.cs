using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Security;

namespace Nop.Web.Framework.Migrations.UpgradeTo480;

[NopMigration("2023-05-21 12:00:00", "ACL. Added advanced permissions")]
public class AclMigration : Migration
{
    protected readonly IRepository<Language> _languageRepository;
    protected readonly IRepository<LocaleStringResource> _localeStringRepository;
    protected readonly IRepository<PermissionRecord> _permissionRepository;
    protected readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionRecordCustomerRoleMappingRepository;

    public AclMigration(IRepository<Language> languageRepository,
        IRepository<LocaleStringResource> localeStringRepository,
        IRepository<PermissionRecord> permissionRepository,
        IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository)
    {
        _languageRepository = languageRepository;
        _localeStringRepository = localeStringRepository;
        _permissionRepository = permissionRepository;
        _permissionRecordCustomerRoleMappingRepository = permissionRecordCustomerRoleMappingRepository;
    }

    /// <summary>
    /// Gets a permission record-customer role mapping
    /// </summary>
    /// <param name="permissionId">Permission identifier</param>
    /// <returns>Permission record-customer role mapping</returns>
    protected virtual IList<PermissionRecordCustomerRoleMapping> GetMappingByPermissionRecordId(int permissionId)
    {
        var records = _permissionRecordCustomerRoleMappingRepository.Table
            .Where(x => x.PermissionRecordId == permissionId);

        return records.ToList();
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        var dbPermissions = _permissionRepository.Table
            .OrderBy(pr => pr.Id)
            .ToList();
        
        PermissionRecord getPermissionRecord(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var permissionRecord = dbPermissions
                .FirstOrDefault(pr => pr.SystemName == systemName);

            return permissionRecord;
        }
        
        void insertMappings(string oldPermissionSystemName, params string[] newPermissionSystemNames)
        {
            var record = getPermissionRecord(oldPermissionSystemName);

            if (record == null) 
                return;

            var roles = GetMappingByPermissionRecordId(record.Id)
                .Select(p => p.CustomerRoleId)
                .ToList();

            foreach (var systemName in newPermissionSystemNames)
            {
                var newPermissionRecord = getPermissionRecord(systemName);

                if (newPermissionRecord == null)
                    continue;

                foreach (var role in roles)
                    try
                    {
                        _permissionRecordCustomerRoleMappingRepository.Insert(
                            new PermissionRecordCustomerRoleMapping
                            {
                                CustomerRoleId = role,
                                PermissionRecordId = newPermissionRecord.Id
                            });
                    }
                    catch
                    {
                        //ignore
                    }
            }

            _permissionRepository.Delete(record);
        }
        
        insertMappings("AccessAdminPanel", StandardPermission.Security.ACCESS_ADMIN_PANEL);
        insertMappings("AllowCustomerImpersonation", StandardPermission.Customers.CUSTOMERS_IMPERSONATION);
        insertMappings("ManageProducts", StandardPermission.Catalog.PRODUCTS_VIEW, StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE, StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT, StandardPermission.Reports.LOW_STOCK);
        insertMappings("ManageCategories", StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE, StandardPermission.Catalog.CATEGORIES_IMPORT_EXPORT, StandardPermission.Catalog.CATEGORIES_VIEW);
        insertMappings("ManageManufacturers", StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE, StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT, StandardPermission.Catalog.MANUFACTURER_VIEW);
        insertMappings("ManageProductReviews", StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE, StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW);
        insertMappings("ManageProductTags", StandardPermission.Catalog.PRODUCT_TAGS_CREATE_EDIT_DELETE, StandardPermission.Catalog.PRODUCT_TAGS_VIEW);
        insertMappings("ManageAttributes", StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE, StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW, StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE, StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_VIEW, StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE, StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW);
        insertMappings("ManageCustomers", StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Customers.CUSTOMERS_IMPORT_EXPORT, StandardPermission.Customers.CUSTOMERS_VIEW, StandardPermission.Customers.CUSTOMER_ROLES_VIEW, StandardPermission.Customers.CUSTOMER_ROLES_CREATE_EDIT_DELETE, StandardPermission.Customers.GDPR_MANAGE, StandardPermission.Reports.REGISTERED_CUSTOMERS, StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS, StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL);
        insertMappings("ManageVendors", StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE, StandardPermission.Customers.VENDORS_VIEW);
        insertMappings("ManageCurrentCarts", StandardPermission.Orders.CURRENT_CARTS_MANAGE);
        insertMappings("ManageOrders", StandardPermission.Orders.ORDERS_VIEW, StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE, StandardPermission.Orders.ORDERS_IMPORT_EXPORT, StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE, StandardPermission.Orders.SHIPMENTS_VIEW, StandardPermission.Reports.BESTSELLERS, StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED);
        insertMappings("SalesSummaryReport", StandardPermission.Reports.SALES_SUMMARY);
        insertMappings("ManageRecurringPayments", StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE, StandardPermission.Orders.RECURRING_PAYMENTS_VIEW);
        insertMappings("ManageGiftCards", StandardPermission.Orders.GIFT_CARDS_CREATE_EDIT_DELETE, StandardPermission.Orders.GIFT_CARDS_VIEW);
        insertMappings("ManageReturnRequests", StandardPermission.Orders.RETURN_REQUESTS_CREATE_EDIT_DELETE, StandardPermission.Orders.RETURN_REQUESTS_VIEW);
        insertMappings("OrderCountryReport", StandardPermission.Reports.COUNTRY_SALES);
        insertMappings("ManageAffiliates", StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE, StandardPermission.Promotions.AFFILIATES_VIEW);
        insertMappings("ManageCampaigns", StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT, StandardPermission.Promotions.CAMPAIGNS_DELETE, StandardPermission.Promotions.CAMPAIGNS_SEND_EMAILS, StandardPermission.Promotions.CAMPAIGNS_VIEW);
        insertMappings("ManageDiscounts", StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE, StandardPermission.Promotions.DISCOUNTS_VIEW);
        insertMappings("ManageNewsletterSubscribers", StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE, StandardPermission.Promotions.SUBSCRIBERS_IMPORT_EXPORT, StandardPermission.Promotions.SUBSCRIBERS_VIEW);
        insertMappings("ManagePolls", StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.POLLS_VIEW);
        insertMappings("ManageNews", StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.NEWS_COMMENTS_VIEW, StandardPermission.ContentManagement.NEWS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.NEWS_VIEW);
        insertMappings("ManageBlog", StandardPermission.ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.BLOG_COMMENTS_VIEW, StandardPermission.ContentManagement.BLOG_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.BLOG_VIEW);
        insertMappings("ManageWidgets", StandardPermission.Configuration.MANAGE_WIDGETS);
        insertMappings("ManageTopics", StandardPermission.ContentManagement.TOPICS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.TOPICS_VIEW);
        insertMappings("ManageForums", StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.FORUMS_VIEW);
        insertMappings("ManageMessageTemplates", StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE, StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW);
        insertMappings("ManageCountries", StandardPermission.Configuration.MANAGE_COUNTRIES);
        insertMappings("ManageLanguages", StandardPermission.Configuration.MANAGE_LANGUAGES);
        insertMappings("ManageSettings", StandardPermission.Configuration.MANAGE_SETTINGS);
        insertMappings("ManagePaymentMethods", StandardPermission.Configuration.MANAGE_PAYMENT_METHODS);
        insertMappings("ManageExternalAuthenticationMethods", StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS);
        insertMappings("ManageMultifactorAuthenticationMethods", StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS);
        insertMappings("ManageTaxSettings", StandardPermission.Configuration.MANAGE_TAX_SETTINGS);
        insertMappings("ManageShippingSettings", StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS);
        insertMappings("ManageCurrencies", StandardPermission.Configuration.MANAGE_CURRENCIES);
        insertMappings("ManageActivityLog", StandardPermission.Customers.ACTIVITY_LOG_DELETE, StandardPermission.Customers.ACTIVITY_LOG_MANAGE_TYPES, StandardPermission.Customers.ACTIVITY_LOG_VIEW);
        insertMappings("ManageACL", StandardPermission.Configuration.MANAGE_ACL);
        insertMappings("ManageEmailAccounts", StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS);
        insertMappings("ManageStores", StandardPermission.Configuration.MANAGE_STORES);
        insertMappings("ManagePlugins", StandardPermission.Configuration.MANAGE_PLUGINS);
        insertMappings("ManageSystemLog", StandardPermission.System.MANAGE_SYSTEM_LOG);
        insertMappings("ManageMessageQueue", StandardPermission.System.MANAGE_MESSAGE_QUEUE);
        insertMappings("ManageMaintenance", StandardPermission.System.MANAGE_MAINTENANCE);
        insertMappings("HtmlEditor.ManagePictures", StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES);
        insertMappings("ManageScheduleTasks", StandardPermission.System.MANAGE_SCHEDULE_TASKS);
        insertMappings("ManageAppSettings", StandardPermission.System.MANAGE_APP_SETTINGS);

        insertMappings("DisplayPrices", StandardPermission.PublicStore.DISPLAY_PRICES);
        insertMappings("EnableShoppingCart", StandardPermission.PublicStore.ENABLE_SHOPPING_CART);
        insertMappings("EnableWishlist", StandardPermission.PublicStore.ENABLE_WISHLIST);
        insertMappings("PublicStoreAllowNavigation", StandardPermission.PublicStore.PUBLIC_STORE_ALLOW_NAVIGATION);
        insertMappings("AccessClosedStore", StandardPermission.PublicStore.ACCESS_CLOSED_STORE);

        insertMappings("EnableMultiFactorAuthentication", StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}