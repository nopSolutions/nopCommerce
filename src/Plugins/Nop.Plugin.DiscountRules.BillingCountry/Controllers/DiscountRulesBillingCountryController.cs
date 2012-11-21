using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.BillingCountry.Models;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.DiscountRules.BillingCountry.Controllers
{
    [AdminAuthorize]
    public class DiscountRulesBillingCountryController : Controller
    {
        private readonly ILocalizationService _localizationService;
        private readonly IDiscountService _discountService;
        private readonly ICountryService _countryService;
        private readonly ISettingService _settingService;

        public DiscountRulesBillingCountryController(ILocalizationService localizationService, 
            IDiscountService discountService, ICountryService countryService,
            ISettingService settingService)
        {
            this._localizationService = localizationService;
            this._discountService = discountService;
            this._countryService = countryService;
            this._settingService = settingService;
        }

        public ActionResult Configure(int discountId, int? discountRequirementId)
        {
            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            DiscountRequirement discountRequirement = null;
            if (discountRequirementId.HasValue)
            {
                discountRequirement = discount.DiscountRequirements.Where(dr => dr.Id == discountRequirementId.Value).FirstOrDefault();
                if (discountRequirement == null)
                    return Content("Failed to load requirement.");
            }

            var billingCountryId = _settingService.GetSettingByKey<int>(string.Format("DiscountRequirement.BillingCountry-{0}", discountRequirementId.HasValue ? discountRequirementId.Value : 0));

            var model = new RequirementModel();
            model.RequirementId = discountRequirementId.HasValue ? discountRequirementId.Value : 0;
            model.DiscountId = discountId;
            model.CountryId = billingCountryId;
            //countries
            model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Plugins.DiscountRules.BillingCountry.Fields.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = discountRequirement != null && c.Id == billingCountryId });

            //add a prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("DiscountRulesBillingCountry{0}", discountRequirementId.HasValue ? discountRequirementId.Value.ToString() : "0");

            return View("Nop.Plugin.DiscountRules.BillingCountry.Views.DiscountRulesBillingCountry.Configure", model);
        }

        [HttpPost]
        public ActionResult Configure(int discountId, int? discountRequirementId, int countryId)
        {
            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            DiscountRequirement discountRequirement = null;
            if (discountRequirementId.HasValue)
                discountRequirement = discount.DiscountRequirements.Where(dr => dr.Id == discountRequirementId.Value).FirstOrDefault();

            if (discountRequirement != null)
            {
                //update existing rule
                _settingService.SetSetting(string.Format("DiscountRequirement.BillingCountry-{0}", discountRequirement.Id), countryId);
            }
            else
            {
                //save new rule
                discountRequirement = new DiscountRequirement()
                {
                    DiscountRequirementRuleSystemName = "DiscountRequirement.BillingCountryIs"
                };
                discount.DiscountRequirements.Add(discountRequirement);
                _discountService.UpdateDiscount(discount);
                
                _settingService.SetSetting(string.Format("DiscountRequirement.BillingCountry-{0}", discountRequirement.Id), countryId);
            }
            return Json(new { Result = true, NewRequirementId = discountRequirement.Id }, JsonRequestBehavior.AllowGet);
        }
    }
}