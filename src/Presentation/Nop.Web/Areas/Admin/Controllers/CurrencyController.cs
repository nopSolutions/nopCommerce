﻿using Microsoft.AspNetCore.Mvc;
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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CurrencyController : BaseAdminController
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyModelFactory _currencyModelFactory;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public CurrencyController(CurrencySettings currencySettings,
        ICurrencyModelFactory currencyModelFactory,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreMappingService storeMappingService)
    {
        _currencySettings = currencySettings;
        _currencyModelFactory = currencyModelFactory;
        _currencyService = currencyService;
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _settingService = settingService;
        _storeMappingService = storeMappingService;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Currency currency, CurrencyModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(currency, x => x.Name, localized.Name, localized.LanguageId);
        }
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> List(bool liveRates = false)
    {
        var model = new CurrencySearchModel();

        try
        {
            //prepare model
            model = await _currencyModelFactory.PrepareCurrencySearchModelAsync(model, liveRates);
        }
        catch (Exception e)
        {
            await _notificationService.ErrorNotificationAsync(e);
        }

        return View(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> List(CurrencySearchModel model)
    {
        _currencySettings.ActiveExchangeRateProviderSystemName = model.ExchangeRateProviderModel.ExchangeRateProvider;
        _currencySettings.AutoUpdateEnabled = model.ExchangeRateProviderModel.AutoUpdateEnabled;
        await _settingService.SaveSettingAsync(_currencySettings);

        return RedirectToAction("List", "Currency");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> ListGrid(CurrencySearchModel searchModel)
    {
        //prepare model
        var model = await _currencyModelFactory.PrepareCurrencyListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> ApplyRates(IEnumerable<CurrencyExchangeRateModel> rateModels)
    {
        foreach (var rate in rateModels)
        {
            var currency = await _currencyService.GetCurrencyByCodeAsync(rate.CurrencyCode);
            if (currency == null)
                continue;

            currency.Rate = rate.Rate;
            currency.UpdatedOnUtc = DateTime.UtcNow;
            await _currencyService.UpdateCurrencyAsync(currency);
        }

        return Json(new { result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> MarkAsPrimaryExchangeRateCurrency(int id)
    {
        _currencySettings.PrimaryExchangeRateCurrencyId = id;
        await _settingService.SaveSettingAsync(_currencySettings);

        return Json(new { result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> MarkAsPrimaryStoreCurrency(int id)
    {
        _currencySettings.PrimaryStoreCurrencyId = id;
        await _settingService.SaveSettingAsync(_currencySettings);

        return Json(new { result = true });
    }

    #endregion

    #region Create / Edit / Delete

    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _currencyModelFactory.PrepareCurrencyModelAsync(new CurrencyModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> Create(CurrencyModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var currency = model.ToEntity<Currency>();
            currency.CreatedOnUtc = DateTime.UtcNow;
            currency.UpdatedOnUtc = DateTime.UtcNow;
            await _currencyService.InsertCurrencyAsync(currency);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCurrency",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCurrency"), currency.Id), currency);

            //locales
            await UpdateLocalesAsync(currency, model);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(currency, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = currency.Id });
        }

        //prepare model
        model = await _currencyModelFactory.PrepareCurrencyModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a currency with the specified id
        var currency = await _currencyService.GetCurrencyByIdAsync(id);
        if (currency == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _currencyModelFactory.PrepareCurrencyModelAsync(null, currency);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> Edit(CurrencyModel model, bool continueEditing)
    {
        //try to get a currency with the specified id
        var currency = await _currencyService.GetCurrencyByIdAsync(model.Id);
        if (currency == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            //ensure we have at least one published currency
            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id && !model.Published)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                return RedirectToAction("Edit", new { id = currency.Id });
            }

            currency = model.ToEntity(currency);
            currency.UpdatedOnUtc = DateTime.UtcNow;
            await _currencyService.UpdateCurrencyAsync(currency);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditCurrency",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCurrency"), currency.Id), currency);

            //locales
            await UpdateLocalesAsync(currency, model);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(currency, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = currency.Id });
        }

        //prepare model
        model = await _currencyModelFactory.PrepareCurrencyModelAsync(model, currency, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a currency with the specified id
        var currency = await _currencyService.GetCurrencyByIdAsync(id);
        if (currency == null)
            return RedirectToAction("List");

        try
        {
            if (currency.Id == _currencySettings.PrimaryStoreCurrencyId)
                throw new NopException(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.CantDeletePrimary"));

            if (currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId)
                throw new NopException(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.CantDeleteExchange"));

            //ensure we have at least one published currency
            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                return RedirectToAction("Edit", new { id = currency.Id });
            }

            await _currencyService.DeleteCurrencyAsync(currency);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCurrency",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCurrency"), currency.Id), currency);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("Edit", new { id = currency.Id });
        }
    }

    #endregion
}