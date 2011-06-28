using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models.Directory;
using Nop.Admin.Models.Shipping;
using Nop.Core.Domain.Shipping;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Nop.Services.Security;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class ShippingController : BaseNopController
	{
		#region Fields

        private readonly IShippingService _shippingService;
        private readonly ShippingSettings _shippingSettings;
        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

		#endregion

		#region Constructors

        public ShippingController(IShippingService shippingService, ShippingSettings shippingSettings,
            ISettingService settingService, ICountryService countryService,
            ILocalizationService localizationService, IPermissionService permissionService)
		{
            this._shippingService = shippingService;
            this._shippingSettings = shippingSettings;
            this._settingService = settingService;
            this._countryService = countryService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
		}

		#endregion 

        #region Shipping rate computation methods

        public ActionResult Providers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Providers");
            }

            var srcm = _shippingService.LoadShippingRateComputationMethodBySystemName(model.SystemName);
            if (srcm.IsShippingRateComputationMethodActive(_shippingSettings))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(srcm.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_shippingSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(srcm.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_shippingSettings);
                }
            }


            return Providers(command);
        }

        public ActionResult ConfigureProvider(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var srcm = _shippingService.LoadShippingRateComputationMethodBySystemName(systemName);
            if (srcm == null) 
                throw new ArgumentException("No shipping rate computation method found with the specified system name", "systemName");

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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var shippingMethod = _shippingService.GetShippingMethodById(id);
            _shippingService.DeleteShippingMethod(shippingMethod);

            return Methods(command);
        }

        #endregion
        
        #region Restrictions

        public ActionResult Restrictions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ShippingMethodRestrictionModel();

            var countries = _countryService.GetAllCountries(true);
            var shippingMethods = _shippingService.GetAllShippingMethods();
            foreach (var country in countries)
            {
                model.AvailableCountries.Add(new CountryModel()
                    {
                        Id = country.Id,
                        Name = country.Name
                    });
            }
            foreach (var sm in shippingMethods)
            {
                model.AvailableShippingMethods.Add(new ShippingMethodModel()
                {
                    Id = sm.Id,
                    Name = sm.Name
                });
            }
            foreach (var country in countries)
                foreach (var shippingMethod in shippingMethods)
                {
                    bool restricted = shippingMethod.CountryRestrictionExists(country.Id);
                    if (!model.Restricted.ContainsKey(country.Id))
                        model.Restricted[country.Id] = new Dictionary<int, bool>();
                    model.Restricted[country.Id][shippingMethod.Id] = restricted;
                }

            return View(model);
        }

        [HttpPost, ActionName("Restrictions")]
        public ActionResult RestrictionSave(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var countries = _countryService.GetAllCountries(true);
            var shippingMethods = _shippingService.GetAllShippingMethods();


            foreach (var shippingMethod in shippingMethods)
            {
                string formKey = "restrict_" + shippingMethod.Id;
                var countryIdsToRestrict = form[formKey] != null ? form[formKey].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

                foreach (var country in countries)
                {

                    bool restrict = countryIdsToRestrict.Contains(country.Id);
                    if (restrict)
                    {
                        if (shippingMethod.RestrictedCountries.Where(c => c.Id == country.Id).FirstOrDefault() == null)
                        {
                            shippingMethod.RestrictedCountries.Add(country);
                            _shippingService.UpdateShippingMethod(shippingMethod);
                        }
                    }
                    else
                    {
                        if (shippingMethod.RestrictedCountries.Where(c => c.Id == country.Id).FirstOrDefault() != null)
                        {
                            shippingMethod.RestrictedCountries.Remove(country);
                            _shippingService.UpdateShippingMethod(shippingMethod);
                        }
                    }
                }
            }

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Restrictions.Updated"));
            return RedirectToAction("Restrictions");
        }

        #endregion
    }
}
