using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Models;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class FixedOrByCountryStateZipController : BasePluginController
{
    #region Fields

    protected readonly FixedOrByCountryStateZipTaxSettings _countryStateZipSettings;
    protected readonly ICountryService _countryService;
    protected readonly ICountryStateZipService _taxRateService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreService _storeService;
    protected readonly ITaxCategoryService _taxCategoryService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public FixedOrByCountryStateZipController(FixedOrByCountryStateZipTaxSettings countryStateZipSettings,
        ICountryService countryService,
        ICountryStateZipService taxRateService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStateProvinceService stateProvinceService,
        IStoreService storeService,
        ITaxCategoryService taxCategoryService,
        IGenericAttributeService genericAttributeService,
        IWorkContext workContext)

    {
        _countryStateZipSettings = countryStateZipSettings;
        _countryService = countryService;
        _taxRateService = taxRateService;
        _permissionService = permissionService;
        _localizationService = localizationService;
        _settingService = settingService;
        _stateProvinceService = stateProvinceService;
        _storeService = storeService;
        _taxCategoryService = taxCategoryService;
        _genericAttributeService = genericAttributeService;
        _workContext = workContext;

    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> Configure(bool showtour = false)
    {
        var taxCategories = await _taxCategoryService.GetAllTaxCategoriesAsync();

        if (!taxCategories.Any())
        {
            var errorModel = new ConfigurationModel
            {
                TaxCategoriesCanNotLoadedError = string.Format(
                    await _localizationService.GetResourceAsync(
                        "Plugins.Tax.FixedOrByCountryStateZip.TaxCategoriesCanNotLoaded"),
                    Url.Action("Categories", "Tax"))
            };

            return View("~/Plugins/Tax.FixedOrByCountryStateZip/Views/Configure.cshtml", errorModel);
        }

        var model = new ConfigurationModel { CountryStateZipEnabled = _countryStateZipSettings.CountryStateZipEnabled };

        //stores
        model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
        var stores = await _storeService.GetAllStoresAsync();
        foreach (var s in stores)
            model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
        //tax categories
        foreach (var tc in taxCategories)
            model.AvailableTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString() });
        //countries
        var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
        foreach (var c in countries)
            model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
        //states
        model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });
        var defaultCountry = countries.FirstOrDefault();
        if (defaultCountry != null)
        {
            var states = await _stateProvinceService.GetStateProvincesByCountryIdAsync(defaultCountry.Id);
            foreach (var s in states)
                model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
        }

        //show configuration tour
        if (showtour)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
            var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

            if (!hideCard && !closeCard)
                ViewBag.ShowTour = true;
        }

        return View("~/Plugins/Tax.FixedOrByCountryStateZip/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> SaveMode(bool value)
    {
        //save settings
        _countryStateZipSettings.CountryStateZipEnabled = value;
        await _settingService.SaveSettingAsync(_countryStateZipSettings);

        return Json(new { Result = true });
    }

    #region Fixed tax

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> FixedRatesList(ConfigurationModel searchModel)
    {
        var categories = (await _taxCategoryService.GetAllTaxCategoriesAsync()).ToPagedList(searchModel);

        var gridModel = await new FixedTaxRateListModel().PrepareToGridAsync(searchModel, categories, () =>
        {
            return categories.SelectAwait(async taxCategory => new FixedTaxRateModel
            {
                TaxCategoryId = taxCategory.Id,
                TaxCategoryName = taxCategory.Name,

                Rate = await _settingService
                    .GetSettingByKeyAsync<decimal>(string.Format(FixedOrByCountryStateZipDefaults.FIXED_RATE_SETTINGS_KEY, taxCategory.Id))
            });
        });

        return Json(gridModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> FixedRateUpdate(FixedTaxRateModel model)
    {
        await _settingService.SetSettingAsync(string.Format(FixedOrByCountryStateZipDefaults.FIXED_RATE_SETTINGS_KEY, model.TaxCategoryId), model.Rate);

        return new NullJsonResult();
    }

    #endregion

    #region Tax by country/state/zip

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> RatesByCountryStateZipList(ConfigurationModel searchModel)
    {
        var records = await _taxRateService.GetAllTaxRatesAsync(searchModel.Page - 1, searchModel.PageSize);

        var gridModel = await new CountryStateZipListModel().PrepareToGridAsync(searchModel, records, () =>
        {
            return records.SelectAwait(async record => new CountryStateZipModel
            {
                Id = record.Id,
                StoreId = record.StoreId,
                StoreName = (await _storeService.GetStoreByIdAsync(record.StoreId))?.Name ?? "*",
                TaxCategoryId = record.TaxCategoryId,
                TaxCategoryName = (await _taxCategoryService.GetTaxCategoryByIdAsync(record.TaxCategoryId))?.Name ?? string.Empty,
                CountryId = record.CountryId,
                CountryName = (await _countryService.GetCountryByIdAsync(record.CountryId))?.Name ?? "Unavailable",
                StateProvinceId = record.StateProvinceId,
                StateProvinceName = (await _stateProvinceService.GetStateProvinceByIdAsync(record.StateProvinceId))?.Name ?? "*",

                Zip = !string.IsNullOrEmpty(record.Zip) ? record.Zip : "*",
                Percentage = record.Percentage
            });
        });

        return Json(gridModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> AddRateByCountryStateZip(ConfigurationModel model)
    {
        await _taxRateService.InsertTaxRateAsync(new TaxRate
        {
            StoreId = model.AddStoreId,
            TaxCategoryId = model.AddTaxCategoryId,
            CountryId = model.AddCountryId,
            StateProvinceId = model.AddStateProvinceId,
            Zip = model.AddZip,
            Percentage = model.AddPercentage
        });

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> UpdateRateByCountryStateZip(CountryStateZipModel model)
    {
        var taxRate = await _taxRateService.GetTaxRateByIdAsync(model.Id);
        taxRate.Zip = model.Zip == "*" ? null : model.Zip;
        taxRate.Percentage = model.Percentage;
        await _taxRateService.UpdateTaxRateAsync(taxRate);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> DeleteRateByCountryStateZip(int id)
    {
        var taxRate = await _taxRateService.GetTaxRateByIdAsync(id);
        if (taxRate != null)
            await _taxRateService.DeleteTaxRateAsync(taxRate);

        return new NullJsonResult();
    }

    #endregion

    #endregion
}