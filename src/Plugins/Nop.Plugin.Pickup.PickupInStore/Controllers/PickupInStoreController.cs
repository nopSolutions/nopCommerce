using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _storePickupPointModelFactory.PrepareStorePickupPointSearchModelAsync(new StorePickupPointSearchModel());

            return View("~/Plugins/Pickup.PickupInStore/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> List(StorePickupPointSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _storePickupPointModelFactory.PrepareStorePickupPointListModelAsync(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new StorePickupPointModel
            {
                Address = new AddressModel()
            };

            model.Address.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var country in await _countryService.GetAllCountriesAsync(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = country.Name, Value = country.Id.ToString() });

            var states = !model.Address.CountryId.HasValue ? new List<StateProvince>()
                : await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.Address.CountryId.Value, showHidden: true);
            if (states.Any())
            {
                model.Address.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.SelectState"), Value = "0" });
                foreach (var state in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = state.Name, Value = state.Id.ToString() });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.Other"), Value = "0" });

            model.AvailableStores.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.StoreScope.AllStores"), Value = "0" });
            foreach (var store in await _storeService.GetAllStoresAsync())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

            return View("~/Plugins/Pickup.PickupInStore/Views/Create.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StorePickupPointModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
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
            await _addressService.InsertAddressAsync(address);

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
            await _storePickupPointService.InsertStorePickupPointAsync(pickupPoint);

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Pickup.PickupInStore/Views/Create.cshtml", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var pickupPoint = await _storePickupPointService.GetStorePickupPointByIdAsync(id);
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

            var address = await _addressService.GetAddressByIdAsync(pickupPoint.AddressId);
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
                };
            }

            model.Address.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var country in await _countryService.GetAllCountriesAsync(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = country.Name, Value = country.Id.ToString(), Selected = (address != null && country.Id == address.CountryId) });

            var states = !model.Address.CountryId.HasValue ? new List<StateProvince>()
                : await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.Address.CountryId.Value, showHidden: true);
            if (states.Any())
            {
                model.Address.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.SelectState"), Value = "0" });
                foreach (var state in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = state.Name, Value = state.Id.ToString(), Selected = (address != null && state.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.Other"), Value = "0" });

            model.AvailableStores.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.StoreScope.AllStores"), Value = "0" });
            foreach (var store in await _storeService.GetAllStoresAsync())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = store.Id == model.StoreId });

            return View("~/Plugins/Pickup.PickupInStore/Views/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StorePickupPointModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Edit(model.Id);

            var pickupPoint = await _storePickupPointService.GetStorePickupPointByIdAsync(model.Id);
            if (pickupPoint == null)
                return RedirectToAction("Configure");

            var address = await _addressService.GetAddressByIdAsync(pickupPoint.AddressId) ?? new Address { CreatedOnUtc = DateTime.UtcNow };
            address.Address1 = model.Address.Address1;
            address.City = model.Address.City;
            address.County = model.Address.County;
            address.CountryId = model.Address.CountryId;
            address.StateProvinceId = model.Address.StateProvinceId;
            address.ZipPostalCode = model.Address.ZipPostalCode;
            if (address.Id > 0)
                await _addressService.UpdateAddressAsync(address);
            else
                await _addressService.InsertAddressAsync(address);

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
            await _storePickupPointService.UpdateStorePickupPointAsync(pickupPoint);

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Pickup.PickupInStore/Views/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var pickupPoint = await _storePickupPointService.GetStorePickupPointByIdAsync(id);
            if (pickupPoint == null)
                return RedirectToAction("Configure");

            var address = await _addressService.GetAddressByIdAsync(pickupPoint.AddressId);
            if (address != null)
                await _addressService.DeleteAddressAsync(address);

            await _storePickupPointService.DeleteStorePickupPointAsync(pickupPoint);

            return new NullJsonResult();
        }

        #endregion
    }
}
