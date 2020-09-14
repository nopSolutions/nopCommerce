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

        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyModelFactory _currencyModelFactory;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

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
            _currencySettings = currencySettings;
            _currencyModelFactory = currencyModelFactory;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocales(Currency currency, CurrencyModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(currency, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        protected virtual async Task SaveStoreMappings(Currency currency, CurrencyModel model)
        {
            currency.LimitedToStores = model.SelectedStoreIds.Any();
            await _currencyService.UpdateCurrency(currency);

            var existingStoreMappings = await _storeMappingService.GetStoreMappings(currency);
            var allStores = await _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await _storeMappingService.InsertStoreMapping(currency, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
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
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var model = new CurrencySearchModel();

            try
            {
                //prepare model
                model = await _currencyModelFactory.PrepareCurrencySearchModel(model, liveRates);
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e);
            }

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> List(CurrencySearchModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            _currencySettings.ActiveExchangeRateProviderSystemName = model.ExchangeRateProviderModel.ExchangeRateProvider;
            _currencySettings.AutoUpdateEnabled = model.ExchangeRateProviderModel.AutoUpdateEnabled;
            await _settingService.SaveSetting(_currencySettings);

            return RedirectToAction("List", "Currency");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ListGrid(CurrencySearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _currencyModelFactory.PrepareCurrencyListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApplyRates(IEnumerable<CurrencyExchangeRateModel> rateModels)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            foreach (var rate in rateModels)
            {
                var currency = await _currencyService.GetCurrencyByCode(rate.CurrencyCode);
                if (currency == null)
                    continue;

                currency.Rate = rate.Rate;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                await _currencyService.UpdateCurrency(currency);
            }

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> MarkAsPrimaryExchangeRateCurrency(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            _currencySettings.PrimaryExchangeRateCurrencyId = id;
            await _settingService.SaveSetting(_currencySettings);

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> MarkAsPrimaryStoreCurrency(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            _currencySettings.PrimaryStoreCurrencyId = id;
            await _settingService.SaveSetting(_currencySettings);

            return Json(new { result = true });
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //prepare model
            var model = await _currencyModelFactory.PrepareCurrencyModel(new CurrencyModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CurrencyModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var currency = model.ToEntity<Currency>();
                currency.CreatedOnUtc = DateTime.UtcNow;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                await _currencyService.InsertCurrency(currency);

                //activity log
                await _customerActivityService.InsertActivity("AddNewCurrency",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewCurrency"), currency.Id), currency);

                //locales
                await UpdateLocales(currency, model);

                //stores
                await SaveStoreMappings(currency, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Currencies.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = currency.Id });
            }

            //prepare model
            model = await _currencyModelFactory.PrepareCurrencyModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //try to get a currency with the specified id
            var currency = await _currencyService.GetCurrencyById(id);
            if (currency == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _currencyModelFactory.PrepareCurrencyModel(null, currency);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CurrencyModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //try to get a currency with the specified id
            var currency = await _currencyService.GetCurrencyById(model.Id);
            if (currency == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published currency
                var allCurrencies = await _currencyService.GetAllCurrencies();
                if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id && !model.Published)
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                    return RedirectToAction("Edit", new { id = currency.Id });
                }

                currency = model.ToEntity(currency);
                currency.UpdatedOnUtc = DateTime.UtcNow;
                await _currencyService.UpdateCurrency(currency);

                //activity log
                await _customerActivityService.InsertActivity("EditCurrency",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditCurrency"), currency.Id), currency);

                //locales
                await UpdateLocales(currency, model);

                //stores
                await SaveStoreMappings(currency, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Currencies.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = currency.Id });
            }

            //prepare model
            model = await _currencyModelFactory.PrepareCurrencyModel(model, currency, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //try to get a currency with the specified id
            var currency = await _currencyService.GetCurrencyById(id);
            if (currency == null)
                return RedirectToAction("List");

            try
            {
                if (currency.Id == _currencySettings.PrimaryStoreCurrencyId)
                    throw new NopException(await _localizationService.GetResource("Admin.Configuration.Currencies.CantDeletePrimary"));

                if (currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId)
                    throw new NopException(await _localizationService.GetResource("Admin.Configuration.Currencies.CantDeleteExchange"));

                //ensure we have at least one published currency
                var allCurrencies = await _currencyService.GetAllCurrencies();
                if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id)
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                    return RedirectToAction("Edit", new { id = currency.Id });
                }

                await _currencyService.DeleteCurrency(currency);

                //activity log
                await _customerActivityService.InsertActivity("DeleteCurrency",
                    string.Format(await _localizationService.GetResource("ActivityLog.DeleteCurrency"), currency.Id), currency);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Currencies.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = currency.Id });
            }
        }

        #endregion
    }
}