using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Core.Domain.Stores;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class StoreController : BaseAdminController
    {
        #region Fields

        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Ctor

        public StoreController(IStoreService storeService,
            ISettingService settingService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService)
        {
            this._storeService = storeService;
            this._settingService = settingService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
        }

        #endregion

        #region Utilities

        protected virtual void PrepareLanguagesModel(StoreModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            
            model.AvailableLanguages.Add(new SelectListItem
            {
                Text = "---",
                Value = "0"
            });
            var languages = _languageService.GetAllLanguages(true);
            foreach (var language in languages)
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Text = language.Name,
                    Value = language.Id.ToString()
                });
            }
        }

        protected virtual void UpdateAttributeLocales(Store store, StoreModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(store,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedKendoGridJson();

            var storeModels = _storeService.GetAllStores(false)
                .Select(x => x.ToModel())
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = storeModels,
                Total = storeModels.Count()
            };

            return Json(gridModel);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var model = new StoreModel();
            //languages
            PrepareLanguagesModel(model);
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(StoreModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();
            
            if (ModelState.IsValid)
            {
                var store = model.ToEntity();
                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";
                _storeService.InsertStore(store);

                //activity log
                _customerActivityService.InsertActivity("AddNewStore", _localizationService.GetResource("ActivityLog.AddNewStore"), store.Id);

                //locales
                UpdateAttributeLocales(store, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //languages
            PrepareLanguagesModel(model);
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(id, false);
            if (store == null)
                //No store found with the specified id
                return RedirectToAction("List");

            var model = store.ToModel();
            //languages
            PrepareLanguagesModel(model);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = store.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(StoreModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(model.Id, false);
            if (store == null)
                //No store found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                store = model.ToEntity(store);
                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";
                _storeService.UpdateStore(store);

                //activity log
                _customerActivityService.InsertActivity("EditStore", _localizationService.GetResource("ActivityLog.EditStore"), store.Id);

                //locales
                UpdateAttributeLocales(store, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //languages
            PrepareLanguagesModel(model);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(id, false);
            if (store == null)
                //No store found with the specified id
                return RedirectToAction("List");

            try
            {
                _storeService.DeleteStore(store);

                //activity log
                _customerActivityService.InsertActivity("DeleteStore", _localizationService.GetResource("ActivityLog.DeleteStore"), store.Id);

                //when we delete a store we should also ensure that all "per store" settings will also be deleted
                var settingsToDelete = _settingService
                    .GetAllSettings()
                    .Where(s => s.StoreId == id)
                    .ToList();
                    _settingService.DeleteSettings(settingsToDelete);
                //when we had two stores and now have only one store, we also should delete all "per store" settings
                var allStores = _storeService.GetAllStores(false);
                if (allStores.Count == 1)
                {
                    settingsToDelete = _settingService
                        .GetAllSettings()
                        .Where(s => s.StoreId == allStores[0].Id)
                        .ToList();
                        _settingService.DeleteSettings(settingsToDelete);
                }

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new {id = store.Id});
            }
        }

        #endregion
    }
}