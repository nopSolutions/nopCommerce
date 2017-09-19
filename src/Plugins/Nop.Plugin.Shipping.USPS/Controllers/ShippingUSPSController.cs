using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.USPS.Domain;
using Nop.Plugin.Shipping.USPS.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.USPS.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class ShippingUSPSController : BasePluginController
    {
        #region Fields

        private readonly USPSSettings _uspsSettings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public ShippingUSPSController(USPSSettings uspsSettings,
            ISettingService settingService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._uspsSettings = uspsSettings;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new USPSShippingModel();
            model.Url = _uspsSettings.Url;
            model.Username = _uspsSettings.Username;
            model.Password = _uspsSettings.Password;
            model.AdditionalHandlingCharge = _uspsSettings.AdditionalHandlingCharge;

            // Load Domestic service names
            string carrierServicesOfferedDomestic = _uspsSettings.CarrierServicesOfferedDomestic;
            foreach (string service in USPSServices.DomesticServices)
                model.AvailableCarrierServicesDomestic.Add(service);

            if (!String.IsNullOrEmpty(carrierServicesOfferedDomestic))
                foreach (string service in USPSServices.DomesticServices)
                {
                    string serviceId = USPSServices.GetServiceIdDomestic(service);
                    if (!String.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        if (carrierServicesOfferedDomestic.Contains($"[{serviceId}]"))
                            model.CarrierServicesOfferedDomestic.Add(service);
                    }
                }

            // Load Internation service names
            string carrierServicesOfferedInternational = _uspsSettings.CarrierServicesOfferedInternational;
            foreach (string service in USPSServices.InternationalServices)
                model.AvailableCarrierServicesInternational.Add(service);

            if (!String.IsNullOrEmpty(carrierServicesOfferedInternational))
                foreach (string service in USPSServices.InternationalServices)
                {
                    string serviceId = USPSServices.GetServiceIdInternational(service);
                    if (!String.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        if (carrierServicesOfferedInternational.Contains($"[{serviceId}]"))
                            model.CarrierServicesOfferedInternational.Add(service);
                    }
                }
            return View("~/Plugins/Shipping.USPS/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(USPSShippingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();
            
            //save settings
            _uspsSettings.Url = model.Url;
            _uspsSettings.Username = model.Username;
            _uspsSettings.Password = model.Password;
            _uspsSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;



            // Save selected Domestic services
            var carrierServicesOfferedDomestic = new StringBuilder();
            int carrierServicesDomesticSelectedCount = 0;
            if (model.CheckedCarrierServicesDomestic != null)
            {
                foreach (var cs in model.CheckedCarrierServicesDomestic)
                {
                    carrierServicesDomesticSelectedCount++;

                    string serviceId = USPSServices.GetServiceIdDomestic(cs);
                    //unselect any other services if NONE is selected
                    if (!String.IsNullOrEmpty(serviceId) && serviceId.Equals("NONE"))
                    {
                        carrierServicesOfferedDomestic.Clear();
                        carrierServicesOfferedDomestic.AppendFormat("[{0}]:", serviceId);
                        break;
                    }

                    if (!String.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        carrierServicesOfferedDomestic.AppendFormat("[{0}]:", serviceId);
                    }
                }
            }
            // Add default options if no services were selected
            if (carrierServicesDomesticSelectedCount == 0)
                _uspsSettings.CarrierServicesOfferedDomestic = "[1]:[3]:[4]:";
            else
                _uspsSettings.CarrierServicesOfferedDomestic = carrierServicesOfferedDomestic.ToString();



            // Save selected International services
            var carrierServicesOfferedInternational = new StringBuilder();
            int carrierServicesInternationalSelectedCount = 0;
            if (model.CheckedCarrierServicesInternational != null)
            {
                foreach (var cs in model.CheckedCarrierServicesInternational)
                {
                    carrierServicesInternationalSelectedCount++;
                    string serviceId = USPSServices.GetServiceIdInternational(cs);
                    // unselect other services if NONE is selected
                    if (!String.IsNullOrEmpty(serviceId) && serviceId.Equals("NONE"))
                    {
                        carrierServicesOfferedInternational.Clear();
                        carrierServicesOfferedInternational.AppendFormat("[{0}]:", serviceId);
                        break;
                    }
                    if (!String.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        carrierServicesOfferedInternational.AppendFormat("[{0}]:", serviceId);
                    }
                }
            }
            // Add default options if no services were selected
            if (carrierServicesInternationalSelectedCount == 0)
                _uspsSettings.CarrierServicesOfferedInternational = "[2]:[15]:[1]:";
            else
                _uspsSettings.CarrierServicesOfferedInternational = carrierServicesOfferedInternational.ToString();
            

            _settingService.SaveSetting(_uspsSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}
