using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.Fedex.Domain;
using Nop.Plugin.Shipping.Fedex.Models;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.Fedex.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class ShippingFedexController : BasePluginController
    {
        #region Fields

        private readonly FedexSettings _fedexSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor
        
        public ShippingFedexController(FedexSettings fedexSettings,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            this._fedexSettings = fedexSettings;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._settingService = settingService;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new FedexShippingModel()
            {
                Url = _fedexSettings.Url,
                Key = _fedexSettings.Key,
                Password = _fedexSettings.Password,
                AccountNumber = _fedexSettings.AccountNumber,
                MeterNumber = _fedexSettings.MeterNumber,
                DropoffType = Convert.ToInt32(_fedexSettings.DropoffType),
                AvailableDropOffTypes = _fedexSettings.DropoffType.ToSelectList(),
                UseResidentialRates = _fedexSettings.UseResidentialRates,
                ApplyDiscounts = _fedexSettings.ApplyDiscounts,
                AdditionalHandlingCharge = _fedexSettings.AdditionalHandlingCharge,
                PackingPackageVolume = _fedexSettings.PackingPackageVolume,
                PackingType = Convert.ToInt32(_fedexSettings.PackingType),
                PackingTypeValues = _fedexSettings.PackingType.ToSelectList(),
                PassDimensions = _fedexSettings.PassDimensions
            };

            // Load service names
            var availableServices = new FedexServices().Services;
            model.AvailableCarrierServices = availableServices;
            if (!string.IsNullOrEmpty(_fedexSettings.CarrierServicesOffered))
            {
                foreach (var service in availableServices)
                {
                    var serviceId = FedexServices.GetServiceId(service);
                    if (!string.IsNullOrEmpty(serviceId) && _fedexSettings.CarrierServicesOffered.Contains(serviceId))
                        model.CarrierServicesOffered.Add(service);
                }
            }

            return View("~/Plugins/Shipping.Fedex/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(FedexShippingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _fedexSettings.Url = model.Url;
            _fedexSettings.Key = model.Key;
            _fedexSettings.Password = model.Password;
            _fedexSettings.AccountNumber = model.AccountNumber;
            _fedexSettings.MeterNumber = model.MeterNumber;
            _fedexSettings.DropoffType = (DropoffType)model.DropoffType;
            _fedexSettings.UseResidentialRates = model.UseResidentialRates;
            _fedexSettings.ApplyDiscounts = model.ApplyDiscounts;
            _fedexSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;
            _fedexSettings.PackingPackageVolume = model.PackingPackageVolume;
            _fedexSettings.PackingType = (PackingType)model.PackingType;
            _fedexSettings.PassDimensions = model.PassDimensions;

            // Save selected services
            var carrierServicesOfferedDomestic = new StringBuilder();
            int carrierServicesDomesticSelectedCount = 0;
            if (model.CheckedCarrierServices != null)
            {
                foreach (var cs in model.CheckedCarrierServices)
                {
                    carrierServicesDomesticSelectedCount++;
                    string serviceId = FedexServices.GetServiceId(cs);
                    if (!String.IsNullOrEmpty(serviceId))
                        carrierServicesOfferedDomestic.AppendFormat("{0}:", serviceId);
                }
            }
            // Add default options if no services were selected
            if (carrierServicesDomesticSelectedCount == 0)
                _fedexSettings.CarrierServicesOffered = "FEDEX_2_DAY:PRIORITY_OVERNIGHT:FEDEX_GROUND:GROUND_HOME_DELIVERY:INTERNATIONAL_ECONOMY";
            else
                _fedexSettings.CarrierServicesOffered = carrierServicesOfferedDomestic.ToString();

            _settingService.SaveSetting(_fedexSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}
