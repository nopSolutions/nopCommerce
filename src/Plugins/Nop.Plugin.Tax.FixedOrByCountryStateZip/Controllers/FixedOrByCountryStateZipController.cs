using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Models;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class FixedOrByCountryStateZipController : BasePluginController
    {
        #region Fields

        private readonly FixedOrByCountryStateZipTaxSettings _countryStateZipSettings;
        private readonly ICountryService _countryService;
        private readonly ICountryStateZipService _taxRateService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreService _storeService;
        private readonly ITaxCategoryService _taxCategoryService;

        #endregion

        #region Ctor

        public FixedOrByCountryStateZipController(FixedOrByCountryStateZipTaxSettings countryStateZipSettings,
            ICountryService countryService,
            ICountryStateZipService taxRateService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStateProvinceService stateProvinceService,
            IStoreService storeService,
            ITaxCategoryService taxCategoryService)
        {
            this._countryStateZipSettings = countryStateZipSettings;
            this._countryService = countryService;
            this._taxRateService = taxRateService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._stateProvinceService = stateProvinceService;
            this._storeService = storeService;
            this._taxCategoryService = taxCategoryService;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            if (!taxCategories.Any())
                return Content("No tax categories can be loaded");

            var model = new ConfigurationModel { CountryStateZipEnabled = _countryStateZipSettings.CountryStateZipEnabled };
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            var stores = _storeService.GetAllStores();
            foreach (var s in stores)
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            //tax categories
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString() });
            //countries
            var countries = _countryService.GetAllCountries(showHidden: true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });
            var defaultCountry = countries.FirstOrDefault();
            if (defaultCountry != null)
            {
                var states = _stateProvinceService.GetStateProvincesByCountryId(defaultCountry.Id);
                foreach (var s in states)
                    model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            }

            return View("~/Plugins/Tax.FixedOrByCountryStateZip/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveMode(bool value)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            //save settings
            _countryStateZipSettings.CountryStateZipEnabled = value;
            _settingService.SaveSetting(_countryStateZipSettings);

            return Json(new { Result = true });
        }

        #region Fixed tax

        [HttpPost]
        public IActionResult FixedRatesList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedKendoGridJson();

            var taxRateModels = _taxCategoryService.GetAllTaxCategories().Select(taxCategory => new FixedTaxRateModel
            {
                TaxCategoryId = taxCategory.Id,
                TaxCategoryName = taxCategory.Name,
                Rate = _settingService.GetSettingByKey<decimal>(string.Format(FixedOrByCountryStateZipDefaults.FixedRateSettingsKey, taxCategory.Id))
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = taxRateModels,
                Total = taxRateModels.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public IActionResult FixedRateUpdate(FixedTaxRateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");
            
            _settingService.SetSetting(string.Format(FixedOrByCountryStateZipDefaults.FixedRateSettingsKey, model.TaxCategoryId), model.Rate);

            return new NullJsonResult();
        }
        
        #endregion

        #region Tax by country/state/zip

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult RatesByCountryStateZipList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedKendoGridJson();

            var records = _taxRateService.GetAllTaxRates(command.Page - 1, command.PageSize);
            var taxRatesModel = records.Select(record => new CountryStateZipModel
            {
                Id = record.Id,
                StoreId = record.StoreId,
                StoreName = _storeService.GetStoreById(record.StoreId)?.Name ?? "*",
                TaxCategoryId = record.TaxCategoryId,
                TaxCategoryName = _taxCategoryService.GetTaxCategoryById(record.TaxCategoryId)?.Name ?? string.Empty,
                CountryId = record.CountryId,
                CountryName = _countryService.GetCountryById(record.CountryId)?.Name ?? "Unavailable",
                StateProvinceId = record.StateProvinceId,
                StateProvinceName = _stateProvinceService.GetStateProvinceById(record.StateProvinceId)?.Name ?? "*",
                Zip = !string.IsNullOrEmpty(record.Zip) ? record.Zip : "*",
                Percentage = record.Percentage,
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = taxRatesModel,
                Total = records.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult AddRateByCountryStateZip(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");
            
            _taxRateService.InsertTaxRate(new TaxRate
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
        [AdminAntiForgery]
        public IActionResult UpdateRateByCountryStateZip(CountryStateZipModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = _taxRateService.GetTaxRateById(model.Id);
            taxRate.Zip = model.Zip == "*" ? null : model.Zip;
            taxRate.Percentage = model.Percentage;
            _taxRateService.UpdateTaxRate(taxRate);

            return new NullJsonResult();
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult DeleteRateByCountryStateZip(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = _taxRateService.GetTaxRateById(id);
            if (taxRate != null)
                _taxRateService.DeleteTaxRate(taxRate);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}