using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Shipping.ByWeight.Domain;
using Nop.Plugin.Shipping.ByWeight.Models;
using Nop.Plugin.Shipping.ByWeight.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Shipping;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Plugin.Shipping.ByWeight.Controllers
{
    [AdminAuthorize]
    public class ShippingByWeightController : Controller
    {
        private readonly IShippingService _shippingService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ShippingByWeightSettings _shippingByWeightSettings;
        private readonly IShippingByWeightService _shippingByWeightService;
        private readonly ISettingService _settingService;

        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;

        public ShippingByWeightController(IShippingService shippingService,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            ShippingByWeightSettings shippingByWeightSettings,
            IShippingByWeightService shippingByWeightService, ISettingService settingService,
            ICurrencyService currencyService, CurrencySettings currencySettings,
            IMeasureService measureService, MeasureSettings measureSettings)
        {
            this._shippingService = shippingService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingByWeightSettings = shippingByWeightSettings;
            this._shippingByWeightService = shippingByWeightService;
            this._settingService = settingService;

            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
        }
        
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //little hack here
            //always set culture to 'en-US' (Telerik Grid has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs
            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            base.Initialize(requestContext);
        }

        public ActionResult Configure()
        {
            var shippingMethods = _shippingService.GetAllShippingMethods();
            if (shippingMethods.Count == 0)
                return Content("No shipping methods can be loaded");

            var model = new ShippingByWeightListModel();
            //shipping methods
            foreach (var sm in shippingMethods)
                model.AvailableShippingMethods.Add(new SelectListItem() { Text = sm.Name, Value = sm.Id.ToString() });
            //countries
            model.AvailableCountries.Add(new SelectListItem() { Text = "*", Value = "0" });
            var countries = _countryService.GetAllCountries(true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem() { Text = "*", Value = "0" });
            //other settings
            model.LimitMethodsToCreated = _shippingByWeightSettings.LimitMethodsToCreated;
            model.CalculatePerWeightUnit = _shippingByWeightSettings.CalculatePerWeightUnit;
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;
            //rates
            model.Records = _shippingByWeightService.GetAll()
                .Select(x =>
                {
                    var m = new ShippingByWeightModel()
                    {
                        Id = x.Id,
                        ShippingMethodId = x.ShippingMethodId,
                        CountryId = x.CountryId,
                        StateProvinceId = x.StateProvinceId,
                        Zip = x.Zip,
                        From = x.From,
                        To = x.To,
                        UsePercentage = x.UsePercentage,
                        ShippingChargePercentage = x.ShippingChargePercentage,
                        ShippingChargeAmount = x.ShippingChargeAmount,
                    };
                    var shippingMethod = _shippingService.GetShippingMethodById(x.ShippingMethodId);
                    m.ShippingMethodName = (shippingMethod != null) ? shippingMethod.Name : "Unavailable";
                    var c = _countryService.GetCountryById(x.CountryId);
                    m.CountryName = (c != null) ? c.Name : "*";
                    var s = _stateProvinceService.GetStateProvinceById(x.StateProvinceId);
                    m.StateProvinceName = (s != null) ? s.Name : "*";
                    m.Zip = (!String.IsNullOrEmpty(x.Zip)) ? x.Zip : "*";
                    
                    return m;
                })
                .ToList();

            return View("Nop.Plugin.Shipping.ByWeight.Views.ShippingByWeight.Configure", model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult RatesList(GridCommand command)
        {
            var sbwModel = _shippingByWeightService.GetAll()
                .Select(x =>
                {
                    var m = new ShippingByWeightModel()
                    {
                        Id = x.Id,
                        ShippingMethodId = x.ShippingMethodId,
                        CountryId = x.CountryId,
                        From = x.From,
                        To = x.To,
                        UsePercentage = x.UsePercentage,
                        ShippingChargePercentage = x.ShippingChargePercentage,
                        ShippingChargeAmount = x.ShippingChargeAmount,
                    };
                    var shippingMethod = _shippingService.GetShippingMethodById(x.ShippingMethodId);
                    m.ShippingMethodName = (shippingMethod != null) ? shippingMethod.Name : "Unavailable";
                    var c = _countryService.GetCountryById(x.CountryId);
                    m.CountryName = (c != null) ? c.Name : "*";
                    var s = _stateProvinceService.GetStateProvinceById(x.StateProvinceId);
                    m.StateProvinceName = (s != null) ? s.Name : "*";
                    m.Zip = (!String.IsNullOrEmpty(x.Zip)) ? x.Zip : "*";
                    return m;
                })
                .ToList();
            var model = new GridModel<ShippingByWeightModel>
            {
                Data = sbwModel,
                Total = sbwModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult RateUpdate(ShippingByWeightModel model, GridCommand command)
        {
            var sbw = _shippingByWeightService.GetById(model.Id);
            sbw.Zip = model.Zip == "*" ? null : model.Zip;
            sbw.From = model.From;
            sbw.To = model.To;
            sbw.UsePercentage = model.UsePercentage;
            sbw.ShippingChargeAmount = model.ShippingChargeAmount;
            sbw.ShippingChargePercentage = model.ShippingChargePercentage;
            _shippingByWeightService.UpdateShippingByWeightRecord(sbw);

            return RatesList(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult RateDelete(int id, GridCommand command)
        {
            var sbw = _shippingByWeightService.GetById(id);
            if (sbw != null)
                _shippingByWeightService.DeleteShippingByWeightRecord(sbw);

            return RatesList(command);
        }

        [HttpPost]
        public ActionResult AddShippingRate(ShippingByWeightListModel model)
        {
            var sbw = new ShippingByWeightRecord()
            {
                ShippingMethodId = model.AddShippingMethodId,
                CountryId = model.AddCountryId,
                StateProvinceId = model.AddStateProvinceId,
                Zip = model.AddZip,
                From = model.AddFrom,
                To = model.AddTo,
                UsePercentage = model.AddUsePercentage,
                ShippingChargeAmount = model.AddShippingChargeAmount,
                ShippingChargePercentage = model.AddShippingChargePercentage
            };
            _shippingByWeightService.InsertShippingByWeightRecord(sbw);

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult SaveGeneralSettings(ShippingByWeightListModel model)
        {
            //save settings
            _shippingByWeightSettings.LimitMethodsToCreated = model.LimitMethodsToCreated;
            _shippingByWeightSettings.CalculatePerWeightUnit = model.CalculatePerWeightUnit;
            _settingService.SaveSetting(_shippingByWeightSettings);

            return Json(new { Result = true });
        }
        
    }
}
