using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Tax.CountryStateZip.Domain;
using Nop.Plugin.Tax.CountryStateZip.Models;
using Nop.Plugin.Tax.CountryStateZip.Services;
using Nop.Services.Directory;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.Tax.CountryStateZip.Controllers
{
    [AdminAuthorize]
    public class TaxCountryStateZipController : BasePluginController
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
            CommonHelper.SetTelerikCulture();

            base.Initialize(requestContext);
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            if (!taxCategories.Any())
                return Content("No tax categories can be loaded");

            var model = new TaxRateListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            var stores = _storeService.GetAllStores();
            foreach (var s in stores)
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            //tax categories
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString() });
            //countries
            var countries = _countryService.GetAllCountries(showHidden: true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });
            var defaultCountry = countries.FirstOrDefault();
            if (defaultCountry != null)
            {
                var states = _stateProvinceService.GetStateProvincesByCountryId(defaultCountry.Id);
                foreach (var s in states)
                    model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            }

            return View("~/Plugins/Tax.CountryStateZip/Views/TaxCountryStateZip/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult RatesList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var records = _taxRateService.GetAllTaxRates(command.Page - 1, command.PageSize);
            var taxRatesModel = records
                .Select(x =>
                {
                    var m = new TaxRateModel
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
            var gridModel = new DataSourceResult
            {
                Data = taxRatesModel,
                Total = records.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult RateUpdate(TaxRateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = _taxRateService.GetTaxRateById(model.Id);
            taxRate.Zip = model.Zip == "*" ? null : model.Zip;
            taxRate.Percentage = model.Percentage;
            _taxRateService.UpdateTaxRate(taxRate);

            return new NullJsonResult();
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult RateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = _taxRateService.GetTaxRateById(id);
            if (taxRate != null)
                _taxRateService.DeleteTaxRate(taxRate);

            return new NullJsonResult();
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult AddTaxRate(TaxRateListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = new TaxRate
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
