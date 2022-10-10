using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Tax.AbcTax.Domain;
using Nop.Plugin.Tax.AbcTax.Models;
using Nop.Plugin.Tax.AbcTax.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Tax.AbcTax.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class AbcTaxController : BasePluginController
    {
        private readonly AbcTaxSettings _abcTaxSettings;
        private readonly ICountryService _countryService;
        private readonly IAbcTaxService _abcTaxService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreService _storeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        public AbcTaxController(AbcTaxSettings abcTaxSettings,
            ICountryService countryService,
            IAbcTaxService abcTaxService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStateProvinceService stateProvinceService,
            IStoreService storeService,
            ITaxCategoryService taxCategoryService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext)

        {
            _abcTaxSettings = abcTaxSettings;
            _countryService = countryService;
            _abcTaxService = abcTaxService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _settingService = settingService;
            _stateProvinceService = stateProvinceService;
            _storeService = storeService;
            _taxCategoryService = taxCategoryService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;

        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> Configure(bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            var taxCategories = await _taxCategoryService.GetAllTaxCategoriesAsync();

            if (!taxCategories.Any())
            {
                var errorModel = new ConfigurationModel
                {
                    TaxCategoriesCanNotLoadedError = string.Format(
                        await _localizationService.GetResourceAsync(
                            "Plugins.Tax.AbcTax.TaxCategoriesCanNotLoaded"),
                        Url.Action("Categories", "Tax"))
                };

                return View("~/Plugins/Tax.AbcTax/Views/Configure.cshtml", errorModel);
            }

            var model = new ConfigurationModel();
            
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
                var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.HideConfigurationStepsAttribute);

                var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View("~/Plugins/Tax.AbcTax/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return await Configure();
            }

            _abcTaxSettings.TaxJarAPIToken = model.TaxJarAPIToken;
            _abcTaxSettings.IsDebugMode = model.IsDebugMode;
            await _settingService.SaveSettingAsync(_abcTaxSettings);

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved")
            );

            return await Configure();
        }

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> RatesByCountryStateZipList(ConfigurationModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return await AccessDeniedDataTablesJson();

            var records = await _abcTaxService.GetAllTaxRatesAsync(searchModel.Page - 1, searchModel.PageSize);

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
                    Percentage = record.Percentage,
                    IsTaxJarEnabled = record.IsTaxJarEnabled
                });
            });

            return Json(gridModel);
        }

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> AddRateByCountryStateZip(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            await _abcTaxService.InsertTaxRateAsync(new AbcTaxRate
            {
                StoreId = model.AddStoreId,
                TaxCategoryId = model.AddTaxCategoryId,
                CountryId = model.AddCountryId,
                StateProvinceId = model.AddStateProvinceId,
                Zip = model.AddZip,
                Percentage = model.AddPercentage,
                IsTaxJarEnabled = model.IsTaxJarEnabled
            });

            return Json(new { Result = true });
        }

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> UpdateRateByCountryStateZip(CountryStateZipModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = await _abcTaxService.GetTaxRateByIdAsync(model.Id);
            taxRate.Zip = model.Zip == "*" ? null : model.Zip;
            taxRate.Percentage = model.Percentage;
            await _abcTaxService.UpdateTaxRateAsync(taxRate);

            return new NullJsonResult();
        }

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> DeleteRateByCountryStateZip(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = await _abcTaxService.GetTaxRateByIdAsync(id);
            if (taxRate != null)
                await _abcTaxService.DeleteTaxRateAsync(taxRate);

            return new NullJsonResult();
        }
    }
}