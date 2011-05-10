using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Shipping;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class ShippingController : BaseNopController
	{
		#region Fields

        private readonly IShippingService _shippingService;
        private ShippingSettings _shippingSettings;
        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;

		#endregion Fields 

		#region Constructors

        public ShippingController(IShippingService shippingService, ShippingSettings shippingSettings,
            ISettingService settingService, ICountryService countryService, 
            IStateProvinceService stateProvinceService, IAddressService addressService)
		{
            this._shippingService = shippingService;
            this._shippingSettings = shippingSettings;
            this._settingService = settingService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
		}

		#endregion Constructors 

        #region Shipping rate computation methods

        public ActionResult Providers()
        {
            var shippingProvidersModel = new List<ShippingRateComputationMethodModel>();
            var shippingProviders = _shippingService.LoadAllShippingRateComputationMethods();
            foreach (var shippingProvider in shippingProviders)
            {
                var tmp1 = shippingProvider.ToModel();
                tmp1.IsActive = shippingProvider.IsShippingRateComputationMethodActive(_shippingSettings);
                shippingProvidersModel.Add(tmp1);
            }
            var gridModel = new GridModel<ShippingRateComputationMethodModel>
            {
                Data = shippingProvidersModel,
                Total = shippingProvidersModel.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Providers(GridCommand command)
        {
            var shippingProvidersModel = new List<ShippingRateComputationMethodModel>();
            var shippingProviders = _shippingService.LoadAllShippingRateComputationMethods();
            foreach (var shippingProvider in shippingProviders)
            {
                var tmp1 = shippingProvider.ToModel();
                tmp1.IsActive = shippingProvider.IsShippingRateComputationMethodActive(_shippingSettings);
                shippingProvidersModel.Add(tmp1);
            }
            shippingProvidersModel = shippingProvidersModel.ForCommand(command).ToList();
            var gridModel = new GridModel<ShippingRateComputationMethodModel>
            {
                Data = shippingProvidersModel,
                Total = shippingProvidersModel.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProviderUpdate(ShippingRateComputationMethodModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Providers");
            }

            //TODO an issue: when a store owner clicks on 'Update' button and then 'Cancel' button,
            //the 'Configure provider' hyperlink disappers
            
            //UNDONE allow store owner to edit display order of shipping rate computation methods

            var srcm = _shippingService.LoadShippingRateComputationMethodBySystemName(model.SystemName);
            if (srcm.IsShippingRateComputationMethodActive(_shippingSettings))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(srcm.SystemName);
                    _settingService.SaveSetting(_shippingSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(srcm.SystemName);
                    _settingService.SaveSetting(_shippingSettings);
                }
            }


            return Providers(command);
        }

        public ActionResult ConfigureProvider(string systemName)
        {
            var srcm = _shippingService.LoadShippingRateComputationMethodBySystemName(systemName);
            if (srcm == null) throw new ArgumentException("No shipping rate computation method found with the specified system name", "systemName");

            var model = srcm.ToModel();
            string actionName, controllerName;
            RouteValueDictionary routeValues;
            srcm.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;
            return View(model);
        }

        #endregion
        
        #region Shipping methods

        public ActionResult Methods()
        {
            var shippingMethodsModel = _shippingService.GetAllShippingMethods()
                .Select(x => x.ToModel())
                .ToList();
            var model = new GridModel<ShippingMethodModel>
            {
                Data = shippingMethodsModel,
                Total = shippingMethodsModel.Count
            };
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Methods(GridCommand command)
        {
            var shippingMethodsModel = _shippingService.GetAllShippingMethods()
                .Select(x => x.ToModel())
                .ForCommand(command)
                .ToList();
            var model = new GridModel<ShippingMethodModel>
            {
                Data = shippingMethodsModel,
                Total = shippingMethodsModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult MethodUpdate(ShippingMethodModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Methods");
            }

            var shippingMethod = _shippingService.GetShippingMethodById(model.Id);
            shippingMethod = model.ToEntity(shippingMethod);
            _shippingService.UpdateShippingMethod(shippingMethod);

            return Methods(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult MethodAdd([Bind(Exclude = "Id")] ShippingMethodModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult { Data = "error" };
            }

            var shippingMethod = new ShippingMethod();
            shippingMethod = model.ToEntity(shippingMethod);
            _shippingService.InsertShippingMethod(shippingMethod);

            return Methods(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult MethodDelete(int id, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult { Data = "error" };
            }

            var shippingMethod = _shippingService.GetShippingMethodById(id);
            _shippingService.DeleteShippingMethod(shippingMethod);

            return Methods(command);
        }

        #endregion
        
        #region Shipping settings

        public ActionResult Settings()
        {
            var model = _shippingSettings.ToModel();

            //shipping origin
            var originAddress = _shippingSettings.ShippingOriginAddressId > 0
                                     ? _addressService.GetAddressById(_shippingSettings.ShippingOriginAddressId)
                                     : null;
            if (originAddress != null)
                model.ShippingOriginAddress = originAddress.ToModel();
            else
                model.ShippingOriginAddress = new AddressModel();

            model.ShippingOriginAddress.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.ShippingOriginAddress.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (originAddress != null && c.Id == originAddress.CountryId) });

            var states = originAddress != null && originAddress.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(originAddress.Country.Id).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.ShippingOriginAddress.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == originAddress.StateProvinceId) });
            }
            else
                model.ShippingOriginAddress.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });
            model.ShippingOriginAddress.FirstNameDisabled = true;
            model.ShippingOriginAddress.LastNameDisabled = true;
            model.ShippingOriginAddress.EmailDisabled = true;
            model.ShippingOriginAddress.CompanyDisabled = true;
            model.ShippingOriginAddress.CityDisabled = true;
            model.ShippingOriginAddress.Address1Disabled = true;
            model.ShippingOriginAddress.Address2Disabled = true;
            model.ShippingOriginAddress.PhoneNumberDisabled = true;
            model.ShippingOriginAddress.FaxNumberDisabled = true;

            return View(model);
        }

        [HttpPost]
        public ActionResult Settings(ShippingSettingsModel model)
        {
            _shippingSettings = model.ToEntity(_shippingSettings);

            var originAddress = _addressService.GetAddressById(_shippingSettings.ShippingOriginAddressId) ??
                                         new Core.Domain.Common.Address()
                                         {
                                             CreatedOnUtc = DateTime.UtcNow,
                                         };
            originAddress = model.ShippingOriginAddress.ToEntity(originAddress);
            if (originAddress.Id > 0)
                _addressService.UpdateAddress(originAddress);
            else
                _addressService.InsertAddress(originAddress);

            _shippingSettings.ShippingOriginAddressId = originAddress.Id;
            _settingService.SaveSetting(_shippingSettings);

            return RedirectToAction("Settings");
        }

        #endregion
    }
}
