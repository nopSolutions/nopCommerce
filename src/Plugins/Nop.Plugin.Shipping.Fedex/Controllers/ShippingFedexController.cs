using System;
using System.Text;
using System.Web.Mvc;
using Nop.Plugin.Shipping.Fedex.Domain;
using Nop.Plugin.Shipping.Fedex.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using Nop.Core;

namespace Nop.Plugin.Shipping.Fedex.Controllers
{
    [AdminAuthorize]
    public class ShippingFedexController : Controller
    {
        private readonly FedexSettings _fedexSettings;
        private readonly ISettingService _settingService;

        public ShippingFedexController(FedexSettings fedexSettings, ISettingService settingService)
        {
            this._fedexSettings = fedexSettings;
            this._settingService = settingService;
        }

        public ActionResult Configure()
        {
            var model = new FedexShippingModel();
            model.Url = _fedexSettings.Url;
            model.Key = _fedexSettings.Key;
            model.Password = _fedexSettings.Password;
            model.AccountNumber = _fedexSettings.AccountNumber;
            model.MeterNumber = _fedexSettings.MeterNumber;
            model.UseResidentialRates = _fedexSettings.UseResidentialRates;
            model.ApplyDiscounts = _fedexSettings.ApplyDiscounts;
            model.AdditionalHandlingCharge = _fedexSettings.AdditionalHandlingCharge;
            model.Street = _fedexSettings.Street;
            model.City = _fedexSettings.City;
            model.StateOrProvinceCode = _fedexSettings.StateOrProvinceCode;
            model.PostalCode = _fedexSettings.PostalCode;
            model.CountryCode = _fedexSettings.CountryCode;


            var services = new FedexServices();
            // Load service names
            string carrierServicesOfferedDomestic = _fedexSettings.CarrierServicesOffered;
            foreach (string service in services.Services)
                model.AvailableCarrierServices.Add(service);

            if (!String.IsNullOrEmpty(carrierServicesOfferedDomestic))
                foreach (string service in services.Services)
                {
                    string serviceId = FedexServices.GetServiceId(service);
                    if (!String.IsNullOrEmpty(serviceId) && !String.IsNullOrEmpty(carrierServicesOfferedDomestic))
                    {
                        if (carrierServicesOfferedDomestic.Contains(serviceId))
                            model.CarrierServicesOffered.Add(service);
                    }
                }


            return View("Nop.Plugin.Shipping.Fedex.Views.ShippingFedex.Configure", model);
        }

        [HttpPost]
        public ActionResult Configure(FedexShippingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            _fedexSettings.Url = model.Url;
            _fedexSettings.Key = model.Key;
            _fedexSettings.Password = model.Password;
            _fedexSettings.AccountNumber = model.AccountNumber;
            _fedexSettings.MeterNumber = model.MeterNumber;
            _fedexSettings.UseResidentialRates = model.UseResidentialRates;
            _fedexSettings.ApplyDiscounts = model.ApplyDiscounts;
            _fedexSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;
            _fedexSettings.Street = model.Street;
            _fedexSettings.City = model.City;
            _fedexSettings.StateOrProvinceCode = CommonHelper.EnsureMaximumLength(model.StateOrProvinceCode, 2);
            _fedexSettings.PostalCode = model.PostalCode;
            _fedexSettings.CountryCode = model.CountryCode;



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
            // Add default options if no services were selected (Priority Mail International, First-Class Mail International Package, and Express Mail International)
            if (carrierServicesDomesticSelectedCount == 0)
                _fedexSettings.CarrierServicesOffered = "FEDEX_2_DAY:PRIORITY_OVERNIGHT:FEDEX_GROUND:GROUND_HOME_DELIVERY:INTERNATIONAL_ECONOMY";
            else
                _fedexSettings.CarrierServicesOffered = carrierServicesOfferedDomestic.ToString();


            _settingService.SaveSetting(_fedexSettings);

            return Configure();
        }
    }
}
