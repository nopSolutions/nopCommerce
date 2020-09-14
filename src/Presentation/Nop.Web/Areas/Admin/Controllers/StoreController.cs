using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Stores;
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

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class StoreController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreModelFactory _storeModelFactory;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public StoreController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreModelFactory storeModelFactory,
            IStoreService storeService)
        {
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeModelFactory = storeModelFactory;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocales(Store store, StoreModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(store,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //prepare model
            var model = await _storeModelFactory.PrepareStoreSearchModel(new StoreSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(StoreSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _storeModelFactory.PrepareStoreListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //prepare model
            var model = await _storeModelFactory.PrepareStoreModel(new StoreModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(StoreModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var store = model.ToEntity<Store>();

                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";

                await _storeService.InsertStore(store);

                //activity log
                await _customerActivityService.InsertActivity("AddNewStore",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewStore"), store.Id), store);

                //locales
                await UpdateAttributeLocales(store, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Stores.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _storeModelFactory.PrepareStoreModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await _storeService.GetStoreById(id);
            if (store == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _storeModelFactory.PrepareStoreModel(null, store);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(StoreModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await _storeService.GetStoreById(model.Id);
            if (store == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                store = model.ToEntity(store);

                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";

                await _storeService.UpdateStore(store);

                //activity log
                await _customerActivityService.InsertActivity("EditStore",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditStore"), store.Id), store);

                //locales
                await UpdateAttributeLocales(store, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Stores.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _storeModelFactory.PrepareStoreModel(model, store, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await _storeService.GetStoreById(id);
            if (store == null)
                return RedirectToAction("List");

            try
            {
                await _storeService.DeleteStore(store);

                //activity log
                await _customerActivityService.InsertActivity("DeleteStore",
                    string.Format(await _localizationService.GetResource("ActivityLog.DeleteStore"), store.Id), store);

                //when we delete a store we should also ensure that all "per store" settings will also be deleted
                var settingsToDelete = (await _settingService
                    .GetAllSettings())
                    .Where(s => s.StoreId == id)
                    .ToList();
                await _settingService.DeleteSettings(settingsToDelete);

                //when we had two stores and now have only one store, we also should delete all "per store" settings
                var allStores = await _storeService.GetAllStores();
                if (allStores.Count == 1)
                {
                    settingsToDelete = (await _settingService
                        .GetAllSettings())
                        .Where(s => s.StoreId == allStores[0].Id)
                        .ToList();
                    await _settingService.DeleteSettings(settingsToDelete);
                }

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Stores.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = store.Id });
            }
        }

        #endregion
    }
}