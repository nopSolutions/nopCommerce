using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Menus;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Menus;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class MenuController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly IMenuModelFactory _menuModelFactory;
    protected readonly IMenuService _menuService;
    protected readonly INotificationService _notificationService;
    protected readonly IProductService _productService;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public MenuController(
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        IMenuModelFactory menuModelFactory,
        IMenuService menuService,
        INotificationService notificationService,
        IProductService productService,
        IStoreMappingService storeMappingService)
    {
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _menuModelFactory = menuModelFactory;
        _menuService = menuService;
        _notificationService = notificationService;
        _productService = productService;
        _storeMappingService = storeMappingService;
    }

    #endregion

    #region Utilities

    protected virtual void DefinePopupViewBagData(int entityId, string entityInfo)
    {
        ViewBag.RefreshPage = true;
        ViewBag.entityId = entityId;
        ViewBag.entityInfo = entityInfo;
    }

    protected virtual async Task<MenuItem> SaveMenuItemAsync(Menu menu, MenuItemModel model, MenuItem menuItem, Func<MenuItem, Task> serviceAction)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(menuItem);

        menuItem = model.ToEntity(menuItem);

        menuItem.EntityId = menuItem.MenuItemType switch
        {
            MenuItemType.Category => model.CategoryId,
            MenuItemType.TopicPage => model.TopicId,
            MenuItemType.Manufacturer => model.ManufacturerId,
            MenuItemType.Vendor => model.VendorId,
            MenuItemType.Product => model.ProductId,
            _ => null
        };

        if ((menuItem.MenuItemType != MenuItemType.Category && (menuItem.EntityId ?? 0) != 0) || menu.MenuType == MenuType.Footer)
            menuItem.Template = MenuItemTemplate.Simple;

        if (menuItem.MenuItemType != MenuItemType.StandardPage)
            menuItem.RouteName = null;

        await serviceAction(menuItem);

        //stores
        await _storeMappingService.SaveStoreMappingsAsync(menuItem, model.SelectedStoreIds);

        //locales
        await UpdateMenuItemLocalesAsync(menuItem, model);

        return menuItem;
    }

    protected virtual async Task UpdateMenuLocalesAsync(Menu menu, MenuModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(menu,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    protected virtual async Task UpdateMenuItemLocalesAsync(MenuItem menuItem, MenuItemModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(menuItem,
                x => x.Title,
                localized.Title,
                localized.LanguageId);
        }
    }

    #endregion

    #region Methods

    #region Menus

    public virtual IActionResult Index()
    {
        return RedirectToAction(nameof(List));
    }

    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _menuModelFactory.PrepareMenuSearchModelAsync(new MenuSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> MenuList(MenuSearchModel searchModel)
    {
        //prepare model
        var model = await _menuModelFactory.PrepareMenuListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> MenuItemList(MenuItemSearchModel searchModel)
    {
        //prepare model
        var model = await _menuModelFactory.PrepareMenuItemListModelAsync(searchModel);

        return Json(model);
    }


    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _menuModelFactory.PrepareMenuModelAsync(new MenuModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(MenuModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var menu = model.ToEntity<Menu>();
            await _menuService.InsertMenuAsync(menu);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(menu, model.SelectedCustomerRoleIds);

            await UpdateMenuLocalesAsync(menu, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Added"));

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewMenu",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewMenu"), menu.Name), menu);

            if (!continueEditing)
                return RedirectToAction(nameof(List));

            return RedirectToAction(nameof(Edit), new { id = menu.Id });
        }

        //prepare model
        model = await _menuModelFactory.PrepareMenuModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        var menu = await _menuService.GetMenuByIdAsync(id);
        if (menu == null)
            return RedirectToAction(nameof(List));

        //prepare model
        var model = await _menuModelFactory.PrepareMenuModelAsync(null, menu);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(MenuModel model, bool continueEditing)
    {
        var menu = await _menuService.GetMenuByIdAsync(model.Id);
        if (menu == null)
            return RedirectToAction(nameof(List));

        if (ModelState.IsValid)
        {
            menu = model.ToEntity(menu);
            await _menuService.UpdateMenuAsync(menu);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(menu, model.SelectedStoreIds);

            await UpdateMenuLocalesAsync(menu, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Updated"));

            //activity log
            await _customerActivityService.InsertActivityAsync("EditMenu",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMenu"), menu.Name), menu);

            if (!continueEditing)
                return RedirectToAction(nameof(List));

            return RedirectToAction(nameof(Edit), new { id = menu.Id });
        }

        //prepare model
        model = await _menuModelFactory.PrepareMenuModelAsync(model, menu, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        var menu = await _menuService.GetMenuByIdAsync(id);
        if (menu is null)
            return RedirectToAction(nameof(List));

        await _menuService.DeleteMenuAsync(menu);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Deleted"));

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteMenu",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMenu"), menu.Name), menu);

        return RedirectToAction(nameof(List));
    }

    #endregion

    #region Menu items

    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MenuItemCreate(int menuId)
    {
        var menu = await _menuService.GetMenuByIdAsync(menuId);
        if (menu == null)
            return RedirectToAction(nameof(List));

        //prepare model
        var model = await _menuModelFactory.PrepareMenuItemModelAsync(menu, new MenuItemModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MenuItemCreate(MenuItemModel model, bool continueEditing)
    {
        var menu = await _menuService.GetMenuByIdAsync(model.MenuId);
        if (menu == null)
            return RedirectToAction(nameof(List));

        if (ModelState.IsValid)
        {
            var menuItem = await SaveMenuItemAsync(menu, model, model.ToEntity<MenuItem>(), _menuService.InsertMenuItemAsync);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Added"));

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewMenuItem",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewMenuItem"), menuItem.Id), menuItem);

            if (!continueEditing)
                return RedirectToAction(nameof(Edit), new { id = menuItem.MenuId });

            return RedirectToAction(nameof(MenuItemEdit), new { id = menuItem.Id });
        }

        //prepare model
        model = await _menuModelFactory.PrepareMenuItemModelAsync(menu, model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> MenuItemEdit(int id)
    {
        var menuItem = await _menuService.GetMenuItemByIdAsync(id);
        if (menuItem == null)
            return RedirectToAction(nameof(List));

        var menu = await _menuService.GetMenuByIdAsync(menuItem.MenuId);
        if (menu == null)
            return RedirectToAction(nameof(List));

        //prepare model
        var model = await _menuModelFactory.PrepareMenuItemModelAsync(menu, null, menuItem);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MenuItemEdit(MenuItemModel model, bool continueEditing)
    {
        var menu = await _menuService.GetMenuByIdAsync(model.MenuId);
        if (menu == null)
            return RedirectToAction(nameof(List));

        var menuItem = await _menuService.GetMenuItemByIdAsync(model.Id);
        if (menuItem == null)
            return RedirectToAction(nameof(List));

        if (ModelState.IsValid)
        {
            await SaveMenuItemAsync(menu, model, menuItem, _menuService.UpdateMenuItemAsync);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItems.Updated"));

            //activity log
            await _customerActivityService.InsertActivityAsync("EditMenuItem",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMenuItem"), menuItem.Title), menuItem);

            if (!continueEditing)
                return RedirectToAction(nameof(Edit), new { id = menuItem.MenuId });

            return RedirectToAction(nameof(MenuItemEdit), new { id = menuItem.Id });
        }

        //prepare model
        model = await _menuModelFactory.PrepareMenuItemModelAsync(menu, model, menuItem, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MenuItemDelete(int id)
    {
        var menuItem = await _menuService.GetMenuItemByIdAsync(id);
        if (menuItem is null)
            return RedirectToAction(nameof(List));

        await _menuService.DeleteMenuItemAsync(menuItem);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItems.Deleted"));

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteMenuItem",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMenuItem"), menuItem.Title), menuItem);

        return RedirectToAction(nameof(Edit), new { id = menuItem.MenuId });
    }

    #endregion

    #region Popups

    #region Products

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> MenuItemSelectProductPopup(int menuItemId)
    {
        //prepare model
        var model = await _menuModelFactory.PrepareMenuItemSelectProductSearchModelAsync(new SelectMenuItemProductSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> MenuItemSelectProductPopupList(SelectMenuItemProductSearchModel searchModel)
    {
        //prepare model
        var model = await _menuModelFactory.PrepareMenuItemSelectProductListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MenuItemSelectProductPopup(SelectMenuItemEntityModel model)
    {
        var product = await _productService.GetProductByIdAsync(model.EntityId)
            ?? throw new ArgumentException("No products found with the specified id");

        DefinePopupViewBagData(product.Id, product.Name);

        return View(new SelectMenuItemProductSearchModel());
    }
    #endregion

    #endregion

    #endregion
}
