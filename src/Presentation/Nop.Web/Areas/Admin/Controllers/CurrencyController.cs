using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CurrencyController : BaseAdminController
    {
        #region Fields

        protected CurrencySettings CurrencySettings { get; }
        protected ICurrencyModelFactory CurrencyModelFactory { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected ISettingService SettingService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }

        #endregion

        #region Ctor

        public CurrencyController(CurrencySettings currencySettings,
            ICurrencyModelFactory currencyModelFactory,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            CurrencySettings = currencySettings;
            CurrencyModelFactory = currencyModelFactory;
            CurrencyService = currencyService;
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            SettingService = settingService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Currency currency, CurrencyModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(currency, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(Currency currency, CurrencyModel model)
        {
            currency.LimitedToStores = model.SelectedStoreIds.Any();
            await CurrencyService.UpdateCurrencyAsync(currency);

            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(currency);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await StoreMappingService.InsertStoreMappingAsync(currency, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await StoreMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List(bool liveRates = false)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var model = new CurrencySearchModel();

            try
            {
                //prepare model
                model = await CurrencyModelFactory.PrepareCurrencySearchModelAsync(model, liveRates);
            }
            catch (Exception e)
            {
                await NotificationService.ErrorNotificationAsync(e);
            }

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> List(CurrencySearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            CurrencySettings.ActiveExchangeRateProviderSystemName = model.ExchangeRateProviderModel.ExchangeRateProvider;
            CurrencySettings.AutoUpdateEnabled = model.ExchangeRateProviderModel.AutoUpdateEnabled;
            await SettingService.SaveSettingAsync(CurrencySettings);

            return RedirectToAction("List", "Currency");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ListGrid(CurrencySearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CurrencyModelFactory.PrepareCurrencyListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApplyRates(IEnumerable<CurrencyExchangeRateModel> rateModels)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            foreach (var rate in rateModels)
            {
                var currency = await CurrencyService.GetCurrencyByCodeAsync(rate.CurrencyCode);
                if (currency == null)
                    continue;

                currency.Rate = rate.Rate;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                await CurrencyService.UpdateCurrencyAsync(currency);
            }

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> MarkAsPrimaryExchangeRateCurrency(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            CurrencySettings.PrimaryExchangeRateCurrencyId = id;
            await SettingService.SaveSettingAsync(CurrencySettings);

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> MarkAsPrimaryStoreCurrency(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            CurrencySettings.PrimaryStoreCurrencyId = id;
            await SettingService.SaveSettingAsync(CurrencySettings);

            return Json(new { result = true });
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //prepare model
            var model = await CurrencyModelFactory.PrepareCurrencyModelAsync(new CurrencyModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CurrencyModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var currency = model.ToEntity<Currency>();
                currency.CreatedOnUtc = DateTime.UtcNow;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                await CurrencyService.InsertCurrencyAsync(currency);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewCurrency",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewCurrency"), currency.Id), currency);

                //locales
                await UpdateLocalesAsync(currency, model);

                //stores
                await SaveStoreMappingsAsync(currency, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Currencies.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = currency.Id });
            }

            //prepare model
            model = await CurrencyModelFactory.PrepareCurrencyModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //try to get a currency with the specified id
            var currency = await CurrencyService.GetCurrencyByIdAsync(id);
            if (currency == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CurrencyModelFactory.PrepareCurrencyModelAsync(null, currency);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CurrencyModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //try to get a currency with the specified id
            var currency = await CurrencyService.GetCurrencyByIdAsync(model.Id);
            if (currency == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published currency
                var allCurrencies = await CurrencyService.GetAllCurrenciesAsync();
                if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id && !model.Published)
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                    return RedirectToAction("Edit", new { id = currency.Id });
                }

                currency = model.ToEntity(currency);
                currency.UpdatedOnUtc = DateTime.UtcNow;
                await CurrencyService.UpdateCurrencyAsync(currency);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditCurrency",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditCurrency"), currency.Id), currency);

                //locales
                await UpdateLocalesAsync(currency, model);

                //stores
                await SaveStoreMappingsAsync(currency, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Currencies.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = currency.Id });
            }

            //prepare model
            model = await CurrencyModelFactory.PrepareCurrencyModelAsync(model, currency, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //try to get a currency with the specified id
            var currency = await CurrencyService.GetCurrencyByIdAsync(id);
            if (currency == null)
                return RedirectToAction("List");

            try
            {
                if (currency.Id == CurrencySettings.PrimaryStoreCurrencyId)
                    throw new NopException(await LocalizationService.GetResourceAsync("Admin.Configuration.Currencies.CantDeletePrimary"));

                if (currency.Id == CurrencySettings.PrimaryExchangeRateCurrencyId)
                    throw new NopException(await LocalizationService.GetResourceAsync("Admin.Configuration.Currencies.CantDeleteExchange"));

                //ensure we have at least one published currency
                var allCurrencies = await CurrencyService.GetAllCurrenciesAsync();
                if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id)
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                    return RedirectToAction("Edit", new { id = currency.Id });
                }

                await CurrencyService.DeleteCurrencyAsync(currency);

                //activity log
                await CustomerActivityService.InsertActivityAsync("DeleteCurrency",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteCurrency"), currency.Id), currency);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Currencies.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = currency.Id });
            }
        }

        #endregion
    }
}