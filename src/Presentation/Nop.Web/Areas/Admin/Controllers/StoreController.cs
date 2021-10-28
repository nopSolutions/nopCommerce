using System;
using System.Linq;
using System.Threading.Tasks;
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

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class StoreController : BaseAdminController
    {
        #region Fields

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected ISettingService SettingService { get; }
        protected IStoreModelFactory StoreModelFactory { get; }
        protected IStoreService StoreService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IWorkContext WorkContext { get; }

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
            IWorkContext workContext)
        {
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            SettingService = settingService;
            StoreModelFactory = storeModelFactory;
            StoreService = storeService;
            GenericAttributeService = genericAttributeService;
            WorkContext = workContext;

        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocalesAsync(Store store, StoreModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(store,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //prepare model
            var model = await StoreModelFactory.PrepareStoreSearchModelAsync(new StoreSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(StoreSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await StoreModelFactory.PrepareStoreListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //prepare model
            var model = await StoreModelFactory.PrepareStoreModelAsync(new StoreModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(StoreModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var store = model.ToEntity<Store>();

                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";

                await StoreService.InsertStoreAsync(store);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewStore",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewStore"), store.Id), store);

                //locales
                await UpdateAttributeLocalesAsync(store, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Stores.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await StoreModelFactory.PrepareStoreModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id, bool showtour = false)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await StoreService.GetStoreByIdAsync(id);
            if (store == null)
                return RedirectToAction("List");

            //prepare model
            var model = await StoreModelFactory.PrepareStoreModelAsync(null, store);

            //show configuration tour
            if (showtour)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var hideCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(StoreModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await StoreService.GetStoreByIdAsync(model.Id);
            if (store == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                store = model.ToEntity(store);

                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";

                await StoreService.UpdateStoreAsync(store);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditStore",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditStore"), store.Id), store);

                //locales
                await UpdateAttributeLocalesAsync(store, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Stores.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await StoreModelFactory.PrepareStoreModelAsync(model, store, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await StoreService.GetStoreByIdAsync(id);
            if (store == null)
                return RedirectToAction("List");

            try
            {
                await StoreService.DeleteStoreAsync(store);

                //activity log
                await CustomerActivityService.InsertActivityAsync("DeleteStore",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteStore"), store.Id), store);

                //when we delete a store we should also ensure that all "per store" settings will also be deleted
                var settingsToDelete = (await SettingService
                    .GetAllSettingsAsync())
                    .Where(s => s.StoreId == id)
                    .ToList();
                await SettingService.DeleteSettingsAsync(settingsToDelete);

                //when we had two stores and now have only one store, we also should delete all "per store" settings
                var allStores = await StoreService.GetAllStoresAsync();
                if (allStores.Count == 1)
                {
                    settingsToDelete = (await SettingService
                        .GetAllSettingsAsync())
                        .Where(s => s.StoreId == allStores[0].Id)
                        .ToList();
                    await SettingService.DeleteSettingsAsync(settingsToDelete);
                }

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Stores.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = store.Id });
            }
        }

        #endregion
    }
}