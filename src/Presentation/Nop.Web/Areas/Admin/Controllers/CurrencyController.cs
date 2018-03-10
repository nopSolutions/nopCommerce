using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CurrencyController :  BaseAdminController
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ISettingService _settingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Ctor

        public CurrencyController(ICurrencyService currencyService, 
            CurrencySettings currencySettings, 
            ISettingService settingService,
            IDateTimeHelper dateTimeHelper, 
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ILocalizedEntityService localizedEntityService, 
            ILanguageService languageService,
            IStoreService storeService, 
            IStoreMappingService storeMappingService,
            ICustomerActivityService customerActivityService)
        {
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._settingService = settingService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._customerActivityService = customerActivityService;
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

        protected virtual void PrepareStoresMappingModel(CurrencyModel model, Currency currency, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && currency != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(currency).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
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

            var model = new CurrencyListModel
            {
                AutoUpdateEnabled = _currencySettings.AutoUpdateEnabled,
                ExchangeRateProviders = _currencyService.LoadAllExchangeRateProviders().Select(erp => new SelectListItem
                {
                    Text = erp.PluginDescriptor.FriendlyName,
                    Value = erp.PluginDescriptor.SystemName,
                    Selected = erp.PluginDescriptor.SystemName.Equals(_currencySettings.ActiveExchangeRateProviderSystemName, StringComparison.InvariantCultureIgnoreCase)
                }).ToList(),
            };

            if (liveRates)
            {
                try
                {
                    var primaryExchangeCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId, false);
                    if (primaryExchangeCurrency == null)
                        throw new NopException("Primary exchange rate currency is not set");
                    
                    //filter by existing currencies
                    var currencies = _currencyService.GetAllCurrencies(true, loadCacheableCopy: false);
                    model.ExchangeRates = _currencyService.GetCurrencyLiveRates(primaryExchangeCurrency.CurrencyCode)
                        .Where(rate => currencies.Any(currency => currency.CurrencyCode.Equals(rate.CurrencyCode, StringComparison.InvariantCultureIgnoreCase)))
                        .Select(rate => new CurrencyListModel.ExchangeRateModel { CurrencyCode = rate.CurrencyCode, Rate = rate.Rate }).ToList();
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc, false);
                }
            }

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult List(CurrencyListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            _currencySettings.ActiveExchangeRateProviderSystemName = model.ExchangeRateProvider;
            _currencySettings.AutoUpdateEnabled = model.AutoUpdateEnabled;
            _settingService.SaveSetting(_currencySettings);

            return RedirectToAction("List", "Currency");
        }

        [HttpPost]
        public virtual IActionResult ListGrid(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedKendoGridJson();

            var currenciesModel = _currencyService.GetAllCurrencies(true, loadCacheableCopy: false).Select(x => x.ToModel()).ToList();
            foreach (var currency in currenciesModel)
                currency.IsPrimaryExchangeRateCurrency = currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId;
            foreach (var currency in currenciesModel)
                currency.IsPrimaryStoreCurrency = currency.Id == _currencySettings.PrimaryStoreCurrencyId;

            var gridModel = new DataSourceResult
            {
                Data = currenciesModel,
                Total = currenciesModel.Count
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult ApplyAllRates(IEnumerable<CurrencyListModel.ExchangeRateModel> rates)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();
            
            foreach (var rate in rates)
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
        public virtual IActionResult ApplyRate(string currencyCode, decimal rate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var currency = _currencyService.GetCurrencyByCode(currencyCode, false);
            if (currency != null)
            {
                currency.Rate = rate;
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

            var model = new CurrencyModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //default values
            model.Published = true;
            model.Rate = 1;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CurrencyModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var currency = model.ToEntity();
                currency.CreatedOnUtc = DateTime.UtcNow;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.InsertCurrency(currency);

                //activity log
                _customerActivityService.InsertActivity("AddNewCurrency", _localizationService.GetResource("ActivityLog.AddNewCurrency"), currency.Id);

                //locales
                UpdateLocales(currency, model);
                //Stores
                SaveStoreMappings(currency, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Added"));

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = currency.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            PrepareStoresMappingModel(model, null, true);

            return View(model);
        }
        
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var currency = _currencyService.GetCurrencyById(id, false);
            if (currency == null)
                //No currency found with the specified id
                return RedirectToAction("List");

            var model = currency.ToModel();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(currency.CreatedOnUtc, DateTimeKind.Utc);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = currency.GetLocalized(x => x.Name, languageId, false, false);
            });
            //Stores
            PrepareStoresMappingModel(model, currency, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CurrencyModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var currency = _currencyService.GetCurrencyById(model.Id, false);
            if (currency == null)
                //No currency found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published currency
                var allCurrencies = _currencyService.GetAllCurrencies();
                if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id &&
                    !model.Published)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                    return RedirectToAction("Edit", new { id = currency.Id });
                }

                currency = model.ToEntity(currency);
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.UpdateCurrency(currency);

                //activity log
                _customerActivityService.InsertActivity("EditCurrency", _localizationService.GetResource("ActivityLog.EditCurrency"), currency.Id);

                //locales
                UpdateLocales(currency, model);
                //Stores
                SaveStoreMappings(currency, model);


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Updated"));

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new {id = currency.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(currency.CreatedOnUtc, DateTimeKind.Utc);

            //Stores
            PrepareStoresMappingModel(model, currency, true);

            return View(model);
        }
        
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var currency = _currencyService.GetCurrencyById(id, false);
            if (currency == null)
                //No currency found with the specified id
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
                _customerActivityService.InsertActivity("DeleteCurrency", _localizationService.GetResource("ActivityLog.DeleteCurrency"), currency.Id);

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