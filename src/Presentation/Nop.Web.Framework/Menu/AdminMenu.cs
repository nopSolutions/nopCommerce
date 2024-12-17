using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Events;

namespace Nop.Web.Framework.Menu;

/// <summary>
/// Admin menu
/// </summary>
public partial class AdminMenu : IAdminMenu
{
    #region Fields

    protected AdminMenuItem _baseRootMenuItem;
    protected AdminMenuItem _rootItem;

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
#pragma warning disable CS0618 // Type or member is obsolete
    protected readonly IPluginManager<IAdminMenuPlugin> _adminMenuPluginManager;
#pragma warning restore CS0618 // Type or member is obsolete
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    public AdminMenu(IActionContextAccessor actionContextAccessor,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        IPermissionService permissionService,
#pragma warning disable CS0618 // Type or member is obsolete
        IPluginManager<IAdminMenuPlugin> adminMenuPluginManager,
#pragma warning restore CS0618 // Type or member is obsolete
        IUrlHelperFactory urlHelperFactory,
        IWorkContext workContext)
    {
        _actionContextAccessor = actionContextAccessor;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _adminMenuPluginManager = adminMenuPluginManager;
        _urlHelperFactory = urlHelperFactory;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Fills the base root menu item data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task FillBaseRootAsync()
    {
        if (_baseRootMenuItem != null)
            return;

        _baseRootMenuItem = new AdminMenuItem
        {
            SystemName = "Home",
            Title = await _localizationService.GetResourceAsync("Admin.Home"),
            Url = GetMenuItemUrl("Home", "Overview"),
            ChildNodes = new List<AdminMenuItem>
            {
                //dashboard
                new()
                {
                    SystemName = "Dashboard",
                    Title = await _localizationService.GetResourceAsync("Admin.Dashboard"),
                    Url = GetMenuItemUrl("Home", "Index"),
                    IconClass = "fas fa-desktop"
                },
                //catalog
                new()
                {
                    SystemName = "Catalog",
                    Title = await _localizationService.GetResourceAsync("Admin.Catalog"),
                    IconClass = "fas fa-book",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Products",
                            Title = await _localizationService.GetResourceAsync("Admin.Catalog.Products"),
                            PermissionNames = new List<string> { StandardPermission.Catalog.PRODUCTS_VIEW },
                            Url = GetMenuItemUrl("Product", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Categories",
                            Title = await _localizationService.GetResourceAsync("Admin.Catalog.Categories"),
                            PermissionNames = new List<string> { StandardPermission.Catalog.CATEGORIES_VIEW },
                            Url = GetMenuItemUrl("Category", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Manufacturers",
                            Title = await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers"),
                            PermissionNames = new List<string> { StandardPermission.Catalog.MANUFACTURER_VIEW },
                            Url = GetMenuItemUrl("Manufacturer", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Product reviews",
                            Title = await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews"),
                            PermissionNames = new List<string> { StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW },
                            Url = GetMenuItemUrl("ProductReview", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Product tags",
                            Title = await _localizationService.GetResourceAsync("Admin.Catalog.ProductTags"),
                            PermissionNames = new List<string> { StandardPermission.Catalog.PRODUCT_TAGS_VIEW },
                            Url = GetMenuItemUrl("Product", "ProductTags"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Attributes",
                            Title = await _localizationService.GetResourceAsync("Admin.Catalog.Attributes"),
                            IconClass = "far fa-dot-circle",
                            ChildNodes = new List<AdminMenuItem>
                            {
                                new()
                                {
                                    SystemName = "Product attributes",
                                    Title = await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes"),
                                    Url = GetMenuItemUrl("ProductAttribute", "List"),
                                    PermissionNames = new List<string> { StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW },
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Specification attributes",
                                    Title = await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes"),
                                    Url = GetMenuItemUrl("SpecificationAttribute", "List"),
                                    PermissionNames = new List<string> { StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW },
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Checkout attributes",
                                    Title = await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes"),
                                    PermissionNames = new List<string> { StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_VIEW },
                                    Url = GetMenuItemUrl("CheckoutAttribute", "List"),
                                    IconClass = "far fa-circle"
                                }
                            }
                        }
                    }
                },
                //sales
                new()
                {
                    SystemName = "Sales",
                    Title = await _localizationService.GetResourceAsync("Admin.Sales"),
                    IconClass = "fas fa-shopping-cart",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Orders",
                            Title = await _localizationService.GetResourceAsync("Admin.Orders"),
                            PermissionNames = new List<string> { StandardPermission.Orders.ORDERS_VIEW },
                            Url = GetMenuItemUrl("Order", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Shipments",
                            Title = await _localizationService.GetResourceAsync("Admin.Orders.Shipments.List"),
                            PermissionNames = new List<string> { StandardPermission.Orders.SHIPMENTS_VIEW },
                            Url = GetMenuItemUrl("Order", "ShipmentList"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Return requests",
                            Title = await _localizationService.GetResourceAsync("Admin.ReturnRequests"),
                            PermissionNames = new List<string> { StandardPermission.Orders.RETURN_REQUESTS_VIEW },
                            Url = GetMenuItemUrl("ReturnRequest", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Recurring payments",
                            Title = await _localizationService.GetResourceAsync("Admin.RecurringPayments"),
                            PermissionNames = new List<string> { StandardPermission.Orders.RECURRING_PAYMENTS_VIEW },
                            Url = GetMenuItemUrl("RecurringPayment", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Gift cards",
                            Title = await _localizationService.GetResourceAsync("Admin.GiftCards"),
                            PermissionNames = new List<string> { StandardPermission.Orders.GIFT_CARDS_VIEW },
                            Url = GetMenuItemUrl("GiftCard", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Current shopping carts and wishlists",
                            Title = await _localizationService.GetResourceAsync("Admin.CurrentCarts.CartsAndWishlists"),
                            PermissionNames = new List<string> { StandardPermission.Orders.CURRENT_CARTS_MANAGE },
                            Url = GetMenuItemUrl("ShoppingCart", "CurrentCarts"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //customers
                new()
                {
                    SystemName = "Customers",
                    Title = await _localizationService.GetResourceAsync("Admin.Customers"),
                    IconClass = "far fa-user",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Customers list",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.Customers"),
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMERS_VIEW },
                            Url = GetMenuItemUrl("Customer", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Customer roles",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles"),
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMER_ROLES_VIEW },
                            Url = GetMenuItemUrl("CustomerRole", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Online customers",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.OnlineCustomers"),
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMERS_VIEW },
                            Url = GetMenuItemUrl("OnlineCustomer", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Vendors",
                            Title = await _localizationService.GetResourceAsync("Admin.Vendors"),
                            PermissionNames = new List<string> { StandardPermission.Customers.VENDORS_VIEW },
                            Url = GetMenuItemUrl("Vendor", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Activity logs",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.ActivityLog"),
                            PermissionNames = new List<string> { StandardPermission.Customers.ACTIVITY_LOG_VIEW },
                            Url = GetMenuItemUrl("ActivityLog", "ActivityLogs"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Activity types",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.ActivityLogType"),
                            PermissionNames = new List<string> { StandardPermission.Customers.ACTIVITY_LOG_VIEW },
                            Url = GetMenuItemUrl("ActivityLog", "ActivityTypes"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "GDPR log",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.GdprLog"),
                            PermissionNames = new List<string> { StandardPermission.Customers.GDPR_MANAGE },
                            Url = GetMenuItemUrl("Customer", "GdprLog"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //promotions
                new()
                {
                    SystemName = "Promotions",
                    Title = await _localizationService.GetResourceAsync("Admin.Promotions"),
                    IconClass = "fas fa-tags",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Discounts",
                            Title = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts"),
                            PermissionNames = new List<string> { StandardPermission.Promotions.DISCOUNTS_VIEW },
                            Url = GetMenuItemUrl("Discount", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Affiliates",
                            Title = await _localizationService.GetResourceAsync("Admin.Affiliates"),
                            PermissionNames = new List<string> { StandardPermission.Promotions.AFFILIATES_VIEW },
                            Url = GetMenuItemUrl("Affiliate", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Newsletter subscriptions",
                            Title = await _localizationService.GetResourceAsync("Admin.Promotions.NewsletterSubscriptions"),
                            PermissionNames = new List<string> { StandardPermission.Promotions.SUBSCRIBERS_VIEW },
                            Url = GetMenuItemUrl("NewsLetterSubscription", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Campaigns",
                            Title = await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns"),
                            PermissionNames = new List<string> { StandardPermission.Promotions.CAMPAIGNS_VIEW },
                            Url = GetMenuItemUrl("Campaign", "List"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //content management
                new()
                {
                    SystemName = "Content Management",
                    Title = await _localizationService.GetResourceAsync("Admin.ContentManagement"),
                    IconClass = "fas fa-cubes",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Topics",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.Topics"),
                            PermissionNames = new List<string> { StandardPermission.ContentManagement.TOPICS_VIEW },
                            Url = GetMenuItemUrl("Topic", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Message templates",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates"),
                            PermissionNames =
                                new List<string>
                                {
                                    StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW
                                },
                            Url = GetMenuItemUrl("MessageTemplate", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "News items",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems"),
                            PermissionNames =
                                new List<string> { StandardPermission.ContentManagement.NEWS_VIEW },
                            Url = GetMenuItemUrl("News", "NewsItems"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "News comments",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.News.Comments"),
                            PermissionNames =
                                new List<string>
                                {
                                    StandardPermission.ContentManagement.NEWS_COMMENTS_VIEW
                                },
                            Url = GetMenuItemUrl("News", "NewsComments"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Blog posts",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts"),
                            PermissionNames = new List<string> { StandardPermission.ContentManagement.BLOG_VIEW },
                            Url = GetMenuItemUrl("Blog", "BlogPosts"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Blog comments",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.Comments"),
                            PermissionNames =
                                new List<string>
                                {
                                    StandardPermission.ContentManagement.BLOG_COMMENTS_VIEW
                                },
                            Url = GetMenuItemUrl("Blog", "BlogComments"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Polls",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.Polls"),
                            PermissionNames = new List<string> { StandardPermission.ContentManagement.POLLS_VIEW },
                            Url = GetMenuItemUrl("Poll", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Manage forums",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums"),
                            PermissionNames = new List<string> { StandardPermission.ContentManagement.FORUMS_VIEW },
                            Url = GetMenuItemUrl("Forum", "List"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //configuration
                new()
                {
                    SystemName = "Configuration",
                    Title = await _localizationService.GetResourceAsync("Admin.Configuration"),
                    IconClass = "fas fa-cogs",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Settings",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_SETTINGS },
                            IconClass = "far fa-dot-circle",
                            ChildNodes = new List<AdminMenuItem>
                            {
                                new()
                                {
                                    SystemName = "General settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon"),
                                    Url = GetMenuItemUrl("Setting", "GeneralCommon"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Customer and user settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.CustomerUser"),
                                    Url = GetMenuItemUrl("Setting", "CustomerUser"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Order settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Order"),
                                    Url = GetMenuItemUrl("Setting", "Order"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Shipping settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Shipping"),
                                    Url = GetMenuItemUrl("Setting", "Shipping"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Tax settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Tax"),
                                    Url = GetMenuItemUrl("Setting", "Tax"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Catalog settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Catalog"),
                                    Url = GetMenuItemUrl("Setting", "Catalog"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Shopping cart settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.ShoppingCart"),
                                    Url = GetMenuItemUrl("Setting", "ShoppingCart"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Reward points",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.RewardPoints"),
                                    Url = GetMenuItemUrl("Setting", "RewardPoints"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "GDPR settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr"),
                                    Url = GetMenuItemUrl("Setting", "Gdpr"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Vendor settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Vendor"),
                                    Url = GetMenuItemUrl("Setting", "Vendor"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Blog settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Blog"),
                                    Url = GetMenuItemUrl("Setting", "Blog"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "News settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.News"),
                                    Url = GetMenuItemUrl("Setting", "News"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Forums settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Forums"),
                                    Url = GetMenuItemUrl("Setting", "Forum"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Media settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Media"),
                                    Url = GetMenuItemUrl("Setting", "Media"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "App settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.AppSettings"),
                                    PermissionNames =
                                        new List<string>
                                        {
                                            StandardPermission.System.MANAGE_APP_SETTINGS
                                        },
                                    Url = GetMenuItemUrl("Setting", "AppSettings"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "All settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings"),
                                    Url = GetMenuItemUrl("Setting", "AllSettings"),
                                    IconClass = "far fa-circle"
                                }
                            }
                        },
                        new()
                        {
                            SystemName = "Email accounts",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS },
                            Url = GetMenuItemUrl("EmailAccount",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Stores",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Stores"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_STORES },
                            Url = GetMenuItemUrl("Store",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Countries",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Countries"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_COUNTRIES },
                            Url = GetMenuItemUrl("Country",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Languages",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Languages"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_LANGUAGES },
                            Url = GetMenuItemUrl("Language",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Currencies",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Currencies"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_CURRENCIES },
                            Url = GetMenuItemUrl("Currency",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Payment methods",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Payment.Methods"),
                            PermissionNames =
                                new List<string>
                                {
                                    StandardPermission.Configuration.MANAGE_PAYMENT_METHODS
                                },
                            Url = GetMenuItemUrl("Payment", "Methods"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Payment restrictions",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Payment.MethodRestrictions"),
                            PermissionNames =
                                new List<string>
                                {
                                    StandardPermission.Configuration.MANAGE_PAYMENT_METHODS
                                },
                            Url = GetMenuItemUrl("Payment", "MethodRestrictions"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Tax providers",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Tax.Providers"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_TAX_SETTINGS },
                            Url = GetMenuItemUrl("Tax", "Providers"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Tax categories",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Tax.Categories"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_TAX_SETTINGS },
                            Url = GetMenuItemUrl("Tax", "Categories"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Shipping",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Shipping"),
                            PermissionNames =
                                new List<string>
                                {
                                    StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS
                                },
                            IconClass = "far fa-dot-circle",
                            ChildNodes = new List<AdminMenuItem>
                            {
                                new()
                                {
                                    SystemName = "Shipping providers",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Providers"),
                                    Url = GetMenuItemUrl("Shipping", "Providers"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Warehouses",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses"),
                                    Url = GetMenuItemUrl("Shipping", "Warehouses"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Pickup points",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.PickupPoints"),
                                    Url = GetMenuItemUrl("Shipping", "PickupPointProviders"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Dates and ranges",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.DatesAndRanges"),
                                    Url = GetMenuItemUrl("Shipping", "DatesAndRanges"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Measures",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures"),
                                    Url = GetMenuItemUrl("Measure", "List"),
                                    IconClass = "far fa-circle"
                                }
                            }
                        },
                        new()
                        {
                            SystemName = "Access control list",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.ACL"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_ACL },
                            Url = GetMenuItemUrl("Security", "Permissions"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Widgets",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.Widgets"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_WIDGETS },
                            Url = GetMenuItemUrl("Widget", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Authentication",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Authentication"),
                            IconClass = "far fa-dot-circle",
                            ChildNodes = new List<AdminMenuItem>
                            {
                                new()
                                {
                                    SystemName = "External authentication methods",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Authentication.ExternalMethods"),
                                    PermissionNames =
                                        new List<string>
                                        {
                                            StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS
                                        },
                                    Url = GetMenuItemUrl("Authentication", "ExternalMethods"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Multi-factor authentication methods",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Authentication.MultiFactorMethods"),
                                    PermissionNames =
                                        new List<string>
                                        {
                                            StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS
                                        },
                                    Url = GetMenuItemUrl("Authentication", "MultiFactorMethods"),
                                    IconClass = "far fa-circle"
                                }
                            }
                        },
                        new()
                        {
                            SystemName = "Local plugins",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.Local"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_PLUGINS },
                            Url = GetMenuItemUrl("Plugin", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "All plugins and themes",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.OfficialFeed"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_PLUGINS },
                            Url = GetMenuItemUrl("Plugin", "OfficialFeed"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //system
                new()
                {
                    SystemName = "System",
                    Title = await _localizationService.GetResourceAsync("Admin.System"),
                    IconClass = "fas fa-cube",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "System information",
                            Title = await _localizationService.GetResourceAsync("Admin.System.SystemInfo"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "SystemInfo"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Log",
                            Title = await _localizationService.GetResourceAsync("Admin.System.Log"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_SYSTEM_LOG },
                            Url = GetMenuItemUrl("Log", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Warnings",
                            Title = await _localizationService.GetResourceAsync("Admin.System.Warnings"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "Warnings"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Maintenance",
                            Title = await _localizationService.GetResourceAsync("Admin.System.Maintenance"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "Maintenance"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Queued emails",
                            Title =
                                await _localizationService.GetResourceAsync("Admin.System.QueuedEmails"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MESSAGE_QUEUE },
                            Url = GetMenuItemUrl("QueuedEmail", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Schedule tasks",
                            Title = await _localizationService.GetResourceAsync("Admin.System.ScheduleTasks"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_SCHEDULE_TASKS },
                            Url = GetMenuItemUrl("ScheduleTask",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Search engine friendly names",
                            Title = await _localizationService.GetResourceAsync("Admin.System.SeNames"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "SeNames"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Templates",
                            Title = await _localizationService.GetResourceAsync("Admin.System.Templates"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Template", "List"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //reports
                new()
                {
                    SystemName = "Reports",
                    Title = await _localizationService.GetResourceAsync("Admin.Reports"),
                    IconClass = "fas fa-chart-line",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Sales summary",
                            Title = await _localizationService.GetResourceAsync("Admin.Reports.SalesSummary"),
                            PermissionNames = new List<string> { StandardPermission.Reports.SALES_SUMMARY },
                            Url = GetMenuItemUrl("Report", "SalesSummary"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Low stock",
                            Title = await _localizationService.GetResourceAsync("Admin.Reports.LowStock"),
                            PermissionNames =
                                new List<string>
                                {
                                    StandardPermission.Catalog.PRODUCTS_VIEW,
                                    StandardPermission.Reports.LOW_STOCK
                                },
                            Url = GetMenuItemUrl("Report", "LowStock"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Bestsellers",
                            Title = await _localizationService.GetResourceAsync("Admin.Reports.Sales.Bestsellers"),
                            PermissionNames =
                                new List<string>
                                {
                                    StandardPermission.Orders.ORDERS_VIEW,
                                    StandardPermission.Reports.BESTSELLERS
                                },
                            Url = GetMenuItemUrl("Report", "Bestsellers"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Products never purchased",
                            Title = await _localizationService.GetResourceAsync("Admin.Reports.Sales.NeverSold"),
                            PermissionNames =
                                new List<string>
                                {
                                    StandardPermission.Orders.ORDERS_VIEW,
                                    StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED
                                },
                            Url = GetMenuItemUrl("Report", "NeverSold"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Country sales",
                            Title = await _localizationService.GetResourceAsync("Admin.Reports.Sales.Country"),
                            PermissionNames = new List<string> { StandardPermission.Reports.COUNTRY_SALES },
                            Url = GetMenuItemUrl("Report", "CountrySales"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Customers",
                            Title = await _localizationService.GetResourceAsync("Admin.Reports.Customers"),
                            IconClass = "far fa-dot-circle",
                            ChildNodes = new List<AdminMenuItem>
                            {
                                new()
                                {
                                    SystemName = "Registered customers",
                                    Title = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers"),
                                    PermissionNames =
                                        new List<string>
                                        {
                                            StandardPermission.Customers.CUSTOMERS_VIEW,
                                            StandardPermission.Reports.REGISTERED_CUSTOMERS
                                        },
                                    Url = GetMenuItemUrl("Report", "RegisteredCustomers"),
                                    IconClass = "far fa-dot-circle"
                                },
                                new()
                                {
                                    SystemName = "Customers by order total",
                                    Title = await _localizationService.GetResourceAsync("Admin.Reports.Customers.BestBy.BestByOrderTotal"),
                                    PermissionNames =
                                        new List<string>
                                        {
                                            StandardPermission.Customers.CUSTOMERS_VIEW,
                                            StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL
                                        },
                                    Url = GetMenuItemUrl("Report", "BestCustomersByOrderTotal"),
                                    IconClass = "far fa-dot-circle"
                                },
                                new()
                                {
                                    SystemName = "Customers by number of orders",
                                    Title = await _localizationService.GetResourceAsync("Admin.Reports.Customers.BestBy.BestByNumberOfOrders"),
                                    PermissionNames =
                                        new List<string>
                                        {
                                            StandardPermission.Customers.CUSTOMERS_VIEW,
                                            StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS
                                        },
                                    Url = GetMenuItemUrl("Report", "BestCustomersByNumberOfOrders"),
                                    IconClass = "far fa-dot-circle"
                                }
                            }
                        }
                    }
                },
                //help
                new()
                {
                    SystemName = "Help",
                    Title = await _localizationService.GetResourceAsync("Admin.Help"),
                    IconClass = "fas fa-question-circle",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Training",
                            Title = await _localizationService.GetResourceAsync("Admin.Help.Training"),
                            Url = "https://www.nopcommerce.com/training?utm_source=admin-panel&utm_medium=menu&utm_campaign=course&utm_content=help",
                            IconClass = "far fa-dot-circle",
                            OpenUrlInNewTab = true
                        },
                        new()
                        {
                            SystemName = "Documentation",
                            Title = await _localizationService.GetResourceAsync("Admin.Help.Documentation"),
                            Url = "https://docs.nopcommerce.com?utm_source=admin-panel&utm_medium=menu&utm_campaign=documentation&utm_content=help",
                            IconClass = "far fa-dot-circle",
                            OpenUrlInNewTab = true
                        },
                        new()
                        {
                            SystemName = "Community forums",
                            Title = await _localizationService.GetResourceAsync("Admin.Help.Forums"),
                            Url = "https://www.nopcommerce.com/boards?utm_source=admin-panel&utm_medium=menu&utm_campaign=forum&utm_content=help",
                            IconClass = "far fa-dot-circle",
                            OpenUrlInNewTab = true
                        },
                        new()
                        {
                            SystemName = "Premium support services",
                            Title = await _localizationService.GetResourceAsync("Admin.Help.SupportServices"),
                            Url = "https://www.nopcommerce.com/nopcommerce-premium-support-services?utm_source=admin-panel&utm_medium=menu&utm_campaign=premium_support&utm_content=help",
                            IconClass = "far fa-dot-circle",
                            OpenUrlInNewTab = true
                        },
                        new()
                        {
                            SystemName = "Solution partners",
                            Title = await _localizationService.GetResourceAsync("Admin.Help.SolutionPartners"),
                            Url = "https://www.nopcommerce.com/solution-partners?utm_source=admin-panel&utm_medium=menu&utm_campaign=solution_partners&utm_content=help",
                            IconClass = "far fa-dot-circle",
                            OpenUrlInNewTab = true
                        }
                    }
                },
                //third party plugins
                new()
                {
                    SystemName = "Third party plugins",
                    Title = await _localizationService.GetResourceAsync("Admin.Plugins"),
                    IconClass = "fas fa-bars"
                }
            }
        };
    }

    /// <summary>
    /// Loads admin menu
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records (Visible == false)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the root menu item for admin menu
    /// </returns>
    protected virtual async Task<AdminMenuItem> LoadMenuAsync(bool showHidden)
    {
        await FillBaseRootAsync();

        AdminMenuItem cloneMenuItem(AdminMenuItem item)
        {
            return new AdminMenuItem
            {
                PermissionNames = item.PermissionNames,
                ChildNodes = item.ChildNodes.Select(cloneMenuItem).ToList(),
                IconClass = item.IconClass,
                Visible = item.Visible,
                OpenUrlInNewTab = item.OpenUrlInNewTab,
                SystemName = item.SystemName,
                Title = item.Title,
                Url = item.Url
            };
        }

        var root = cloneMenuItem(_baseRootMenuItem);

        var customer = await _workContext.GetCurrentCustomerAsync();

        await _eventPublisher.PublishAsync(new AdminMenuCreatedEvent(this, root));

        if (await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PLUGINS, customer))
        {
            await _eventPublisher.PublishAsync(new ThirdPartyPluginsMenuItemCreatedEvent(this, root.GetItemBySystem("Third party plugins")));
            
            var adminMenuPlugins = await _adminMenuPluginManager.LoadAllPluginsAsync(customer);

            foreach (var adminMenuPlugin in adminMenuPlugins)
                await adminMenuPlugin.ManageSiteMapAsync(root);
        }

        async ValueTask<bool> authorizePermission(string permissionName) => await _permissionService.AuthorizeAsync(permissionName.Trim());

        async Task checkPermissions(AdminMenuItem menuItem, AdminMenuItem rootItem = null)
        {
            var permissions = (menuItem.PermissionNames.Any() ? menuItem.PermissionNames : (rootItem?.PermissionNames ?? new List<string>())).Distinct().Where(p => !string.IsNullOrEmpty(p)).ToList();

            if (permissions.Any())
                menuItem.Visible = menuItem.ChildNodes.Any() ? await permissions.AnyAwaitAsync(authorizePermission) : await permissions.AllAwaitAsync(authorizePermission);

            foreach (var childNode in menuItem.ChildNodes)
                await checkPermissions(childNode, menuItem);
        }

        await checkPermissions(root);

        if (showHidden)
            return root;

        void checkVisible(AdminMenuItem menuItem)
        {
            if (!menuItem.ChildNodes.Any())
            {
                menuItem.Visible = menuItem.Visible && !string.IsNullOrEmpty(menuItem.Url);

                return;
            }

            foreach (var childNode in menuItem.ChildNodes)
                checkVisible(childNode);

            menuItem.Visible = menuItem.ChildNodes.Any(n => n.Visible);
        }

        checkVisible(root);

        return root;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the root node
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records (Visible == false)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the root menu item
    /// </returns>
    public async Task<AdminMenuItem> GetRootNodeAsync(bool showHidden = false)
    {
        if (_rootItem != null)
            return _rootItem;

        _rootItem = await LoadMenuAsync(showHidden);

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext ?? throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext)));

        void transformUrl(AdminMenuItem node)
        {
            if (node.Url?.StartsWith("~/", StringComparison.Ordinal) ?? false)
                node.Url = urlHelper.Content(node.Url);

            foreach (var childNode in node.ChildNodes)
                transformUrl(childNode);
        }

        transformUrl(_rootItem);

        return _rootItem;
    }

    /// <summary>
    /// Generates an admin menu item URL 
    /// </summary>
    /// <param name="controllerName">The name of the controller</param>
    /// <param name="actionName">The name of the action method</param>
    /// <returns>Menu item URL</returns>
    public string GetMenuItemUrl(string controllerName, string actionName)
    {
        if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
            return null;

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext ?? throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext)));

        return urlHelper.Action(actionName, controllerName, new RouteValueDictionary { { "area", AreaNames.ADMIN } }, null, null);
    }

    #endregion
}