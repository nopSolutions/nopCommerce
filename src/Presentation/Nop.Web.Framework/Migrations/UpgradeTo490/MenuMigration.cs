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
using Nop.Web.Framework.Extensions;
using M = Nop.Core.Domain.Menus;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopMigration("2025-06-18 12:00:00", "Menus. Adding menus", MigrationProcessType.Update)]
public class MenuMigration : Migration
{
    #region Fields

    private readonly INopDataProvider _dataProvider;
    private readonly IRepository<M.Menu> _menuRepository;
    private readonly IRepository<Topic> _topicRepository;

    #endregion

    #region Ctor

    public MenuMigration(INopDataProvider dataProvider,
        IRepository<M.Menu> menuRepository,
        IRepository<Topic> topicRepository)
    {
        _dataProvider = dataProvider;
        _menuRepository = menuRepository;
        _topicRepository = topicRepository;
    }

    #endregion

    #region Utilities

    private bool IsSettingEnabled(string key, out Setting setting)
    {
        setting = this.GetSetting(key);
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
            _dataProvider.InsertEntity(new M.Menu
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
        var catalogSettings = this.LoadSetting<CatalogSettings>();

        #region Information

        var footerInformation = new M.Menu
        {
            Name = "Information",
            MenuType = MenuType.Footer,
            DisplayOrder = 0,
            Published = true
        };
        _dataProvider.InsertEntity(footerInformation);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerInformation.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.SITEMAP,
            Title = "Sitemap",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaysitemapfooteritem", out var displaysitemapfooteritem) && this.LoadSetting<SitemapSettings>().SitemapEnabled
        });

        if (displaysitemapfooteritem is not null)
            this.DeleteSetting(displaysitemapfooteritem);

        _dataProvider.BulkDeleteEntities(
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

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerInformation.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CONTACT_US,
            Title = "Contact us",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycontactusfooteritem", out var displaycontactusfooteritem)
        });

        if (displaycontactusfooteritem is not null)
            this.DeleteSetting(displaycontactusfooteritem);

        #endregion

        #region Customer services

        var footerCustomerService = new M.Menu
        {
            Name = "Customer service",
            MenuType = MenuType.Footer,
            DisplayOrder = 1,
            Published = true
        };
        _dataProvider.InsertEntity(footerCustomerService);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.SEARCH,
            Title = "Search",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayproductsearchfooteritem", out var displayproductsearchfooteritem) && catalogSettings.ProductSearchEnabled
        });

        if (displayproductsearchfooteritem is not null)
            this.DeleteSetting(displayproductsearchfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.NEWS,
            Title = "News",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaynewsfooteritem", out var displaynewsfooteritem) && this.LoadSetting<NewsSettings>().Enabled
        });

        if (displaynewsfooteritem is not null)
            this.DeleteSetting(displaynewsfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.BLOG,
            Title = "Blog",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayblogfooteritem", out var displayblogfooteritem) && this.LoadSetting<BlogSettings>().Enabled
        });

        if (displayblogfooteritem is not null)
            this.DeleteSetting(displayblogfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.BOARDS,
            Title = "Forum",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayforumsfooteritem", out var displayforumsfooteritem) && this.LoadSetting<ForumSettings>().ForumsEnabled
        });

        if (displayforumsfooteritem is not null)
            this.DeleteSetting(displayforumsfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.RECENTLY_VIEWED_PRODUCTS,
            Title = "Recently viewed products",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayrecentlyviewedproductsfooteritem", out var displayrecentlyviewedproductsfooteritem) && catalogSettings.RecentlyViewedProductsEnabled
        });

        if (displayrecentlyviewedproductsfooteritem is not null)
            this.DeleteSetting(displayrecentlyviewedproductsfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.COMPARE_PRODUCTS,
            Title = "Compare products list",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycompareproductsfooteritem", out var displaycompareproductsfooteritem) && catalogSettings.CompareProductsEnabled
        });

        if (displaycompareproductsfooteritem is not null)
            this.DeleteSetting(displaycompareproductsfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.NEW_PRODUCTS,
            Title = "New products",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaynewproductsfooteritem", out var displaynewproductsfooteritem) && catalogSettings.NewProductsEnabled
        });

        if (displaynewproductsfooteritem is not null)
            this.DeleteSetting(displaynewproductsfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerCustomerService.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CHECK_GIFT_CARD_BALANCE,
            Title = "Check gift card balance",
            Published = this.LoadSetting<CustomerSettings>().AllowCustomersToCheckGiftCardBalance
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

        _dataProvider.InsertEntity(footerMyAccount);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CUSTOMER_INFO,
            Title = "My account",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycustomerinfofooteritem", out var displaycustomerinfofooteritem)
        });

        if (displaycustomerinfofooteritem is not null)
            this.DeleteSetting(displaycustomerinfofooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CUSTOMER_ORDERS,
            Title = "Orders",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycustomerordersfooteritem", out var displaycustomerordersfooteritem)
        });

        if (displaycustomerordersfooteritem is not null)
            this.DeleteSetting(displaycustomerordersfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CUSTOMER_ADDRESSES,
            Title = "Addresses",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaycustomeraddressesfooteritem", out var displaycustomeraddressesfooteritem)
        });

        if (displaycustomeraddressesfooteritem is not null)
            this.DeleteSetting(displaycustomeraddressesfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.CART,
            Title = "Shopping cart",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayshoppingcartfooteritem", out var displayshoppingcartfooteritem)
        });

        if (displayshoppingcartfooteritem is not null)
            this.DeleteSetting(displayshoppingcartfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.WISHLIST,
            Title = "Wishlist",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displaywishlistfooteritem", out var displaywishlistfooteritem)
        });

        if (displaywishlistfooteritem is not null)
            this.DeleteSetting(displaywishlistfooteritem);

        _dataProvider.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.APPLY_VENDOR_ACCOUNT,
            Title = "Apply for vendor account",
            Published = IsSettingEnabled("displaydefaultfooteritemsettings.displayapplyvendoraccountfooteritem", out var displayapplyvendoraccountfooteritem) && this.LoadSetting<VendorSettings>().AllowCustomersToApplyForVendorAccount
        });

        if (displayapplyvendoraccountfooteritem is not null)
            this.DeleteSetting(displayapplyvendoraccountfooteritem);

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