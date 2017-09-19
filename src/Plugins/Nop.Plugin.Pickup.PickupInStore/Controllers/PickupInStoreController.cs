using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Nop.Plugin.Pickup.PickupInStore.Models;
using Nop.Plugin.Pickup.PickupInStore.Services;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Pickup.PickupInStore.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    public class PickupInStoreController : BasePluginController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStorePickupPointService _storePickupPointService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public PickupInStoreController(IAddressService addressService,
            ICountryService countryService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStateProvinceService stateProvinceService,
            IStorePickupPointService storePickupPointService,
            IStoreService storeService)
        {
            this._addressService = addressService;
            this._countryService = countryService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._stateProvinceService = stateProvinceService;
            this._storePickupPointService = storePickupPointService;
            this._storeService = storeService;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            return View("~/Plugins/Pickup.PickupInStore/Views/Configure.cshtml");
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedKendoGridJson();

            var pickupPoints = _storePickupPointService.GetAllStorePickupPoints(pageIndex: command.Page - 1, pageSize: command.PageSize);
            var model = pickupPoints.Select(point =>
            {
                var store = _storeService.GetStoreById(point.StoreId);
                return new StorePickupPointModel
                {
                    Id = point.Id,
                    Name = point.Name,
                    OpeningHours = point.OpeningHours,
                    PickupFee = point.PickupFee,
                    DisplayOrder = point.DisplayOrder,
                    StoreName = store?.Name ?? (point.StoreId == 0 ? _localizationService.GetResource("Admin.Configuration.Settings.StoreScope.AllStores") : string.Empty)
                };
            }).ToList();

            return Json(new DataSourceResult
            {
                Data = model,
                Total = pickupPoints.TotalCount
            });
        }

        public IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new StorePickupPointModel();

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
        [AdminAntiForgery]
        public IActionResult Create(StorePickupPointModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var address = new Address
            {
                Address1 = model.Address.Address1,
                City = model.Address.City,
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
                StoreId = model.StoreId
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
                StoreId = pickupPoint.StoreId
            };

            var address = _addressService.GetAddressById(pickupPoint.AddressId);
            if (address != null)
            {
                model.Address = new AddressModel
                {
                    Address1 = address.Address1,
                    City = address.City,
                    CountryId = address.CountryId,
                    StateProvinceId = address.StateProvinceId,
                    ZipPostalCode = address.ZipPostalCode
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
        [AdminAntiForgery]
        public IActionResult Edit(StorePickupPointModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var pickupPoint = _storePickupPointService.GetStorePickupPointById(model.Id);
            if (pickupPoint == null)
                return RedirectToAction("Configure");

            var address = _addressService.GetAddressById(pickupPoint.AddressId) ?? new Address { CreatedOnUtc = DateTime.UtcNow };
            address.Address1 = model.Address.Address1;
            address.City = model.Address.City;
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
            _storePickupPointService.UpdateStorePickupPoint(pickupPoint);

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Pickup.PickupInStore/Views/Edit.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
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
