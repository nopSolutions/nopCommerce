using System;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Shipping.UPS.Domain;
using Nop.Plugin.Shipping.UPS.Models;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Shipping.UPS.Controllers
{
    [AdminAuthorize]
    public class ShippingUPSController : BasePluginController
    {
        private readonly UPSSettings _upsSettings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public ShippingUPSController(UPSSettings upsSettings,
            ISettingService settingService,
            ILocalizationService localizationService)
        {
            this._upsSettings = upsSettings;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new UPSShippingModel();
            model.Url = _upsSettings.Url;
            model.AccessKey = _upsSettings.AccessKey;
            model.Username = _upsSettings.Username;
            model.Password = _upsSettings.Password;
            model.AdditionalHandlingCharge = _upsSettings.AdditionalHandlingCharge;
            model.InsurePackage = _upsSettings.InsurePackage;
            model.PackingPackageVolume = _upsSettings.PackingPackageVolume;
            model.PackingType = Convert.ToInt32(_upsSettings.PackingType);
            model.PackingTypeValues = _upsSettings.PackingType.ToSelectList();
            model.PassDimensions = _upsSettings.PassDimensions;

            foreach (UPSCustomerClassification customerClassification in Enum.GetValues(typeof(UPSCustomerClassification)))
            {
                model.AvailableCustomerClassifications.Add(new SelectListItem
                    {
                        Text = CommonHelper.ConvertEnum(customerClassification.ToString()),
                        Value = customerClassification.ToString(),
                        Selected = customerClassification == _upsSettings.CustomerClassification
                    });
            }
            foreach (UPSPickupType pickupType in Enum.GetValues(typeof(UPSPickupType)))
            {
                model.AvailablePickupTypes.Add(new SelectListItem
                {
                    Text = CommonHelper.ConvertEnum(pickupType.ToString()),
                    Value = pickupType.ToString(),
                    Selected = pickupType == _upsSettings.PickupType
                });
            }
            foreach (UPSPackagingType packagingType in Enum.GetValues(typeof(UPSPackagingType)))
            {
                model.AvailablePackagingTypes.Add(new SelectListItem
                {
                    Text = CommonHelper.ConvertEnum(packagingType.ToString()),
                    Value = packagingType.ToString(),
                    Selected = packagingType == _upsSettings.PackagingType
                });
            }

            // Load Domestic service names
            string carrierServicesOfferedDomestic = _upsSettings.CarrierServicesOffered;
            foreach (string service in UPSServices.Services)
                model.AvailableCarrierServices.Add(service);

            if (!String.IsNullOrEmpty(carrierServicesOfferedDomestic))
                foreach (string service in UPSServices.Services)
                {
                    string serviceId = UPSServices.GetServiceId(service);
                    if (!String.IsNullOrEmpty(serviceId) && !String.IsNullOrEmpty(carrierServicesOfferedDomestic))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        if (carrierServicesOfferedDomestic.Contains(String.Format("[{0}]", serviceId)))
                            model.CarrierServicesOffered.Add(service);
                    }
                }

            return View("~/Plugins/Shipping.UPS/Views/ShippingUPS/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(UPSShippingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            _upsSettings.Url = model.Url;
            _upsSettings.AccessKey = model.AccessKey;
            _upsSettings.Username = model.Username;
            _upsSettings.Password = model.Password;
            _upsSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;
            _upsSettings.InsurePackage = model.InsurePackage;
            _upsSettings.CustomerClassification = (UPSCustomerClassification)Enum.Parse(typeof(UPSCustomerClassification), model.CustomerClassification);
            _upsSettings.PickupType = (UPSPickupType)Enum.Parse(typeof(UPSPickupType), model.PickupType);
            _upsSettings.PackagingType = (UPSPackagingType)Enum.Parse(typeof(UPSPackagingType), model.PackagingType);
            _upsSettings.PackingPackageVolume = model.PackingPackageVolume;
            _upsSettings.PackingType = (PackingType)model.PackingType;
            _upsSettings.PassDimensions = model.PassDimensions;
            _upsSettings.Tracing = model.Tracing;


            // Save selected services
            var carrierServicesOfferedDomestic = new StringBuilder();
            int carrierServicesDomesticSelectedCount = 0;
            if (model.CheckedCarrierServices != null)
            {
                foreach (var cs in model.CheckedCarrierServices)
                {
                    carrierServicesDomesticSelectedCount++;
                    string serviceId = UPSServices.GetServiceId(cs);
                    if (!String.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        carrierServicesOfferedDomestic.AppendFormat("[{0}]:", serviceId);
                    }
                }
            }
            // Add default options if no services were selected
            if (carrierServicesDomesticSelectedCount == 0)
                _upsSettings.CarrierServicesOffered = "[03]:[12]:[11]:[08]:";
            else
                _upsSettings.CarrierServicesOffered = carrierServicesOfferedDomestic.ToString();

            _settingService.SaveSetting(_upsSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

    }
}
