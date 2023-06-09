using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Models;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class FixedByWeightByTotalController : BasePluginController
    {
        #region Fields

        protected readonly CurrencySettings _currencySettings;
        protected readonly FixedByWeightByTotalSettings _fixedByWeightByTotalSettings;
        protected readonly ICountryService _countryService;
        protected readonly ICurrencyService _currencyService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IMeasureService _measureService;
        protected readonly IPermissionService _permissionService;
        protected readonly ISettingService _settingService;
        protected readonly IShippingByWeightByTotalService _shippingByWeightService;
        protected readonly IShippingService _shippingService;
        protected readonly IStateProvinceService _stateProvinceService;
        protected readonly IStoreService _storeService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IWorkContext _workContext;
        protected readonly MeasureSettings _measureSettings;

        #endregion

        #region Ctor

        public FixedByWeightByTotalController(CurrencySettings currencySettings,
            FixedByWeightByTotalSettings fixedByWeightByTotalSettings,
            ICountryService countryService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IPermissionService permissionService,
            ISettingService settingService,
            IShippingByWeightByTotalService shippingByWeightService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IStoreService storeService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            MeasureSettings measureSettings)
        {
            _currencySettings = currencySettings;
            _fixedByWeightByTotalSettings = fixedByWeightByTotalSettings;
            _countryService = countryService;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _measureService = measureService;
            _permissionService = permissionService;
            _settingService = settingService;
            _shippingByWeightService = shippingByWeightService;
            _stateProvinceService = stateProvinceService;
            _shippingService = shippingService;
            _storeService = storeService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _measureSettings = measureSettings;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure(bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                LimitMethodsToCreated = _fixedByWeightByTotalSettings.LimitMethodsToCreated,
                ShippingByWeightByTotalEnabled = _fixedByWeightByTotalSettings.ShippingByWeightByTotalEnabled
            };

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in await _storeService.GetAllStoresAsync())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });
            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var warehouses in await _shippingService.GetAllWarehousesAsync())
                model.AvailableWarehouses.Add(new SelectListItem { Text = warehouses.Name, Value = warehouses.Id.ToString() });
            //shipping methods
            foreach (var sm in await _shippingService.GetAllShippingMethodsAsync())
                model.AvailableShippingMethods.Add(new SelectListItem { Text = sm.Name, Value = sm.Id.ToString() });
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            var countries = await _countryService.GetAllCountriesAsync();
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });

            model.SetGridPageSize();

            //show configuration tour
            if (showtour)
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View("~/Plugins/Shipping.FixedByWeightByTotal/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            //save settings
            _fixedByWeightByTotalSettings.LimitMethodsToCreated = model.LimitMethodsToCreated;
            await _settingService.SaveSettingAsync(_fixedByWeightByTotalSettings);

            return Json(new { Result = true });
        }

        [HttpPost]
        public async Task<IActionResult> SaveMode(bool value)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            //save settings
            _fixedByWeightByTotalSettings.ShippingByWeightByTotalEnabled = value;
            await _settingService.SaveSettingAsync(_fixedByWeightByTotalSettings);

            return Json(new { Result = true });
        }

        #region Fixed rate

        [HttpPost]
        public async Task<IActionResult> FixedShippingRateList(ConfigurationModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            var shippingMethods = (await _shippingService.GetAllShippingMethodsAsync()).ToPagedList(searchModel);

            var gridModel = await new FixedRateListModel().PrepareToGridAsync(searchModel, shippingMethods, () =>
            {
                return shippingMethods.SelectAwait(async shippingMethod => new FixedRateModel
                {
                    ShippingMethodId = shippingMethod.Id,
                    ShippingMethodName = shippingMethod.Name,

                    Rate = await _settingService
                        .GetSettingByKeyAsync<decimal>(string.Format(FixedByWeightByTotalDefaults.FixedRateSettingsKey, shippingMethod.Id)),
                    TransitDays = await _settingService
                        .GetSettingByKeyAsync<int?>(string.Format(FixedByWeightByTotalDefaults.TransitDaysSettingsKey, shippingMethod.Id))
                });
            });

            return Json(gridModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFixedShippingRate(FixedRateModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            await _settingService.SetSettingAsync(string.Format(FixedByWeightByTotalDefaults.FixedRateSettingsKey, model.ShippingMethodId), model.Rate, 0, false);
            await _settingService.SetSettingAsync(string.Format(FixedByWeightByTotalDefaults.TransitDaysSettingsKey, model.ShippingMethodId), model.TransitDays, 0, false);

            await _settingService.ClearCacheAsync();

            return new NullJsonResult();
        }

        #endregion

        #region Rate by weight

        [HttpPost]
        public async Task<IActionResult> RateByWeightByTotalList(ConfigurationModel searchModel, ConfigurationModel filter)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            //var records = _shippingByWeightService.GetAll(command.Page - 1, command.PageSize);
            var records = await _shippingByWeightService.FindRecordsAsync(
              pageIndex: searchModel.Page - 1,
              pageSize: searchModel.PageSize,
              storeId: filter.SearchStoreId,
              warehouseId: filter.SearchWarehouseId,
              countryId: filter.SearchCountryId,
              stateProvinceId: filter.SearchStateProvinceId,
              zip: filter.SearchZip,
              shippingMethodId: filter.SearchShippingMethodId,
              weight: null,
              orderSubtotal: null
              );

            var gridModel = await new ShippingByWeightByTotalListModel().PrepareToGridAsync(searchModel, records, () =>
            {
                return records.SelectAwait(async record =>
                {
                    var model = new ShippingByWeightByTotalModel
                    {
                        Id = record.Id,
                        StoreId = record.StoreId,
                        StoreName = (await _storeService.GetStoreByIdAsync(record.StoreId))?.Name ?? "*",
                        WarehouseId = record.WarehouseId,
                        WarehouseName = (await _shippingService.GetWarehouseByIdAsync(record.WarehouseId))?.Name ?? "*",
                        ShippingMethodId = record.ShippingMethodId,
                        ShippingMethodName = (await _shippingService.GetShippingMethodByIdAsync(record.ShippingMethodId))?.Name ?? "Unavailable",
                        CountryId = record.CountryId,
                        CountryName = (await _countryService.GetCountryByIdAsync(record.CountryId))?.Name ?? "*",
                        StateProvinceId = record.StateProvinceId,
                        StateProvinceName = (await _stateProvinceService.GetStateProvinceByIdAsync(record.StateProvinceId))?.Name ?? "*",
                        WeightFrom = record.WeightFrom,
                        WeightTo = record.WeightTo,
                        OrderSubtotalFrom = record.OrderSubtotalFrom,
                        OrderSubtotalTo = record.OrderSubtotalTo,
                        AdditionalFixedCost = record.AdditionalFixedCost,
                        PercentageRateOfSubtotal = record.PercentageRateOfSubtotal,
                        RatePerWeightUnit = record.RatePerWeightUnit,
                        LowerWeightLimit = record.LowerWeightLimit,
                        Zip = !string.IsNullOrEmpty(record.Zip) ? record.Zip : "*"
                    };

                    var htmlSb = new StringBuilder("<div>");
                    htmlSb.AppendFormat("{0}: {1}",
                        await _localizationService.GetResourceAsync("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightFrom"),
                        model.WeightFrom);
                    htmlSb.Append("<br />");
                    htmlSb.AppendFormat("{0}: {1}",
                        await _localizationService.GetResourceAsync("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightTo"),
                        model.WeightTo);
                    htmlSb.Append("<br />");
                    htmlSb.AppendFormat("{0}: {1}",
                        await _localizationService.GetResourceAsync("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalFrom"),
                        model.OrderSubtotalFrom);
                    htmlSb.Append("<br />");
                    htmlSb.AppendFormat("{0}: {1}",
                        await _localizationService.GetResourceAsync("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalTo"),
                        model.OrderSubtotalTo);
                    htmlSb.Append("<br />");
                    htmlSb.AppendFormat("{0}: {1}",
                        await _localizationService.GetResourceAsync("Plugins.Shipping.FixedByWeightByTotal.Fields.AdditionalFixedCost"),
                        model.AdditionalFixedCost);
                    htmlSb.Append("<br />");
                    htmlSb.AppendFormat("{0}: {1}",
                        await _localizationService.GetResourceAsync("Plugins.Shipping.FixedByWeightByTotal.Fields.RatePerWeightUnit"),
                        model.RatePerWeightUnit);
                    htmlSb.Append("<br />");
                    htmlSb.AppendFormat("{0}: {1}",
                        await _localizationService.GetResourceAsync("Plugins.Shipping.FixedByWeightByTotal.Fields.LowerWeightLimit"),
                        model.LowerWeightLimit);
                    htmlSb.Append("<br />");
                    htmlSb.AppendFormat("{0}: {1}",
                        await _localizationService.GetResourceAsync("Plugins.Shipping.FixedByWeightByTotal.Fields.PercentageRateOfSubtotal"),
                        model.PercentageRateOfSubtotal);

                    htmlSb.Append("</div>");
                    model.DataHtml = htmlSb.ToString();

                    return model;
                });
            });

            return Json(gridModel);
        }

        public async Task<IActionResult> AddRateByWeightByTotalPopup()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ShippingByWeightByTotalModel
            {
                PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode,
                BaseWeightIn = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId))?.Name,
                WeightTo = 1000000,
                OrderSubtotalTo = 1000000
            };

            var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();
            if (!shippingMethods.Any())
                return Content("No shipping methods can be loaded");

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in await _storeService.GetAllStoresAsync())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });
            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var warehouses in await _shippingService.GetAllWarehousesAsync())
                model.AvailableWarehouses.Add(new SelectListItem { Text = warehouses.Name, Value = warehouses.Id.ToString() });
            //shipping methods
            foreach (var sm in shippingMethods)
                model.AvailableShippingMethods.Add(new SelectListItem { Text = sm.Name, Value = sm.Id.ToString() });
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });

            return View("~/Plugins/Shipping.FixedByWeightByTotal/Views/AddRateByWeightByTotalPopup.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddRateByWeightByTotalPopup(ShippingByWeightByTotalModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            await _shippingByWeightService.InsertShippingByWeightRecordAsync(new ShippingByWeightByTotalRecord
            {
                StoreId = model.StoreId,
                WarehouseId = model.WarehouseId,
                CountryId = model.CountryId,
                StateProvinceId = model.StateProvinceId,
                Zip = model.Zip == "*" ? null : model.Zip,
                ShippingMethodId = model.ShippingMethodId,
                WeightFrom = model.WeightFrom,
                WeightTo = model.WeightTo,
                OrderSubtotalFrom = model.OrderSubtotalFrom,
                OrderSubtotalTo = model.OrderSubtotalTo,
                AdditionalFixedCost = model.AdditionalFixedCost,
                RatePerWeightUnit = model.RatePerWeightUnit,
                PercentageRateOfSubtotal = model.PercentageRateOfSubtotal,
                LowerWeightLimit = model.LowerWeightLimit,
                TransitDays = model.TransitDays
            });

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Shipping.FixedByWeightByTotal/Views/AddRateByWeightByTotalPopup.cshtml", model);
        }

        public async Task<IActionResult> EditRateByWeightByTotalPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var sbw = await _shippingByWeightService.GetByIdAsync(id);
            if (sbw == null)
                //no record found with the specified id
                return RedirectToAction("Configure");

            var model = new ShippingByWeightByTotalModel
            {
                Id = sbw.Id,
                StoreId = sbw.StoreId,
                WarehouseId = sbw.WarehouseId,
                CountryId = sbw.CountryId,
                StateProvinceId = sbw.StateProvinceId,
                Zip = sbw.Zip,
                ShippingMethodId = sbw.ShippingMethodId,
                WeightFrom = sbw.WeightFrom,
                WeightTo = sbw.WeightTo,
                OrderSubtotalFrom = sbw.OrderSubtotalFrom,
                OrderSubtotalTo = sbw.OrderSubtotalTo,
                AdditionalFixedCost = sbw.AdditionalFixedCost,
                PercentageRateOfSubtotal = sbw.PercentageRateOfSubtotal,
                RatePerWeightUnit = sbw.RatePerWeightUnit,
                LowerWeightLimit = sbw.LowerWeightLimit,
                PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode,
                BaseWeightIn = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId))?.Name,
                TransitDays = sbw.TransitDays
            };

            var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();
            if (!shippingMethods.Any())
                return Content("No shipping methods can be loaded");

            var selectedStore = await _storeService.GetStoreByIdAsync(sbw.StoreId);
            var selectedWarehouse = await _shippingService.GetWarehouseByIdAsync(sbw.WarehouseId);
            var selectedShippingMethod = await _shippingService.GetShippingMethodByIdAsync(sbw.ShippingMethodId);
            var selectedCountry = await _countryService.GetCountryByIdAsync(sbw.CountryId);
            var selectedState = await _stateProvinceService.GetStateProvinceByIdAsync(sbw.StateProvinceId);
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in await _storeService.GetAllStoresAsync())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = (selectedStore != null && store.Id == selectedStore.Id) });
            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var warehouse in await _shippingService.GetAllWarehousesAsync())
                model.AvailableWarehouses.Add(new SelectListItem { Text = warehouse.Name, Value = warehouse.Id.ToString(), Selected = (selectedWarehouse != null && warehouse.Id == selectedWarehouse.Id) });
            //shipping methods
            foreach (var sm in shippingMethods)
                model.AvailableShippingMethods.Add(new SelectListItem { Text = sm.Name, Value = sm.Id.ToString(), Selected = (selectedShippingMethod != null && sm.Id == selectedShippingMethod.Id) });
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (selectedCountry != null && c.Id == selectedCountry.Id) });
            //states
            var states = selectedCountry != null ? (await _stateProvinceService.GetStateProvincesByCountryIdAsync(selectedCountry.Id, showHidden: true)).ToList() : new List<StateProvince>();
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var s in states)
                model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (selectedState != null && s.Id == selectedState.Id) });

            return View("~/Plugins/Shipping.FixedByWeightByTotal/Views/EditRateByWeightByTotalPopup.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRateByWeightByTotalPopup(ShippingByWeightByTotalModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var sbw = await _shippingByWeightService.GetByIdAsync(model.Id);
            if (sbw == null)
                //no record found with the specified id
                return RedirectToAction("Configure");

            sbw.StoreId = model.StoreId;
            sbw.WarehouseId = model.WarehouseId;
            sbw.CountryId = model.CountryId;
            sbw.StateProvinceId = model.StateProvinceId;
            sbw.Zip = model.Zip == "*" ? null : model.Zip;
            sbw.ShippingMethodId = model.ShippingMethodId;
            sbw.WeightFrom = model.WeightFrom;
            sbw.WeightTo = model.WeightTo;
            sbw.OrderSubtotalFrom = model.OrderSubtotalFrom;
            sbw.OrderSubtotalTo = model.OrderSubtotalTo;
            sbw.AdditionalFixedCost = model.AdditionalFixedCost;
            sbw.RatePerWeightUnit = model.RatePerWeightUnit;
            sbw.PercentageRateOfSubtotal = model.PercentageRateOfSubtotal;
            sbw.LowerWeightLimit = model.LowerWeightLimit;
            sbw.TransitDays = model.TransitDays;

            await _shippingByWeightService.UpdateShippingByWeightRecordAsync(sbw);

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Shipping.FixedByWeightByTotal/Views/EditRateByWeightByTotalPopup.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRateByWeightByTotal(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var sbw = await _shippingByWeightService.GetByIdAsync(id);
            if (sbw != null)
                await _shippingByWeightService.DeleteShippingByWeightRecordAsync(sbw);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}