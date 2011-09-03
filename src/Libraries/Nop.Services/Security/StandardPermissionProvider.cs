using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "Access admin area", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord ManageCatalog = new PermissionRecord { Name = "Manage Catalog", SystemName = "ManageCatalog", Category = "Catalog" };
        public static readonly PermissionRecord ManageCustomers = new PermissionRecord { Name = "Manage Customers", SystemName = "ManageCustomers", Category = "Customers" };
        public static readonly PermissionRecord ManageCustomerRoles = new PermissionRecord { Name = "Manage Customer Roles", SystemName = "ManageCustomerRoles", Category = "Customers" };
        public static readonly PermissionRecord ManageOrders = new PermissionRecord { Name = "Manage Orders", SystemName = "ManageOrders", Category = "Orders" };
        public static readonly PermissionRecord ManageGiftCards = new PermissionRecord { Name = "Manage Gift Cards", SystemName = "ManageGiftCards", Category = "Orders" };
        public static readonly PermissionRecord ManageReturnRequests = new PermissionRecord { Name = "Manage Return Requests", SystemName = "ManageReturnRequests", Category = "Orders" };
        public static readonly PermissionRecord ManageAffiliates = new PermissionRecord { Name = "Manage Affiliates", SystemName = "ManageAffiliates", Category = "Promo" };
        public static readonly PermissionRecord ManageCampaigns = new PermissionRecord { Name = "Manage Campaigns", SystemName = "ManageCampaigns", Category = "Promo" };
        public static readonly PermissionRecord ManageDiscounts = new PermissionRecord { Name = "Manage Discounts", SystemName = "ManageDiscounts", Category = "Promo" };
        public static readonly PermissionRecord ManagePromotionFeeds = new PermissionRecord { Name = "Manage Promotion Feeds", SystemName = "ManagePromotionFeeds", Category = "Promo" };
        public static readonly PermissionRecord ManageNewsletterSubscribers = new PermissionRecord { Name = "Manage Newsletter Subscribers", SystemName = "ManageNewsletterSubscribers", Category = "Promo" };
        public static readonly PermissionRecord ManagePolls = new PermissionRecord { Name = "Manage Polls", SystemName = "ManagePolls", Category = "Content Management" };
        public static readonly PermissionRecord ManageNews = new PermissionRecord { Name = "Manage News", SystemName = "ManageNews", Category = "Content Management" };
        public static readonly PermissionRecord ManageBlog = new PermissionRecord { Name = "Manage Blog", SystemName = "ManageBlog", Category = "Content Management" };
        public static readonly PermissionRecord ManageWidgets = new PermissionRecord { Name = "Manage Widgets", SystemName = "ManageWidgets", Category = "Content Management" };
        public static readonly PermissionRecord ManageTopics = new PermissionRecord { Name = "Manage Topics", SystemName = "ManageTopics", Category = "Content Management" };
        public static readonly PermissionRecord ManageForums = new PermissionRecord { Name = "Manage Forums", SystemName = "ManageForums", Category = "Content Management" };
        public static readonly PermissionRecord ManageMessageTemplates = new PermissionRecord { Name = "Manage Message Templates", SystemName = "ManageMessageTemplates", Category = "Content Management" };
        public static readonly PermissionRecord ManageCountries = new PermissionRecord { Name = "Manage Countries", SystemName = "ManageCountries", Category = "Configuration" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "Manage Languages", SystemName = "ManageLanguages", Category = "Configuration" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "Manage Settings", SystemName = "ManageSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManagePaymentMethods = new PermissionRecord { Name = "Manage Payment Methods", SystemName = "ManagePaymentMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManageExternalAuthenticationMethods = new PermissionRecord { Name = "Manage External Authentication Methods", SystemName = "ManageExternalAuthenticationMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManageTaxSettings = new PermissionRecord { Name = "Manage Tax Settings", SystemName = "ManageTaxSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageShippingSettings = new PermissionRecord { Name = "Manage Shipping Settings", SystemName = "ManageShippingSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageCurrencies = new PermissionRecord { Name = "Manage Currencies", SystemName = "ManageCurrencies", Category = "Configuration" };
        public static readonly PermissionRecord ManageMeasures = new PermissionRecord { Name = "Manage Measures", SystemName = "ManageMeasures", Category = "Configuration" };
        public static readonly PermissionRecord ManageActivityLog = new PermissionRecord { Name = "Manage Activity Log", SystemName = "ManageActivityLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageAcl = new PermissionRecord { Name = "Manage ACL", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly PermissionRecord ManageSmsProviders = new PermissionRecord { Name = "Manage SMS Providers", SystemName = "ManageSMSProviders", Category = "Configuration" };
        public static readonly PermissionRecord ManageEmailAccounts = new PermissionRecord { Name = "Manage Email Accounts", SystemName = "ManageEmailAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManagePlugins = new PermissionRecord { Name = "Manage Plugins", SystemName = "ManagePlugins", Category = "Configuration" };
        public static readonly PermissionRecord ManageSystemLog = new PermissionRecord { Name = "Manage System Log", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageQueue = new PermissionRecord { Name = "Manage Message Queue", SystemName = "ManageMessageQueue", Category = "Configuration" };
        public static readonly PermissionRecord ManageMaintenance = new PermissionRecord { Name = "Manage Maintenance", SystemName = "ManageMaintenance", Category = "Configuration" };

        
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[] 
            {
                AccessAdminPanel,
                ManageCatalog,
                ManageCustomers,
                ManageCustomerRoles,
                ManageOrders,
                ManageGiftCards,
                ManageReturnRequests,
                ManageAffiliates,
                ManageCampaigns,
                ManageDiscounts,
                ManagePromotionFeeds,
                ManageNewsletterSubscribers,
                ManagePolls,
                ManageNews,
                ManageBlog,
                ManageWidgets,
                ManageTopics,
                ManageForums,
                ManageMessageTemplates,
                ManageCountries,
                ManageLanguages,
                ManageSettings,
                ManagePaymentMethods,
                ManageExternalAuthenticationMethods,
                ManageTaxSettings,
                ManageShippingSettings,
                ManageCurrencies,
                ManageMeasures,
                ManageActivityLog,
                ManageAcl,
                ManageSmsProviders,
                ManageEmailAccounts,
                ManagePlugins,
                ManageSystemLog,
                ManageMessageQueue,
                ManageMaintenance,
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[] 
            {
                new DefaultPermissionRecord 
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Administrators,
                    PermissionRecords = new[] 
                    {
                        AccessAdminPanel,
                        ManageCatalog,
                        ManageCustomers,
                        ManageCustomerRoles,
                        ManageOrders,
                        ManageGiftCards,
                        ManageReturnRequests,
                        ManageAffiliates,
                        ManageCampaigns,
                        ManageDiscounts,
                        ManagePromotionFeeds,
                        ManageNewsletterSubscribers,
                        ManagePolls,
                        ManageNews,
                        ManageBlog,
                        ManageWidgets,
                        ManageTopics,
                        ManageForums,
                        ManageMessageTemplates,
                        ManageCountries,
                        ManageLanguages,
                        ManageSettings,
                        ManagePaymentMethods,
                        ManageExternalAuthenticationMethods,
                        ManageTaxSettings,
                        ManageShippingSettings,
                        ManageCurrencies,
                        ManageMeasures,
                        ManageActivityLog,
                        ManageAcl,
                        ManageSmsProviders,
                        ManageEmailAccounts,
                        ManagePlugins,
                        ManageSystemLog,
                        ManageMessageQueue,
                        ManageMaintenance,
                    }
                },
            };
        }
    }
}