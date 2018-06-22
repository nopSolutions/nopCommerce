using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
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
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            this._currencySettings = currencySettings;
            this._currencyModelFactory = currencyModelFactory;
            this._currencyService = currencyService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(Currency currency, CurrencyModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(currency, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        protected virtual void SaveStoreMappings(Currency currency, CurrencyModel model)
        {
            currency.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(currency);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(currency, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List(bool liveRates = false)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //prepare model
            var model = _currencyModelFactory.PrepareCurrencySearchModel(new CurrencySearchModel(), liveRates);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult List(CurrencySearchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            _currencySettings.ActiveExchangeRateProviderSystemName = model.ExchangeRateProviderModel.ExchangeRateProvider;
            _currencySettings.AutoUpdateEnabled = model.ExchangeRateProviderModel.AutoUpdateEnabled;
            _settingService.SaveSetting(_currencySettings);

            return RedirectToAction("List", "Currency");
        }

        [HttpPost]
        public virtual IActionResult ListGrid(CurrencySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _currencyModelFactory.PrepareCurrencyListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ApplyRates(IEnumerable<CurrencyExchangeRateModel> rateModels)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            foreach (var rate in rateModels)
            {
                var currency = _currencyService.GetCurrencyByCode(rate.CurrencyCode, false);
                if (currency == null)
                    continue;

                currency.Rate = rate.Rate;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.UpdateCurrency(currency);
            }

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual IActionResult MarkAsPrimaryExchangeRateCurrency(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            _currencySettings.PrimaryExchangeRateCurrencyId = id;
            _settingService.SaveSetting(_currencySettings);

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual IActionResult MarkAsPrimaryStoreCurrency(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            _currencySettings.PrimaryStoreCurrencyId = id;
            _settingService.SaveSetting(_currencySettings);

            return Json(new { result = true });
        }

        #endregion

        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //prepare model
            var model = _currencyModelFactory.PrepareCurrencyModel(new CurrencyModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CurrencyModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var currency = model.ToEntity<Currency>();
                currency.CreatedOnUtc = DateTime.UtcNow;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.InsertCurrency(currency);

                //activity log
                _customerActivityService.InsertActivity("AddNewCurrency",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCurrency"), currency.Id), currency);

                //locales
                UpdateLocales(currency, model);

                //stores
                SaveStoreMappings(currency, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = currency.Id });
            }

            //prepare model
            model = _currencyModelFactory.PrepareCurrencyModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //try to get a currency with the specified id
            var currency = _currencyService.GetCurrencyById(id, false);
            if (currency == null)
                return RedirectToAction("List");

            //prepare model
            var model = _currencyModelFactory.PrepareCurrencyModel(null, currency);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CurrencyModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //try to get a currency with the specified id
            var currency = _currencyService.GetCurrencyById(model.Id, false);
            if (currency == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published currency
                var allCurrencies = _currencyService.GetAllCurrencies();
                if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id && !model.Published)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                    return RedirectToAction("Edit", new { id = currency.Id });
                }

                currency = model.ToEntity(currency);
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.UpdateCurrency(currency);

                //activity log
                _customerActivityService.InsertActivity("EditCurrency",
                    string.Format(_localizationService.GetResource("ActivityLog.EditCurrency"), currency.Id), currency);

                //locales
                UpdateLocales(currency, model);

                //stores
                SaveStoreMappings(currency, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = currency.Id });
            }

            //prepare model
            model = _currencyModelFactory.PrepareCurrencyModel(model, currency, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            //try to get a currency with the specified id
            var currency = _currencyService.GetCurrencyById(id, false);
            if (currency == null)
                return RedirectToAction("List");

            try
            {
                if (currency.Id == _currencySettings.PrimaryStoreCurrencyId)
                    throw new NopException(_localizationService.GetResource("Admin.Configuration.Currencies.CantDeletePrimary"));

                if (currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId)
                    throw new NopException(_localizationService.GetResource("Admin.Configuration.Currencies.CantDeleteExchange"));

                //ensure we have at least one published currency
                var allCurrencies = _currencyService.GetAllCurrencies(loadCacheableCopy: false);
                if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                    return RedirectToAction("Edit", new { id = currency.Id });
                }

                _currencyService.DeleteCurrency(currency);

                //activity log
                _customerActivityService.InsertActivity("DeleteCurrency",
                    string.Format(_localizationService.GetResource("ActivityLog.DeleteCurrency"), currency.Id), currency);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = currency.Id });
            }
        }

        #endregion
    }
}