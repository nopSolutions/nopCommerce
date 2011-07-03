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
        private readonly ShippingByWeightSettings _shippingByWeightSettings;
        private readonly IShippingByWeightService _shippingByWeightService;
        private readonly ISettingService _settingService;

        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;

        public ShippingByWeightController(IShippingService shippingService,
            ICountryService countryService, ShippingByWeightSettings shippingByWeightSettings,
            IShippingByWeightService shippingByWeightService, ISettingService settingService,
            ICurrencyService currencyService, CurrencySettings currencySettings,
            IMeasureService measureService, MeasureSettings measureSettings)
        {
            this._shippingService = shippingService;
            this._countryService = countryService;
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
            foreach (var sm in shippingMethods)
                model.AvailableShippingMethods.Add(new SelectListItem() { Text = sm.Name, Value = sm.Id.ToString() });
            

            model.AvailableCountries.Add(new SelectListItem() { Text = "*", Value = "0" });
            var countries = _countryService.GetAllCountries(true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            model.LimitMethodsToCreated = _shippingByWeightSettings.LimitMethodsToCreated;
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;

            model.Records = _shippingByWeightService.GetAll()
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
                    var shippingMethodId = _shippingService.GetShippingMethodById(x.ShippingMethodId);
                    m.ShippingMethodName = (shippingMethodId != null) ? shippingMethodId.Name : "Unavailable";
                    if (x.CountryId > 0)
                    {
                        var c = _countryService.GetCountryById(x.CountryId);
                        m.CountryName = (c != null) ? c.Name : "Unavailable";
                    }
                    else
                    {
                        m.CountryName = "*";
                    }

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
                    var shippingMethodId = _shippingService.GetShippingMethodById(x.ShippingMethodId);
                    m.ShippingMethodName = (shippingMethodId != null) ? shippingMethodId.Name : "Unavailable";
                    if (x.CountryId > 0)
                    {
                        var c = _countryService.GetCountryById(x.CountryId);
                        m.CountryName = (c != null) ? c.Name : "Unavailable";
                    }
                    else
                    {
                        m.CountryName = "*";
                    }
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
            if (!ModelState.IsValid)
            {
                return new JsonResult { Data = "error" };
            }

            var sbw = _shippingByWeightService.GetById(model.Id);
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
            _shippingByWeightService.DeleteShippingByWeightRecord(sbw);

            return RatesList(command);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("addshippingbyweightrecord")]
        public ActionResult AddShippingByWeightRecord(ShippingByWeightListModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            var sbw = new ShippingByWeightRecord()
            {
                ShippingMethodId = model.AddShippingMethodId,
                CountryId = model.AddCountryId,
                From = model.AddFrom,
                To = model.AddTo,
                UsePercentage = model.AddUsePercentage,
                ShippingChargeAmount = model.AddShippingChargeAmount,
                ShippingChargePercentage = model.AddShippingChargePercentage
            };
            _shippingByWeightService.InsertShippingByWeightRecord(sbw);

            return Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("savegeneralsettings")]
        public ActionResult SaveGeneralSettings(ShippingByWeightListModel model)
        {
            //save settings
            _shippingByWeightSettings.LimitMethodsToCreated = model.LimitMethodsToCreated;
            _settingService.SaveSetting(_shippingByWeightSettings);
            
            return Configure();
        }
        
    }
}
