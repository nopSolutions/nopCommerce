using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Nop.Plugin.Pickup.PickupInStore.Factories;
using Nop.Plugin.Pickup.PickupInStore.Models;
using Nop.Plugin.Pickup.PickupInStore.Services;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Pickup.PickupInStore.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class PickupInStoreController : BasePluginController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStorePickupPointModelFactory _storePickupPointModelFactory;
        private readonly IStorePickupPointService _storePickupPointService;
        private readonly IStoreService _storeService;
        private readonly AddressSettings _addressSettings;

        #endregion

        #region Ctor

        public PickupInStoreController(IAddressService addressService,
            ICountryService countryService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStateProvinceService stateProvinceService,
            IStorePickupPointModelFactory storePickupPointModelFactory,
            IStorePickupPointService storePickupPointService,
            IStoreService storeService,
            AddressSettings customerSettings)
        {
            _addressService = addressService;
            _countryService = countryService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _stateProvinceService = stateProvinceService;
            _storePickupPointModelFactory = storePickupPointModelFactory;
            _storePickupPointService = storePickupPointService;
            _storeService = storeService;
            _addressSettings = customerSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _storePickupPointModelFactory.PrepareStorePickupPointSearchModel(new StorePickupPointSearchModel());

            return View("~/Plugins/Pickup.PickupInStore/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult List(StorePickupPointSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _storePickupPointModelFactory.PrepareStorePickupPointListModel(searchModel);

            return Json(model);
        }

        public IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new StorePickupPointModel
            {
                Address =
                {
                    CountryEnabled = _addressSettings.CountryEnabled,
                    StateProvinceEnabled = _addressSettings.StateProvinceEnabled,
                    ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled,
                    CityEnabled = _addressSettings.CityEnabled,
                    CountyEnabled = _addressSettings.CountyEnabled
                }
            };

            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var country in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = country.Name, Value = country.Id.ToString() });

            var states = !model.Address.CountryId.HasValue ? new List<StateProvince>()
                : _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, showHidden: true);
            if (states.Any())
            {
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectState"), Value = "0" });
                foreach (var state in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = state.Name, Value = state.Id.ToString() });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Configuration.Settings.StoreScope.AllStores"), Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

            return View("~/Plugins/Pickup.PickupInStore/Views/Create.cshtml", model);
        }

        [HttpPost]
        public IActionResult Create(StorePickupPointModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var address = new Address
            {
                Address1 = model.Address.Address1,
                City = model.Address.City,
                County = model.Address.County,
                CountryId = model.Address.CountryId,
                StateProvinceId = model.Address.StateProvinceId,
                ZipPostalCode = model.Address.ZipPostalCode,
                CreatedOnUtc = DateTime.UtcNow
            };
            _addressService.InsertAddress(address);

            var pickupPoint = new StorePickupPoint
            {
                Name = model.Name,
                Description = model.Description,
                AddressId = address.Id,
                OpeningHours = model.OpeningHours,
                PickupFee = model.PickupFee,
                DisplayOrder = model.DisplayOrder,
                StoreId = model.StoreId,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                TransitDays = model.TransitDays
            };
            _storePickupPointService.InsertStorePickupPoint(pickupPoint);

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Pickup.PickupInStore/Views/Create.cshtml", model);
        }

        public IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var pickupPoint = _storePickupPointService.GetStorePickupPointById(id);
            if (pickupPoint == null)
                return RedirectToAction("Configure");

            var model = new StorePickupPointModel
            {
                Id = pickupPoint.Id,
                Name = pickupPoint.Name,
                Description = pickupPoint.Description,
                OpeningHours = pickupPoint.OpeningHours,
                PickupFee = pickupPoint.PickupFee,
                DisplayOrder = pickupPoint.DisplayOrder,
                StoreId = pickupPoint.StoreId,
                Latitude = pickupPoint.Latitude,
                Longitude = pickupPoint.Longitude,
                TransitDays = pickupPoint.TransitDays
            };

            var address = _addressService.GetAddressById(pickupPoint.AddressId);
            if (address != null)
            {
                model.Address = new AddressModel
                {
                    Address1 = address.Address1,
                    City = address.City,
                    County = address.County,
                    CountryId = address.CountryId,
                    StateProvinceId = address.StateProvinceId,
                    ZipPostalCode = address.ZipPostalCode,
                    CountryEnabled = _addressSettings.CountryEnabled,
                    StateProvinceEnabled = _addressSettings.StateProvinceEnabled,
                    ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled,
                    CityEnabled = _addressSettings.CityEnabled,
                    CountyEnabled = _addressSettings.CountyEnabled
                };
            }

            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var country in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = country.Name, Value = country.Id.ToString(), Selected = (address != null && country.Id == address.CountryId) });

            var states = !model.Address.CountryId.HasValue ? new List<StateProvince>()
                : _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, showHidden: true);
            if (states.Any())
            {
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectState"), Value = "0" });
                foreach (var state in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = state.Name, Value = state.Id.ToString(), Selected = (address != null && state.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Configuration.Settings.StoreScope.AllStores"), Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = store.Id == model.StoreId });

            return View("~/Plugins/Pickup.PickupInStore/Views/Edit.cshtml", model);
        }

        [HttpPost]
        public IActionResult Edit(StorePickupPointModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Edit(model.Id);

            var pickupPoint = _storePickupPointService.GetStorePickupPointById(model.Id);
            if (pickupPoint == null)
                return RedirectToAction("Configure");

            var address = _addressService.GetAddressById(pickupPoint.AddressId) ?? new Address { CreatedOnUtc = DateTime.UtcNow };
            address.Address1 = model.Address.Address1;
            address.City = model.Address.City;
            address.County = model.Address.County;
            address.CountryId = model.Address.CountryId;
            address.StateProvinceId = model.Address.StateProvinceId;
            address.ZipPostalCode = model.Address.ZipPostalCode;
            if (address.Id > 0)
                _addressService.UpdateAddress(address);
            else
                _addressService.InsertAddress(address);

            pickupPoint.Name = model.Name;
            pickupPoint.Description = model.Description;
            pickupPoint.AddressId = address.Id;
            pickupPoint.OpeningHours = model.OpeningHours;
            pickupPoint.PickupFee = model.PickupFee;
            pickupPoint.DisplayOrder = model.DisplayOrder;
            pickupPoint.StoreId = model.StoreId;
            pickupPoint.Latitude = model.Latitude;
            pickupPoint.Longitude = model.Longitude;
            pickupPoint.TransitDays = model.TransitDays;
            _storePickupPointService.UpdateStorePickupPoint(pickupPoint);

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Pickup.PickupInStore/Views/Edit.cshtml", model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var pickupPoint = _storePickupPointService.GetStorePickupPointById(id);
            if (pickupPoint == null)
                return RedirectToAction("Configure");

            var address = _addressService.GetAddressById(pickupPoint.AddressId);
            if (address != null)
                _addressService.DeleteAddress(address);

            _storePickupPointService.DeleteStorePickupPoint(pickupPoint);

            return new NullJsonResult();
        }

        #endregion
    }
}
