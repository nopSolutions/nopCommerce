using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Stores;
using Nop.Core.Domain.Stores;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Core.Domain.Discounts;
using Nop.Services.Discounts;

namespace Nop.Admin.Controllers
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
        private readonly IDiscountService _discountService;

        #endregion

        #region Constructors

        public StoreController(IStoreService storeService,
            ISettingService settingService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService,
            IDiscountService discountService)
        {
            this._storeService = storeService;
            this._settingService = settingService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._discountService = discountService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareLanguagesModel(StoreModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            
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

        [NonAction]
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

        [NonAction]
        protected virtual void PrepareDiscountModel(StoreModel model, Store store, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvailableDiscounts = _discountService
                .GetAllDiscounts(DiscountType.AssignedToStores, null, null, true)
                .Select(d => new StoreDiscountModel
                {
                    Id = d.Id,
                    Name = d.Name,
                })
                .ToList();

            if (!excludeProperties && store != null)
            {
                model.SelectedDiscountIds = store.AppliedDiscounts.Select(d => d.Id).ToArray();
            }
        }

        #endregion

        #region Methods

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var storeModels = _storeService.GetAllStores()
                .Select(x => x.ToModel())
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = storeModels,
                Total = storeModels.Count()
            };

            return Json(gridModel);
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var model = new StoreModel();
            //languages
            PrepareLanguagesModel(model);
            //locales
            AddLocales(_languageService, model.Locales);
            //discounts
            PrepareDiscountModel(model, null, true);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(StoreModel model, bool continueEditing)
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
                //locales
                UpdateAttributeLocales(store, model);
                //discounts
                var discounts = _discountService.GetAllDiscounts(DiscountType.AssignedToStores, null, null, true);
                foreach (var discount in discounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        store.AppliedDiscounts.Add(discount);
                }
                _storeService.UpdateStore(store);
                //update "HasDiscountsApplied" property
                _storeService.UpdateHasDiscountsApplied(store);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //languages
            PrepareLanguagesModel(model);
            //discounts
            PrepareDiscountModel(model, null, true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(id);
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
            //discounts
            PrepareDiscountModel(model, store, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(StoreModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(model.Id);
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
                //locales
                UpdateAttributeLocales(store, model);
                //discounts
                var discounts = _discountService.GetAllDiscounts(DiscountType.AssignedToStores, null, null, true);
                foreach (var discount in discounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (store.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            store.AppliedDiscounts.Add(discount);
                    }
                    else
                    {
                        //remove discount
                        if (store.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                            store.AppliedDiscounts.Remove(discount);
                    }
                }
                _storeService.UpdateStore(store);
                //update "HasDiscountsApplied" property
                _storeService.UpdateHasDiscountsApplied(store);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //languages
            PrepareLanguagesModel(model);
            //discounts
            PrepareDiscountModel(model, store, false);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(id);
            if (store == null)
                //No store found with the specified id
                return RedirectToAction("List");

            try
            {
                _storeService.DeleteStore(store);

                //when we delete a store we should also ensure that all "per store" settings will also be deleted
                var settingsToDelete = _settingService
                    .GetAllSettings()
                    .Where(s => s.StoreId == id)
                    .ToList();
                    _settingService.DeleteSettings(settingsToDelete);
                //when we had two stores and now have only one store, we also should delete all "per store" settings
                var allStores = _storeService.GetAllStores();
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
