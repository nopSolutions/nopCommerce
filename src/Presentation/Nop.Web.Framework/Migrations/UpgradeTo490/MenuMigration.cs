using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Menus;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using M = Nop.Core.Domain.Menus;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopMigration("2025-06-18 12:00:00", "Menus. Adding menus", MigrationProcessType.Update)]
public class MenuMigration : Migration
{
    #region Fields

    private readonly IRepository<M.Menu> _menuRepository;
    private readonly IRepository<M.MenuItem> _menuItemRepository;
    private readonly IRepository<Topic> _topicRepository;
    private readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public MenuMigration(IRepository<M.Menu> menuRepository,
        IRepository<M.MenuItem> menuItemRepository,
        IRepository<Topic> topicRepository,
        ISettingService settingService)
    {
        _menuRepository = menuRepository;
        _menuItemRepository = menuItemRepository;
        _topicRepository = topicRepository;
        _settingService = settingService;
    }

    #endregion

    #region Utilities

    private bool IsSettingEnabled(string key, out Setting setting)
    {
        setting = _settingService.GetSetting(key);
        return setting is not null && CommonHelper.To<bool>(setting.Value);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!_menuRepository.Table.Any(m => m.MenuTypeId == (int)MenuType.Main))
        {
            _menuRepository.Insert(new M.Menu
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
        var catalogSettings = _settingService.LoadSetting<CatalogSettings>();

        #region Information

        var footerInformation = new M.Menu
        {
            Name = "Information",
            MenuType = MenuType.Footer,
            DisplayOrder = 0,
            Published = true
        };
        _menuRepository.Insert(footerInformation);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerInformation.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.SITEMAP,
            Title = "Sitemap",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaysitemapfooteritem", out var displaysitemapfooteritem) && _settingService.LoadSetting<SitemapSettings>().SitemapEnabled
        });

        if (displaysitemapfooteritem is not null)
            _settingService.DeleteSetting(displaysitemapfooteritem);

        _menuItemRepository.Insert(
        [
            new()
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = topics.FirstOrDefault(t => t.SystemName == "ShippingInfo")?.Id,
                Published = true
            },
            new()
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = topics.FirstOrDefault(t => t.SystemName == "PrivacyInfo")?.Id,
                Published = true
            },
            new()
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = topics.FirstOrDefault(t => t.SystemName == "ConditionsOfUse")?.Id,
                Published = true
            },
            new()
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = topics.FirstOrDefault(t => t.SystemName == "AboutUs")?.Id,
                Published = true
            }
        ]);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerInformation.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CONTACT_US,
            Title = "Contact us",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycontactusfooteritem", out var displaycontactusfooteritem)
        });

        if (displaycontactusfooteritem is not null)
            _settingService.DeleteSetting(displaycontactusfooteritem);

        #endregion

        #region Customer services

        var footerCustomerService = new M.Menu
        {
            Name = "Customer service",
            MenuType = MenuType.Footer,
            DisplayOrder = 1,
            Published = true
        };
        _menuRepository.Insert(footerCustomerService);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.SEARCH,
            Title = "Search",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayproductsearchfooteritem", out var displayproductsearchfooteritem) && catalogSettings.ProductSearchEnabled
        });

        if (displayproductsearchfooteritem is not null)
            _settingService.DeleteSetting(displayproductsearchfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.NEWS,
            Title = "News",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaynewsfooteritem", out var displaynewsfooteritem) && _settingService.LoadSetting<NewsSettings>().Enabled
        });

        if (displaynewsfooteritem is not null)
            _settingService.DeleteSetting(displaynewsfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.BLOG,
            Title = "Blog",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayblogfooteritem", out var displayblogfooteritem) && _settingService.LoadSetting<BlogSettings>().Enabled
        });

        if (displayblogfooteritem is not null)
            _settingService.DeleteSetting(displayblogfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.BOARDS,
            Title = "Forum",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayforumsfooteritem", out var displayforumsfooteritem) && _settingService.LoadSetting<ForumSettings>().ForumsEnabled
        });

        if (displayforumsfooteritem is not null)
            _settingService.DeleteSetting(displayforumsfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.RECENTLY_VIEWED_PRODUCTS,
            Title = "Recently viewed products",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayrecentlyviewedproductsfooteritem", out var displayrecentlyviewedproductsfooteritem) && catalogSettings.RecentlyViewedProductsEnabled
        });

        if (displayrecentlyviewedproductsfooteritem is not null)
            _settingService.DeleteSetting(displayrecentlyviewedproductsfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.COMPARE_PRODUCTS,
            Title = "Compare products list",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycompareproductsfooteritem", out var displaycompareproductsfooteritem) && catalogSettings.CompareProductsEnabled
        });

        if (displaycompareproductsfooteritem is not null)
            _settingService.DeleteSetting(displaycompareproductsfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.NEW_PRODUCTS,
            Title = "New products",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaynewproductsfooteritem", out var displaynewproductsfooteritem) && catalogSettings.NewProductsEnabled
        });

        if (displaynewproductsfooteritem is not null)
            _settingService.DeleteSetting(displaynewproductsfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CHECK_GIFT_CARD_BALANCE,
            Title = "Check gift card balance",
            Published = _settingService.LoadSetting<CustomerSettings>().AllowCustomersToCheckGiftCardBalance
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

        _menuRepository.Insert(footerMyAccount);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CUSTOMER_INFO,
            Title = "My account",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycustomerinfofooteritem", out var displaycustomerinfofooteritem)
        });

        if (displaycustomerinfofooteritem is not null)
            _settingService.DeleteSetting(displaycustomerinfofooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CUSTOMER_ORDERS,
            Title = "Orders",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycustomerordersfooteritem", out var displaycustomerordersfooteritem)
        });

        if (displaycustomerordersfooteritem is not null)
            _settingService.DeleteSetting(displaycustomerordersfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CUSTOMER_ADDRESSES,
            Title = "Addresses",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycustomeraddressesfooteritem", out var displaycustomeraddressesfooteritem)
        });

        if (displaycustomeraddressesfooteritem is not null)
            _settingService.DeleteSetting(displaycustomeraddressesfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CART,
            Title = "Shopping cart",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayshoppingcartfooteritem", out var displayshoppingcartfooteritem)
        });

        if (displayshoppingcartfooteritem is not null)
            _settingService.DeleteSetting(displayshoppingcartfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.WISHLIST,
            Title = "Wishlist",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaywishlistfooteritem", out var displaywishlistfooteritem)
        });

        if (displaywishlistfooteritem is not null)
            _settingService.DeleteSetting(displaywishlistfooteritem);

        _menuItemRepository.Insert(new M.MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.APPLY_VENDOR_ACCOUNT,
            Title = "Apply for vendor account",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayapplyvendoraccountfooteritem", out var displayapplyvendoraccountfooteritem) &&
                _settingService.LoadSetting<VendorSettings>().AllowCustomersToApplyForVendorAccount
        });

        if (displayapplyvendoraccountfooteritem is not null)
            _settingService.DeleteSetting(displayapplyvendoraccountfooteritem);

        #endregion
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {

    }

    #endregion
}