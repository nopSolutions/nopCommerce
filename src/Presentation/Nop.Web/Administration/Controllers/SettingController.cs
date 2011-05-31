using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Blogs;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class SettingController : BaseNopController
	{
		#region Fields

        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly ITaxCategoryService _taxCategoryService;

        private BlogSettings _blogSettings;
        private ForumSettings _forumSettings;
        private NewsSettings _newsSettings;
        private ShippingSettings _shippingSettings;
        private TaxSettings _taxSettings;

		#endregion Fields 

		#region Constructors

        public SettingController(ISettingService settingService,  
            ICountryService countryService, IStateProvinceService stateProvinceService,
            IAddressService addressService, ITaxCategoryService taxCategoryService,
            BlogSettings blogSettings,
            ForumSettings forumSettings,
            NewsSettings newsSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings)
        {
            this._settingService = settingService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._taxCategoryService = taxCategoryService;

            this._blogSettings = blogSettings;
            this._forumSettings = forumSettings;
            this._newsSettings = newsSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
		}

		#endregion Constructors 
        
        #region Methods

        public ActionResult Blog()
        {
            var model = _blogSettings.ToModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Blog(BlogSettingsModel model)
        {
            _blogSettings = model.ToEntity(_blogSettings);
            _settingService.SaveSetting(_blogSettings);
            return RedirectToAction("Blog");
        }




        public ActionResult Forum()
        {
            var model = _forumSettings.ToModel();
            model.ForumEditorValues = _forumSettings.ForumEditor.ToSelectList();
            return View(model);
        }
        [HttpPost]
        public ActionResult Forum(ForumSettingsModel model)
        {
            _forumSettings = model.ToEntity(_forumSettings);
            _settingService.SaveSetting(_forumSettings);
            return RedirectToAction("Forum");
        }




        public ActionResult News()
        {
            var model = _newsSettings.ToModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult News(NewsSettingsModel model)
        {
            _newsSettings = model.ToEntity(_newsSettings);
            _settingService.SaveSetting(_newsSettings);
            return RedirectToAction("News");
        }




        public ActionResult Shipping()
        {
            var model = _shippingSettings.ToModel();

            //shipping origin
            var originAddress = _shippingSettings.ShippingOriginAddressId > 0
                                     ? _addressService.GetAddressById(_shippingSettings.ShippingOriginAddressId)
                                     : null;
            if (originAddress != null)
                model.ShippingOriginAddress = originAddress.ToModel();
            else
                model.ShippingOriginAddress = new AddressModel();

            model.ShippingOriginAddress.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.ShippingOriginAddress.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (originAddress != null && c.Id == originAddress.CountryId) });

            var states = originAddress != null && originAddress.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(originAddress.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.ShippingOriginAddress.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == originAddress.StateProvinceId) });
            }
            else
                model.ShippingOriginAddress.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });
            model.ShippingOriginAddress.FirstNameDisabled = true;
            model.ShippingOriginAddress.LastNameDisabled = true;
            model.ShippingOriginAddress.EmailDisabled = true;
            model.ShippingOriginAddress.CompanyDisabled = true;
            model.ShippingOriginAddress.CityDisabled = true;
            model.ShippingOriginAddress.Address1Disabled = true;
            model.ShippingOriginAddress.Address2Disabled = true;
            model.ShippingOriginAddress.PhoneNumberDisabled = true;
            model.ShippingOriginAddress.FaxNumberDisabled = true;

            return View(model);
        }
        [HttpPost]
        public ActionResult Shipping(ShippingSettingsModel model)
        {
            _shippingSettings = model.ToEntity(_shippingSettings);

            var originAddress = _addressService.GetAddressById(_shippingSettings.ShippingOriginAddressId) ??
                                         new Core.Domain.Common.Address()
                                         {
                                             CreatedOnUtc = DateTime.UtcNow,
                                         };
            originAddress = model.ShippingOriginAddress.ToEntity(originAddress);
            if (originAddress.Id > 0)
                _addressService.UpdateAddress(originAddress);
            else
                _addressService.InsertAddress(originAddress);

            _shippingSettings.ShippingOriginAddressId = originAddress.Id;
            _settingService.SaveSetting(_shippingSettings);

            return RedirectToAction("Shipping");
        }




        public ActionResult Tax()
        {
            var model = _taxSettings.ToModel();
            model.TaxBasedOnValues = _taxSettings.TaxBasedOn.ToSelectList();
            model.TaxDisplayTypeValues = _taxSettings.TaxDisplayType.ToSelectList();

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.ShippingTaxCategories.Add(new SelectListItem() { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.ShippingTaxCategories.Add(new SelectListItem() { Text = tc.Name, Value = tc.Id.ToString(), Selected = tc.Id == _taxSettings.ShippingTaxClassId });
            model.PaymentMethodAdditionalFeeTaxCategories.Add(new SelectListItem() { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.PaymentMethodAdditionalFeeTaxCategories.Add(new SelectListItem() { Text = tc.Name, Value = tc.Id.ToString(), Selected = tc.Id == _taxSettings.PaymentMethodAdditionalFeeTaxClassId });

            //EU VAT countries
            model.EuVatShopCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.EuVatShopCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = c.Id == _taxSettings.EuVatShopCountryId });

            //default tax address
            var defaultAddress = _taxSettings.DefaultTaxAddressId > 0
                                     ? _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId)
                                     : null;
            if (defaultAddress != null)
                model.DefaultTaxAddress = defaultAddress.ToModel();
            else
                model.DefaultTaxAddress = new AddressModel();

            model.DefaultTaxAddress.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.DefaultTaxAddress.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (defaultAddress != null && c.Id == defaultAddress.CountryId) });

            var states = defaultAddress != null && defaultAddress.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(defaultAddress.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.DefaultTaxAddress.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == defaultAddress.StateProvinceId) });
            }
            else
                model.DefaultTaxAddress.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });
            model.DefaultTaxAddress.FirstNameDisabled = true;
            model.DefaultTaxAddress.LastNameDisabled = true;
            model.DefaultTaxAddress.EmailDisabled = true;
            model.DefaultTaxAddress.CompanyDisabled = true;
            model.DefaultTaxAddress.CityDisabled = true;
            model.DefaultTaxAddress.Address1Disabled = true;
            model.DefaultTaxAddress.Address2Disabled = true;
            model.DefaultTaxAddress.PhoneNumberDisabled = true;
            model.DefaultTaxAddress.FaxNumberDisabled = true;

            return View(model);
        }
        [HttpPost]
        public ActionResult Tax(TaxSettingsModel model)
        {
            _taxSettings = model.ToEntity(_taxSettings);

            var defaultAddress = _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId) ??
                                         new Core.Domain.Common.Address()
                                         {
                                             CreatedOnUtc = DateTime.UtcNow,
                                         };
            defaultAddress = model.DefaultTaxAddress.ToEntity(defaultAddress);
            if (defaultAddress.Id > 0)
                _addressService.UpdateAddress(defaultAddress);
            else
                _addressService.InsertAddress(defaultAddress);

            _taxSettings.DefaultTaxAddressId = defaultAddress.Id;
            _settingService.SaveSetting(_taxSettings);

            return RedirectToAction("Tax");
        }

        #endregion
    }
}
