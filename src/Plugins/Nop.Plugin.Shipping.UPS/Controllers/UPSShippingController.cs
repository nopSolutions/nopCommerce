using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Shipping.UPS.Domain;
using Nop.Plugin.Shipping.UPS.Models;
using Nop.Plugin.Shipping.UPS.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.UPS.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class UPSShippingController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly UPSService _upsService;
        private readonly UPSSettings _upsSettings;

        #endregion

        #region Ctor

        public UPSShippingController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            UPSService upsService,
            UPSSettings upsSettings)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _upsService = upsService;
            _upsSettings = upsSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            //whether user has the authority to manage configuration
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare common model
            var model = new UPSShippingModel
            {
                AccountNumber = _upsSettings.AccountNumber,
                AccessKey = _upsSettings.AccessKey,
                Username = _upsSettings.Username,
                Password = _upsSettings.Password,
                UseSandbox = _upsSettings.UseSandbox,
                AdditionalHandlingCharge = _upsSettings.AdditionalHandlingCharge,
                InsurePackage = _upsSettings.InsurePackage,
                CustomerClassification = (int)_upsSettings.CustomerClassification,
                PickupType = (int)_upsSettings.PickupType,
                PackagingType = (int)_upsSettings.PackagingType,
                SaturdayDeliveryEnabled = _upsSettings.SaturdayDeliveryEnabled,
                PassDimensions = _upsSettings.PassDimensions,
                PackingPackageVolume = _upsSettings.PackingPackageVolume,
                PackingType = (int)_upsSettings.PackingType,
                Tracing = _upsSettings.Tracing
            };

            //prepare offered delivery services
            var servicesCodes = _upsSettings.CarrierServicesOffered.Split(':', StringSplitOptions.RemoveEmptyEntries)
                .Select(idValue => idValue.Trim('[', ']')).ToList();

            //prepare available options
            model.AvailableCustomerClassifications = CustomerClassification.DailyRates.ToSelectList(false)
                .Select(item => new SelectListItem(item.Text, item.Value)).ToList();
            model.AvailablePickupTypes = PickupType.DailyPickup.ToSelectList(false)
                .Select(item => new SelectListItem(item.Text, item.Value)).ToList();
            model.AvailablePackagingTypes = PackagingType.CustomerSuppliedPackage.ToSelectList(false)
                .Select(item => new SelectListItem(item.Text?.TrimStart('_'), item.Value)).ToList();
            model.AvaliablePackingTypes = PackingType.PackByDimensions.ToSelectList(false)
                .Select(item => new SelectListItem(item.Text, item.Value)).ToList();
            model.AvailableCarrierServices = DeliveryService.Standard.ToSelectList(false).Select(item =>
            {
                var serviceCode = _upsService.GetUpsCode((DeliveryService)int.Parse(item.Value));
                return new SelectListItem($"UPS {item.Text?.TrimStart('_')}", serviceCode, servicesCodes.Contains(serviceCode));
            }).ToList();

            return View("~/Plugins/Shipping.UPS/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(UPSShippingModel model)
        {
            //whether user has the authority to manage configuration
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _upsSettings.AccountNumber = model.AccountNumber;
            _upsSettings.AccessKey = model.AccessKey;
            _upsSettings.Username = model.Username;
            _upsSettings.Password = model.Password;
            _upsSettings.UseSandbox = model.UseSandbox;
            _upsSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;
            _upsSettings.CustomerClassification = (CustomerClassification)model.CustomerClassification;
            _upsSettings.PickupType = (PickupType)model.PickupType;
            _upsSettings.PackagingType = (PackagingType)model.PackagingType;
            _upsSettings.InsurePackage = model.InsurePackage;
            _upsSettings.SaturdayDeliveryEnabled = model.SaturdayDeliveryEnabled;
            _upsSettings.PassDimensions = model.PassDimensions;
            _upsSettings.PackingPackageVolume = model.PackingPackageVolume;
            _upsSettings.PackingType = (PackingType)model.PackingType;
            _upsSettings.Tracing = model.Tracing;

            //use default services if no one is selected 
            if (!model.CarrierServices.Any())
            {
                model.CarrierServices = new List<string>
                {
                    _upsService.GetUpsCode(DeliveryService.Ground),
                    _upsService.GetUpsCode(DeliveryService.WorldwideExpedited),
                    _upsService.GetUpsCode(DeliveryService.Standard),
                    _upsService.GetUpsCode(DeliveryService._3DaySelect)
                };
            }
            _upsSettings.CarrierServicesOffered = string.Join(':', model.CarrierServices.Select(service => $"[{service}]"));

            _settingService.SaveSetting(_upsSettings);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}