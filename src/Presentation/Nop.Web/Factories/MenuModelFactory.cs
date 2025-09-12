using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Menus;
using Nop.Core.Http;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Menus;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.Menus;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the menu model factory
/// </summary>
public partial class MenuModelFactory : IMenuModelFactory
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ICategoryService _categoryService;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IMenuService _menuService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IPictureService _pictureService;
    protected readonly IProductService _productService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITopicService _topicService;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly MenuSettings _menuSettings;

    #endregion

    #region Ctor

    public MenuModelFactory(IAclService aclService,
        ICategoryService categoryService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        ILogger logger,
        IManufacturerService manufacturerService,
        IMenuService menuService,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IProductService productService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        ITopicService topicService,
        IVendorService vendorService,
        IWorkContext workContext,
        MenuSettings menuSettings)
    {
        _aclService = aclService;
        _categoryService = categoryService;
        _customerService = customerService;
        _localizationService = localizationService;
        _logger = logger;
        _manufacturerService = manufacturerService;
        _menuService = menuService;
        _nopUrlHelper = nopUrlHelper;
        _pictureService = pictureService;
        _productService = productService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _topicService = topicService;
        _vendorService = vendorService;
        _workContext = workContext;
        _menuSettings = menuSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get category info with authorization check
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains category's title and URL
    ///</returns>
    protected virtual async Task<(string title, string url)> GetAuthorizedCategoryInfoAsync(MenuItem menuItem)
    {
        var categoryId = menuItem.EntityId ?? 0;
        if (categoryId == 0)
            return (await _localizationService.GetLocalizedAsync(menuItem, m => m.Title), string.Empty);

        var category = await _categoryService.GetCategoryByIdAsync(categoryId);

        if (category is null || category.Deleted || !category.Published ||
            !await _aclService.AuthorizeAsync(category) ||
            !await _storeMappingService.AuthorizeAsync(category))
        {
            return (string.Empty, string.Empty);
        }

        var title = await _localizationService.GetLocalizedAsync(category, m => m.Name);
        var url = await _nopUrlHelper.RouteGenericUrlAsync(category);

        return (title, url);
    }

    /// <summary>
    /// Get manufacturer info with authorization check
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains manufacturer's title and URL
    ///</returns>
    protected virtual async Task<(string title, string url)> GetAuthorizedManufacturerInfoAsync(MenuItem menuItem)
    {
        var manufacturerId = menuItem.EntityId ?? 0;
        if (manufacturerId == 0)
            return (await _localizationService.GetLocalizedAsync(menuItem, m => m.Title), _nopUrlHelper.RouteUrl(NopRouteNames.General.MANUFACTURERS));

        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

        if (manufacturer is null || manufacturer.Deleted || !manufacturer.Published ||
            !await _aclService.AuthorizeAsync(manufacturer) ||
            !await _storeMappingService.AuthorizeAsync(manufacturer))
        {
            return (string.Empty, string.Empty);
        }

        var title = await _localizationService.GetLocalizedAsync(manufacturer, m => m.Name);
        var url = await _nopUrlHelper.RouteGenericUrlAsync(manufacturer);

        return (title, url);
    }

    /// <summary>
    /// Get product info with authorization check
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains product's title and URL
    ///</returns>
    protected virtual async Task<(string title, string url)> GetAuthorizedProductInfoAsync(MenuItem menuItem)
    {
        var productId = menuItem.EntityId ?? 0;
        if (productId == 0)
            return (await _localizationService.GetLocalizedAsync(menuItem, m => m.Title), string.Empty);

        var product = await _productService.GetProductByIdAsync(productId);

        if (product is null || product.Deleted || !product.Published ||
            !_productService.ProductIsAvailable(product) ||
            !await _aclService.AuthorizeAsync(product) ||
            !await _storeMappingService.AuthorizeAsync(product))
        {
            return (string.Empty, string.Empty);
        }

        var title = await _localizationService.GetLocalizedAsync(product, m => m.Name);
        var url = await _nopUrlHelper.RouteGenericUrlAsync(product);

        return (title, url);
    }

    /// <summary>
    /// Get topic info with authorization check
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains topic's title and URL
    ///</returns>
    protected virtual async Task<(string title, string url)> GetAuthorizedTopicInfoAsync(MenuItem menuItem)
    {
        var topicId = menuItem.EntityId ?? 0;
        if (topicId == 0)
            return (await _localizationService.GetLocalizedAsync(menuItem, m => m.Title), string.Empty);

        var topic = await _topicService.GetTopicByIdAsync(topicId);

        if (topic is null || !topic.Published ||
            !await _aclService.AuthorizeAsync(topic) ||
            !await _storeMappingService.AuthorizeAsync(topic))
        {
            return (string.Empty, string.Empty);
        }

        var title = await _localizationService.GetLocalizedAsync(topic, m => m.Title);
        var url = await _nopUrlHelper.RouteTopicUrlAsync(topic.SystemName);

        return (title, url);
    }

    /// <summary>
    /// Get vendor info with authorization check
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result vendor's title and URL
    ///</returns>
    protected virtual async Task<(string title, string url)> GetAuthorizedVendorInfoAsync(MenuItem menuItem)
    {
        var vendorId = menuItem.EntityId ?? 0;
        if (vendorId == 0)
            return (await _localizationService.GetLocalizedAsync(menuItem, m => m.Title), _nopUrlHelper.RouteUrl(NopRouteNames.General.VENDORS));

        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

        if (vendor is null || vendor.Deleted || !vendor.Active)
            return (string.Empty, string.Empty);

        var title = await _localizationService.GetLocalizedAsync(vendor, m => m.Name);
        var url = await _nopUrlHelper.RouteGenericUrlAsync(vendor);

        return (title, url);
    }

    /// <summary>
    /// Prepare picture model
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="title">Title</param>
    /// <param name="alternateText">Alternate text</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture model
    /// </returns>
    protected virtual async Task<PictureModel> PreparePictureModelAsync(int pictureId, string title, string alternateText)
    {
        var picture = await _pictureService.GetPictureByIdAsync(pictureId);
        (var fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
        (var imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, _menuSettings.GridThumbPictureSize);

        return new PictureModel
        {
            FullSizeImageUrl = fullSizeImageUrl,
            ImageUrl = imageUrl,
            Title = title,
            AlternateText = alternateText
        };
    }

    /// <summary>
    /// Prepare menu item models for all root categories
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a list of menu item models
    /// </returns>
    protected virtual async Task<List<MenuItemModel>> PrepareAllCategoriesMenuItemModelsAsync()
    {
        //root categories
        var categories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(parentCategoryId: 0);

        return await categories.SelectAwait(async c =>
        {
            var localizedName = await _localizationService.GetLocalizedAsync(c, x => x.Name);
            var item = new MenuItemModel
            {
                EntityId = c.Id,
                MenuItemType = MenuItemType.Category,
                Title = localizedName,
                Url = await _nopUrlHelper.RouteGenericUrlAsync(c),
                Template = MenuItemTemplate.List
            };

            item.ChildrenItems = await PrepareSubMenuItemsAsync(item, _menuSettings.MaximumNumberEntities, false);

            return item;
        }).ToListAsync();
    }

    /// <summary>
    /// Prepare the menu item model
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu item model
    /// </returns>
    protected virtual async Task<MenuItemModel> PrepareMenuItemModelAsync(MenuItem menuItem)
    {
        var entityId = menuItem.EntityId ?? 0;
        var title = string.Empty;
        var url = string.Empty;

        try
        {
            if (menuItem is null || !menuItem.Published || !await _aclService.AuthorizeAsync(menuItem) || !await _storeMappingService.AuthorizeAsync(menuItem))
                throw new NopException("Menu item not found");

            (title, url) = menuItem.MenuItemType switch
            {
                MenuItemType.StandardPage => (await _localizationService.GetLocalizedAsync(menuItem, mi => mi.Title), _nopUrlHelper.RouteUrl(menuItem.RouteName)),
                MenuItemType.TopicPage => await GetAuthorizedTopicInfoAsync(menuItem),
                MenuItemType.Category => await GetAuthorizedCategoryInfoAsync(menuItem),
                MenuItemType.Manufacturer => await GetAuthorizedManufacturerInfoAsync(menuItem),
                MenuItemType.Vendor => await GetAuthorizedVendorInfoAsync(menuItem),
                MenuItemType.Product => await GetAuthorizedProductInfoAsync(menuItem),
                MenuItemType.CustomLink => (await _localizationService.GetLocalizedAsync(menuItem, mi => mi.Title), menuItem.Url),
                MenuItemType.Text => (await _localizationService.GetLocalizedAsync(menuItem, m => m.Title), string.Empty),
                _ => (await _localizationService.GetLocalizedAsync(menuItem, m => m.Title), menuItem.Url)
            };
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync(ex.Message);
        }

        return new MenuItemModel
        {
            Id = menuItem.Id,
            CssClass = menuItem.CssClass,
            MenuItemType = menuItem.MenuItemType,
            Template = menuItem.Template,
            ParentId = menuItem.ParentId,
            EntityId = entityId,
            MaximumNumberEntities = menuItem.MaximumNumberEntities ?? _menuSettings.MaximumNumberEntities,
            NumberOfSubItemsPerGridElement = menuItem.NumberOfSubItemsPerGridElement ?? _menuSettings.NumberOfSubItemsPerGridElement,
            NumberOfItemsPerGridRow = menuItem.NumberOfItemsPerGridRow ?? _menuSettings.NumberOfItemsPerGridRow,
            Title = title,
            Url = url,
            ChildrenItems = new()
        };
    }

    /// <summary>
    /// Prepare menu item models for provided menu 
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains menu item models
    /// </returns>
    protected virtual async Task<List<MenuItemModel>> PrepareMenuItemModelsAsync(Menu menu)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var allItems = await _menuService.GetAllMenuItemsAsync(menuId: menu.Id, storeId: store.Id);
        var result = new List<MenuItemModel>();
        var rootItems = allItems.Where(item => item.ParentId == 0);

        foreach (var rootItem in rootItems)
        {
            var rootItemModel = await PrepareMenuItemModelAsync(rootItem);

            if (string.IsNullOrEmpty(rootItemModel.Title) && string.IsNullOrEmpty(rootItemModel.Url))
                continue;

            if (allItems.Any(i => i.ParentId == rootItemModel.Id))
                rootItemModel.Template = MenuItemTemplate.Simple;

            await getItemTree(rootItemModel);

            if (!rootItemModel.ChildrenItems.Any() && rootItemModel.Template != MenuItemTemplate.Simple)
            {
                rootItemModel.ChildrenItems = await PrepareSubMenuItemsAsync(rootItemModel, rootItemModel.MaximumNumberEntities, rootItemModel.Template == MenuItemTemplate.Grid);
            }

            result.Add(rootItemModel);
        }

        if (menu.MenuType == MenuType.Main && menu.DisplayAllCategories)
            result.InsertRange(0, await PrepareAllCategoriesMenuItemModelsAsync());

        return result;

        async Task getItemTree(MenuItemModel parentItem)
        {
            var children = allItems
                .Where(item => item.ParentId == parentItem.Id);

            foreach (var item in children)
            {
                var menuItemModel = await PrepareMenuItemModelAsync(item);

                if (string.IsNullOrEmpty(menuItemModel.Title) && string.IsNullOrEmpty(menuItemModel.Url))
                    continue;

                await getItemTree(menuItemModel);

                parentItem.ChildrenItems.Add(menuItemModel);
            }
        }
    }

    /// <summary>
    /// Prepare children menu item models for supported entities
    /// </summary>
    /// <param name="menuItemModel">Menu item model</param>
    /// <param name="limitItems">Limitation on the number of returned items</param>
    /// <param name="loadChildren">Value that indicates whether to load child items for the found items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains menu item models
    /// </returns>
    protected virtual async Task<List<MenuItemModel>> PrepareSubMenuItemsAsync(MenuItemModel menuItemModel, int limitItems, bool loadChildren = false)
    {
        if (menuItemModel.Template == MenuItemTemplate.Simple)
            return new();

        var store = await _storeContext.GetCurrentStoreAsync();

        return menuItemModel.MenuItemType switch
        {
            MenuItemType.Category => await (await _categoryService.GetAllCategoriesByParentCategoryIdAsync(menuItemModel.EntityId))
                .SelectAwait(async category =>
                {
                    var localizedName = await _localizationService.GetLocalizedAsync(category, x => x.Name);
                    var item = new MenuItemModel
                    {
                        EntityId = category.Id,
                        MenuItemType = MenuItemType.Category,
                        Title = localizedName,
                        Url = await _nopUrlHelper.RouteGenericUrlAsync(category),
                        Picture = loadChildren ? await PreparePictureModelAsync(category.PictureId, localizedName, localizedName) : null,
                        Template = MenuItemTemplate.List
                    };

                    if (loadChildren)
                        item.ChildrenItems = await PrepareSubMenuItemsAsync(item, menuItemModel.NumberOfSubItemsPerGridElement, false);

                    return item;
                }).Take(limitItems).ToListAsync(),

            MenuItemType.Manufacturer => await (await _manufacturerService.GetAllManufacturersAsync(storeId: store.Id, pageSize: limitItems))
                .SelectAwait(async manufacturer =>
                {
                    var localizedName = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name);
                    var item = new MenuItemModel
                    {
                        EntityId = manufacturer.Id,
                        Title = localizedName,
                        Url = await _nopUrlHelper.RouteGenericUrlAsync(manufacturer),
                        Picture = await PreparePictureModelAsync(manufacturer.PictureId, localizedName, localizedName),
                        MenuItemType = MenuItemType.Manufacturer,
                        Template = MenuItemTemplate.Simple
                    };

                    return item;
                }).ToListAsync(),

            MenuItemType.Vendor => await (await _vendorService.GetAllVendorsAsync(pageSize: limitItems))
                .SelectAwait(async vendor =>
                {
                    var localizedName = await _localizationService.GetLocalizedAsync(vendor, x => x.Name);
                    var item = new MenuItemModel
                    {
                        EntityId = vendor.Id,
                        Title = localizedName,
                        Url = await _nopUrlHelper.RouteGenericUrlAsync(vendor),
                        Picture = await PreparePictureModelAsync(vendor.PictureId, localizedName, localizedName),
                        MenuItemType = MenuItemType.Vendor,
                        Template = MenuItemTemplate.Simple
                    };

                    return item;
                }).ToListAsync(),

            _ => new()
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the menu model
    /// </summary>
    /// <param name="menuType">Menu type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains list of menu models
    /// </returns>
    public virtual async Task<IList<MenuModel>> PrepareMenuModelsAsync(MenuType menuType)
    {
        var language = await _workContext.GetWorkingLanguageAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
        var store = await _storeContext.GetCurrentStoreAsync();
        var menuCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.MenuByTypeModelKey, menuType, customerRoleIds, store, language);

        return await _staticCacheManager.GetAsync(menuCacheKey, async () =>
        {
            var menus = await _menuService.GetAllMenusAsync(menuType, store.Id);

            return await menus.SelectAwait(async menu => new MenuModel
            {
                Id = menu.Id,
                MenuType = (MenuType)menu.MenuTypeId,
                Name = await _localizationService.GetLocalizedAsync(menu, m => m.Name),
                CssClass = menu.CssClass,
                Items = await PrepareMenuItemModelsAsync(menu)
            }).ToListAsync();
        });
    }

    #endregion
}