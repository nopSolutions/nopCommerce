using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class StoreController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreModelFactory _storeModelFactory;
    protected readonly IStoreService _storeService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public StoreController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreModelFactory storeModelFactory,
        IStoreService storeService,
        IGenericAttributeService genericAttributeService,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeModelFactory = storeModelFactory;
        _storeService = storeService;
        _genericAttributeService = genericAttributeService;
        _webHelper = webHelper;
        _workContext = workContext;

    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Store store, StoreModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.Name,
                localized.Name,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.DefaultTitle,
                localized.DefaultTitle,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.DefaultMetaDescription,
                localized.DefaultMetaDescription,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.DefaultMetaKeywords,
                localized.DefaultMetaKeywords,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.HomepageDescription,
                localized.HomepageDescription,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.HomepageTitle,
                localized.HomepageTitle,
                localized.LanguageId);
        }
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _storeModelFactory.PrepareStoreSearchModelAsync(new StoreSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> List(StoreSearchModel searchModel)
    {
        //prepare model
        var model = await _storeModelFactory.PrepareStoreListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _storeModelFactory.PrepareStoreModelAsync(new StoreModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> Create(StoreModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var store = model.ToEntity<Store>();

            //ensure we have "/" at the end
            if (!store.Url.EndsWith("/"))
                store.Url += "/";

            await _storeService.InsertStoreAsync(store);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewStore",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewStore"), store.Id), store);

            //locales
            await UpdateLocalesAsync(store, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _storeModelFactory.PrepareStoreModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpsRequirement(ignore: true)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> SetStoreSslByCurrentRequestScheme(int id)
    {
        //try to get a store with the specified id
        var store = await _storeService.GetStoreByIdAsync(id);
        if (store == null)
            return RedirectToAction("List");

        var value = _webHelper.IsCurrentConnectionSecured();

        if (store.SslEnabled != value)
        {
            store.SslEnabled = value;
            await _storeService.UpdateStoreAsync(store);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Ssl.Updated"));
        }

        return RedirectToAction("Edit", new { id = id });
    }

    [HttpsRequirement(ignore: true)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> Edit(int id, bool showtour = false)
    {
        //try to get a store with the specified id
        var store = await _storeService.GetStoreByIdAsync(id);
        if (store == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _storeModelFactory.PrepareStoreModelAsync(null, store);

        //show configuration tour
        if (showtour)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
            var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

            if (!hideCard && !closeCard)
                ViewBag.ShowTour = true;
        }

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> Edit(StoreModel model, bool continueEditing)
    {
        //try to get a store with the specified id
        var store = await _storeService.GetStoreByIdAsync(model.Id);
        if (store == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            store = model.ToEntity(store);

            //ensure we have "/" at the end
            if (!store.Url.EndsWith("/"))
                store.Url += "/";

            await _storeService.UpdateStoreAsync(store);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditStore",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditStore"), store.Id), store);

            //locales
            await UpdateLocalesAsync(store, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _storeModelFactory.PrepareStoreModelAsync(model, store, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a store with the specified id
        var store = await _storeService.GetStoreByIdAsync(id);
        if (store == null)
            return RedirectToAction("List");

        try
        {
            await _storeService.DeleteStoreAsync(store);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteStore",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteStore"), store.Id), store);

            //when we delete a store we should also ensure that all "per store" settings will also be deleted
            var settingsToDelete = (await _settingService
                    .GetAllSettingsAsync())
                .Where(s => s.StoreId == id)
                .ToList();
            await _settingService.DeleteSettingsAsync(settingsToDelete);

            //when we had two stores and now have only one store, we also should delete all "per store" settings
            var allStores = await _storeService.GetAllStoresAsync();
            if (allStores.Count == 1)
            {
                settingsToDelete = (await _settingService
                        .GetAllSettingsAsync())
                    .Where(s => s.StoreId == allStores[0].Id)
                    .ToList();
                await _settingService.DeleteSettingsAsync(settingsToDelete);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("Edit", new { id = store.Id });
        }
    }

    #endregion
}