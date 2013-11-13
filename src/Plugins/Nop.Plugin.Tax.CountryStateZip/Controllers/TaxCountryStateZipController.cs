using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Nop.Plugin.Tax.CountryStateZip.Domain;
using Nop.Plugin.Tax.CountryStateZip.Models;
using Nop.Plugin.Tax.CountryStateZip.Services;
using Nop.Services.Directory;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Plugin.Tax.CountryStateZip.Controllers
{
    [AdminAuthorize]
    public class TaxCountryStateZipController : Controller
    {
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ITaxRateService _taxRateService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;

        public TaxCountryStateZipController(ITaxCategoryService taxCategoryService,
            ICountryService countryService, 
            IStateProvinceService stateProvinceService,
            ITaxRateService taxRateService,
            IPermissionService permissionService,
            IStoreService storeService)
        {
            this._taxCategoryService = taxCategoryService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._taxRateService = taxRateService;
            this._permissionService = permissionService;
            this._storeService = storeService;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //little hack here
            //always set culture to 'en-US' (Telerik has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs
            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            base.Initialize(requestContext);
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            if (taxCategories.Count == 0)
                return Content("No tax categories can be loaded");

            var model = new TaxRateListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem() { Text = "*", Value = "0" });
            var stores = _storeService.GetAllStores();
            foreach (var s in stores)
                model.AvailableStores.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });
            //tax categories
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem() { Text = tc.Name, Value = tc.Id.ToString() });
            //countries
            var countries = _countryService.GetAllCountries(true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem() { Text = "*", Value = "0" });
            var states = _stateProvinceService.GetStateProvincesByCountryId(countries.FirstOrDefault().Id);
            foreach (var s in states)
                model.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });
            
            return View("Nop.Plugin.Tax.CountryStateZip.Views.TaxCountryStateZip.Configure", model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult RatesList(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var records = _taxRateService.GetAllTaxRates(command.Page - 1, command.PageSize);
            var taxRatesModel = records
                .Select(x =>
                {
                    var m = new TaxRateModel()
                    {
                        Id = x.Id,
                        StoreId = x.StoreId,
                        TaxCategoryId = x.TaxCategoryId,
                        CountryId = x.CountryId,
                        StateProvinceId = x.StateProvinceId,
                        Zip = x.Zip,
                        Percentage = x.Percentage,
                    };
                    //store
                    var store = _storeService.GetStoreById(x.StoreId);
                    m.StoreName = (store != null) ? store.Name : "*";
                    //tax category
                    var tc = _taxCategoryService.GetTaxCategoryById(x.TaxCategoryId);
                    m.TaxCategoryName = (tc != null) ? tc.Name : "";
                    //country
                    var c = _countryService.GetCountryById(x.CountryId);
                    m.CountryName = (c != null) ? c.Name : "Unavailable";
                    //state
                    var s = _stateProvinceService.GetStateProvinceById(x.StateProvinceId);
                    m.StateProvinceName = (s != null) ? s.Name : "*";
                    //zip
                    m.Zip = (!String.IsNullOrEmpty(x.Zip)) ? x.Zip : "*";
                    return m;
                })
                .ToList();
            var model = new GridModel<TaxRateModel>
            {
                Data = taxRatesModel,
                Total = records.TotalCount
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult RateUpdate(TaxRateModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = _taxRateService.GetTaxRateById(model.Id);
            taxRate.Zip = model.Zip == "*" ? null : model.Zip;
            taxRate.Percentage = model.Percentage;
            _taxRateService.UpdateTaxRate(taxRate);

            return RatesList(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult RateDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = _taxRateService.GetTaxRateById(id);
            if (taxRate != null)
                _taxRateService.DeleteTaxRate(taxRate);

            return RatesList(command);
        }

        [HttpPost]
        public ActionResult AddTaxRate(TaxRateListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = new TaxRate()
            {
                StoreId = model.AddStoreId,
                TaxCategoryId = model.AddTaxCategoryId,
                CountryId = model.AddCountryId,
                StateProvinceId = model.AddStateProvinceId,
                Zip = model.AddZip,
                Percentage = model.AddPercentage
            };
            _taxRateService.InsertTaxRate(taxRate);

            return Json(new { Result = true });
        }
    }
}
