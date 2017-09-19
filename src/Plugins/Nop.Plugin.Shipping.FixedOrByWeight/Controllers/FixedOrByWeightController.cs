using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Shipping.FixedOrByWeight.Domain;
using Nop.Plugin.Shipping.FixedOrByWeight.Models;
using Nop.Plugin.Shipping.FixedOrByWeight.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.FixedOrByWeight.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class FixedOrByWeightController : BasePluginController
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly FixedOrByWeightSettings _fixedOrByWeightSettings;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IShippingByWeightService _shippingByWeightService;
        private readonly IShippingService _shippingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreService _storeService;
        private readonly MeasureSettings _measureSettings;

        #endregion

        #region Ctor

        public FixedOrByWeightController(CurrencySettings currencySettings,
            FixedOrByWeightSettings fixedOrByWeightSettings,
            ICountryService countryService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IPermissionService permissionService,
            ISettingService settingService,
            IShippingByWeightService shippingByWeightService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IStoreService storeService,
            MeasureSettings measureSettings)
        {
            this._currencySettings = currencySettings;
            this._fixedOrByWeightSettings = fixedOrByWeightSettings;
            this._countryService = countryService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._measureService = measureService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._shippingByWeightService = shippingByWeightService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._storeService = storeService;
            this._measureSettings = measureSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                LimitMethodsToCreated = _fixedOrByWeightSettings.LimitMethodsToCreated,
                ShippingByWeightEnabled = _fixedOrByWeightSettings.ShippingByWeightEnabled
            };

            return View("~/Plugins/Shipping.FixedOrByWeight/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            //save settings
            _fixedOrByWeightSettings.LimitMethodsToCreated = model.LimitMethodsToCreated;
            _settingService.SaveSetting(_fixedOrByWeightSettings);

            return Json(new { Result = true });
        }

        [HttpPost]
        public IActionResult SaveMode(bool value)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            //save settings
            _fixedOrByWeightSettings.ShippingByWeightEnabled = value;
            _settingService.SaveSetting(_fixedOrByWeightSettings);

            return Json(new { Result = true });
        }

        #region Fixed rate

        [HttpPost]
        public IActionResult FixedShippingRateList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedKendoGridJson();

            var rateModels = _shippingService.GetAllShippingMethods().Select(shippingMethod => new FixedRateModel
            {
                ShippingMethodId = shippingMethod.Id,
                ShippingMethodName = shippingMethod.Name,
                Rate = _settingService.GetSettingByKey<decimal>(string.Format(FixedOrByWeightDefaults.FixedRateSettingsKey, shippingMethod.Id))
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = rateModels,
                Total = rateModels.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult UpdateFixedShippingRate(FixedRateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            _settingService.SetSetting(string.Format(FixedOrByWeightDefaults.FixedRateSettingsKey, model.ShippingMethodId), model.Rate);

            return new NullJsonResult();
        }

        #endregion

        #region Rate by weight

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult RateByWeightList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedKendoGridJson();

            var records = _shippingByWeightService.GetAll(command.Page - 1, command.PageSize);
            var sbwModel = records.Select(record =>
            {
                var model = new ShippingByWeightModel
                {
                    Id = record.Id,
                    StoreId = record.StoreId,
                    StoreName = _storeService.GetStoreById(record.StoreId)?.Name ?? "*",
                    WarehouseId = record.WarehouseId,
                    WarehouseName = _shippingService.GetWarehouseById(record.WarehouseId)?.Name ?? "*",
                    ShippingMethodId = record.ShippingMethodId,
                    ShippingMethodName = _shippingService.GetShippingMethodById(record.ShippingMethodId)?.Name ?? "Unavailable",
                    CountryId = record.CountryId,
                    CountryName = _countryService.GetCountryById(record.CountryId)?.Name ?? "*",
                    StateProvinceId = record.StateProvinceId,
                    StateProvinceName = _stateProvinceService.GetStateProvinceById(record.StateProvinceId)?.Name ?? "*",
                    From = record.From,
                    To = record.To,
                    AdditionalFixedCost = record.AdditionalFixedCost,
                    PercentageRateOfSubtotal = record.PercentageRateOfSubtotal,
                    RatePerWeightUnit = record.RatePerWeightUnit,
                    LowerWeightLimit = record.LowerWeightLimit,
                    Zip = !string.IsNullOrEmpty(record.Zip) ? record.Zip : "*"
                };                

                var htmlSb = new StringBuilder("<div>");
                htmlSb.AppendFormat("{0}: {1}", _localizationService.GetResource("Plugins.Shipping.FixedOrByWeight.Fields.From"), model.From);
                htmlSb.Append("<br />");
                htmlSb.AppendFormat("{0}: {1}", _localizationService.GetResource("Plugins.Shipping.FixedOrByWeight.Fields.To"), model.To);
                htmlSb.Append("<br />");
                htmlSb.AppendFormat("{0}: {1}", _localizationService.GetResource("Plugins.Shipping.FixedOrByWeight.Fields.AdditionalFixedCost"), model.AdditionalFixedCost);
                htmlSb.Append("<br />");
                htmlSb.AppendFormat("{0}: {1}", _localizationService.GetResource("Plugins.Shipping.FixedOrByWeight.Fields.RatePerWeightUnit"), model.RatePerWeightUnit);
                htmlSb.Append("<br />");
                htmlSb.AppendFormat("{0}: {1}", _localizationService.GetResource("Plugins.Shipping.FixedOrByWeight.Fields.LowerWeightLimit"), model.LowerWeightLimit);
                htmlSb.Append("<br />");
                htmlSb.AppendFormat("{0}: {1}", _localizationService.GetResource("Plugins.Shipping.FixedOrByWeight.Fields.PercentageRateOfSubtotal"), model.PercentageRateOfSubtotal);

                htmlSb.Append("</div>");
                model.DataHtml = htmlSb.ToString();

                return model;
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = sbwModel,
                Total = records.TotalCount
            };

            return Json(gridModel);
        }

        public IActionResult AddRateByWeighPopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ShippingByWeightModel
            {
                PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode,
                BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId)?.Name,
                To = 1000000
            };

            var shippingMethods = _shippingService.GetAllShippingMethods();
            if (!shippingMethods.Any())
                return Content("No shipping methods can be loaded");

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });
            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var warehouses in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = warehouses.Name, Value = warehouses.Id.ToString() });
            //shipping methods
            foreach (var sm in shippingMethods)
                model.AvailableShippingMethods.Add(new SelectListItem { Text = sm.Name, Value = sm.Id.ToString() });
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            var countries = _countryService.GetAllCountries(showHidden: true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });

            return View("~/Plugins/Shipping.FixedOrByWeight/Views/AddRateByWeightPopup.cshtml", model);
        }
        
        [HttpPost]
        [AdminAntiForgery]
        public IActionResult AddRateByWeighPopup(ShippingByWeightModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();
            
            _shippingByWeightService.InsertShippingByWeightRecord(new ShippingByWeightRecord
            {
                StoreId = model.StoreId,
                WarehouseId = model.WarehouseId,
                CountryId = model.CountryId,
                StateProvinceId = model.StateProvinceId,
                Zip = model.Zip == "*" ? null : model.Zip,
                ShippingMethodId = model.ShippingMethodId,
                From = model.From,
                To = model.To,
                AdditionalFixedCost = model.AdditionalFixedCost,
                RatePerWeightUnit = model.RatePerWeightUnit,
                PercentageRateOfSubtotal = model.PercentageRateOfSubtotal,
                LowerWeightLimit = model.LowerWeightLimit
            });

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Shipping.FixedOrByWeight/Views/AddRateByWeightPopup.cshtml", model);
        }
        
        public IActionResult EditRateByWeighPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var sbw = _shippingByWeightService.GetById(id);
            if (sbw == null)
                //no record found with the specified id
                return RedirectToAction("Configure");

            var model = new ShippingByWeightModel
            {
                Id = sbw.Id,
                StoreId = sbw.StoreId,
                WarehouseId = sbw.WarehouseId,
                CountryId = sbw.CountryId,
                StateProvinceId = sbw.StateProvinceId,
                Zip = sbw.Zip,
                ShippingMethodId = sbw.ShippingMethodId,
                From = sbw.From,
                To = sbw.To,
                AdditionalFixedCost = sbw.AdditionalFixedCost,
                PercentageRateOfSubtotal = sbw.PercentageRateOfSubtotal,
                RatePerWeightUnit = sbw.RatePerWeightUnit,
                LowerWeightLimit = sbw.LowerWeightLimit,
                PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode,
                BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId)?.Name
            };

            var shippingMethods = _shippingService.GetAllShippingMethods();
            if (!shippingMethods.Any())
                return Content("No shipping methods can be loaded");

            var selectedStore = _storeService.GetStoreById(sbw.StoreId);
            var selectedWarehouse = _shippingService.GetWarehouseById(sbw.WarehouseId);
            var selectedShippingMethod = _shippingService.GetShippingMethodById(sbw.ShippingMethodId);
            var selectedCountry = _countryService.GetCountryById(sbw.CountryId);
            var selectedState = _stateProvinceService.GetStateProvinceById(sbw.StateProvinceId);
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = (selectedStore != null && store.Id == selectedStore.Id) });
            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var warehouse in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = warehouse.Name, Value = warehouse.Id.ToString(), Selected = (selectedWarehouse != null && warehouse.Id == selectedWarehouse.Id) });
            //shipping methods
            foreach (var sm in shippingMethods)
                model.AvailableShippingMethods.Add(new SelectListItem { Text = sm.Name, Value = sm.Id.ToString(), Selected = (selectedShippingMethod != null && sm.Id == selectedShippingMethod.Id) });
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            var countries = _countryService.GetAllCountries(showHidden: true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (selectedCountry != null && c.Id == selectedCountry.Id) });
            //states
            var states = selectedCountry != null ? _stateProvinceService.GetStateProvincesByCountryId(selectedCountry.Id, showHidden: true).ToList() : new List<StateProvince>();
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var s in states)
                model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (selectedState != null && s.Id == selectedState.Id) });

            return View("~/Plugins/Shipping.FixedOrByWeight/Views/EditRateByWeightPopup.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult EditRateByWeighPopup(ShippingByWeightModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var sbw = _shippingByWeightService.GetById(model.Id);
            if (sbw == null)
                //no record found with the specified id
                return RedirectToAction("Configure");

            sbw.StoreId = model.StoreId;
            sbw.WarehouseId = model.WarehouseId;
            sbw.CountryId = model.CountryId;
            sbw.StateProvinceId = model.StateProvinceId;
            sbw.Zip = model.Zip == "*" ? null : model.Zip;
            sbw.ShippingMethodId = model.ShippingMethodId;
            sbw.From = model.From;
            sbw.To = model.To;
            sbw.AdditionalFixedCost = model.AdditionalFixedCost;
            sbw.RatePerWeightUnit = model.RatePerWeightUnit;
            sbw.PercentageRateOfSubtotal = model.PercentageRateOfSubtotal;
            sbw.LowerWeightLimit = model.LowerWeightLimit;

            _shippingByWeightService.UpdateShippingByWeightRecord(sbw);

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Shipping.FixedOrByWeight/Views/EditRateByWeightPopup.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult DeleteRateByWeigh(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var sbw = _shippingByWeightService.GetById(id);
            if (sbw != null)
                _shippingByWeightService.DeleteShippingByWeightRecord(sbw);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}