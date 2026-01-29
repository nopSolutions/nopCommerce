using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Menus;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Helpers;
using Nop.Web.Framework.Extensions;
using M = Nop.Core.Domain.Menus;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopMigration("2025-06-18 12:00:00", "Menus. Adding menus", MigrationProcessType.Update)]
public class MenuMigration : Migration
{
    #region Fields

    private readonly IRepository<M.Menu> _menuRepository;
    private readonly IRepository<Topic> _topicRepository;

    #endregion

    #region Ctor

    public MenuMigration(IRepository<M.Menu> menuRepository,
        IRepository<Topic> topicRepository)
    {
        _menuRepository = menuRepository;
        _topicRepository = topicRepository;
    }

    #endregion

    #region Utilities

    private bool IsSettingEnabled(string key)
    {
        return this.GetSettingByKey(key, defaultValue: false);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        var syncCodeHelper = EngineContext.Current.Resolve<ISyncCodeHelper>();

        if (!_menuRepository.Table.Any(m => m.MenuTypeId == (int)MenuType.Main))
        {
            syncCodeHelper.InsertEntity(new M.Menu
            {
                Name = "Categories",
                MenuType = MenuType.Main,
                DisplayAllCategories = true,
                Published = true
            });
        }

        if (_menuRepository.Table.Any(m => m.MenuTypeId == (int)MenuType.Footer))
            return;

        var topics = _topicRepository.Table.ToList();

        #region Information

        var footerInformation = new M.Menu
        {
            Name = "Information",
            MenuType = MenuType.Footer,
            DisplayOrder = 0,
            Published = true
        };
        syncCodeHelper.InsertEntity(footerInformation);

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerInformation.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.SITEMAP,
            Title = "Sitemap",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaysitemapfooteritem") && IsSettingEnabled($"{nameof(SitemapSettings)}.{nameof(SitemapSettings.SitemapEnabled)}")
        });

        syncCodeHelper.InsertEntities(
        [
            new MenuItem
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = topics.FirstOrDefault(t => t.SystemName == "ShippingInfo")?.Id,
                Published = true
            },
            new MenuItem
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = topics.FirstOrDefault(t => t.SystemName == "PrivacyInfo")?.Id,
                Published = true
            },
            new MenuItem
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = topics.FirstOrDefault(t => t.SystemName == "ConditionsOfUse")?.Id,
                Published = true
            },
            new MenuItem
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = topics.FirstOrDefault(t => t.SystemName == "AboutUs")?.Id,
                Published = true
            }
        ]);

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerInformation.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CONTACT_US,
            Title = "Contact us",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycontactusfooteritem")
        });

        #endregion

        #region Customer services

        var footerCustomerService = new M.Menu
        {
            Name = "Customer service",
            MenuType = MenuType.Footer,
            DisplayOrder = 1,
            Published = true
        };
        syncCodeHelper.InsertEntity(footerCustomerService);

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.SEARCH,
            Title = "Search",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayproductsearchfooteritem") && IsSettingEnabled($"{nameof(CatalogSettings)}.{nameof(CatalogSettings.ProductSearchEnabled)}")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.BLOG,
            Title = "Blog",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayblogfooteritem") && IsSettingEnabled($"{nameof(BlogSettings)}.{nameof(BlogSettings.Enabled)}")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.BOARDS,
            Title = "Forum",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayforumsfooteritem") && IsSettingEnabled($"{nameof(ForumSettings)}.{nameof(ForumSettings.ForumsEnabled)}")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.RECENTLY_VIEWED_PRODUCTS,
            Title = "Recently viewed products",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayrecentlyviewedproductsfooteritem") && IsSettingEnabled($"{nameof(CatalogSettings)}.{nameof(CatalogSettings.RecentlyViewedProductsEnabled)}")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.COMPARE_PRODUCTS,
            Title = "Compare products list",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycompareproductsfooteritem") && IsSettingEnabled($"{nameof(CatalogSettings)}.{nameof(CatalogSettings.CompareProductsEnabled)}")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.NEW_PRODUCTS,
            Title = "New products",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaynewproductsfooteritem") && IsSettingEnabled($"{nameof(CatalogSettings)}.{nameof(CatalogSettings.NewProductsEnabled)}")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CHECK_GIFT_CARD_BALANCE,
            Title = "Check gift card balance",
            Published = IsSettingEnabled($"{nameof(CustomerSettings)}.{nameof(CustomerSettings.AllowCustomersToCheckGiftCardBalance)}")
        });

        #endregion

        #region My account

        var footerMyAccount = new M.Menu
        {
            Name = "My account",
            MenuType = MenuType.Footer,
            DisplayOrder = 2,
            Published = true
        };

        syncCodeHelper.InsertEntity(footerMyAccount);

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CUSTOMER_INFO,
            Title = "My account",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycustomerinfofooteritem")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CUSTOMER_ORDERS,
            Title = "Orders",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycustomerordersfooteritem")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CUSTOMER_ADDRESSES,
            Title = "Addresses",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycustomeraddressesfooteritem")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CART,
            Title = "Shopping cart",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayshoppingcartfooteritem")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.WISHLIST,
            Title = "Wishlist",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaywishlistfooteritem")
        });

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.APPLY_VENDOR_ACCOUNT,
            Title = "Apply for vendor account",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayapplyvendoraccountfooteritem") && IsSettingEnabled($"{nameof(VendorSettings)}.{nameof(VendorSettings.AllowCustomersToApplyForVendorAccount)}")
        });

        #endregion

        this.DeleteSettingsByNames([
            "displaydefaultfooteritemsettings.displaysitemapfooteritem",
            "displaydefaultfooteritemsettings.displaycontactusfooteritem",
            "displaydefaultfooteritemsettings.displayproductsearchfooteritem",
            "displaydefaultfooteritemsettings.displaynewsfooteritem",
            "displaydefaultfooteritemsettings.displayblogfooteritem",
            "displaydefaultfooteritemsettings.displayforumsfooteritem",
            "displaydefaultfooteritemsettings.displayrecentlyviewedproductsfooteritem",
            "displaydefaultfooteritemsettings.displaycompareproductsfooteritem",
            "displaydefaultfooteritemsettings.displaynewproductsfooteritem",
            "displaydefaultfooteritemsettings.displaycustomerinfofooteritem",
            "displaydefaultfooteritemsettings.displaycustomerordersfooteritem",
            "displaydefaultfooteritemsettings.displaycustomeraddressesfooteritem",
            "displaydefaultfooteritemsettings.displayshoppingcartfooteritem",
            "displaydefaultfooteritemsettings.displaywishlistfooteritem",
            "displaydefaultfooteritemsettings.displayapplyvendoraccountfooteritem"
        ]);
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {

    }

    #endregion
}