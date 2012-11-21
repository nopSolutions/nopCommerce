using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.CustomerRoles.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Controllers
{
    [AdminAuthorize]
    public class DiscountRulesCustomerRolesController : Controller
    {
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;

        public DiscountRulesCustomerRolesController(IDiscountService discountService,
            ICustomerService customerService, ISettingService settingService)
        {
            this._discountService = discountService;
            this._customerService = customerService;
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

            var restrictedToCustomerRoleId = _settingService.GetSettingByKey<int>(string.Format("DiscountRequirement.MustBeAssignedToCustomerRole-{0}", discountRequirementId.HasValue ? discountRequirementId.Value : 0));
            
            var model = new RequirementModel();
            model.RequirementId = discountRequirementId.HasValue ? discountRequirementId.Value : 0;
            model.DiscountId = discountId;
            model.CustomerRoleId = restrictedToCustomerRoleId;
            //countries
            model.AvailableCustomerRoles.Add(new SelectListItem() { Text = "Select customer role", Value = "0" });
            foreach (var cr in _customerService.GetAllCustomerRoles(true))
                model.AvailableCustomerRoles.Add(new SelectListItem() { Text = cr.Name, Value = cr.Id.ToString(), Selected = discountRequirement != null && cr.Id == restrictedToCustomerRoleId });

            //add a prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("DiscountRulesCustomerRoles{0}", discountRequirementId.HasValue ? discountRequirementId.Value.ToString() : "0");

            return View("Nop.Plugin.DiscountRules.CustomerRoles.Views.DiscountRulesCustomerRoles.Configure", model);
        }

        [HttpPost]
        public ActionResult Configure(int discountId, int? discountRequirementId, int customerRoleId)
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
                _settingService.SetSetting(string.Format("DiscountRequirement.MustBeAssignedToCustomerRole-{0}", discountRequirement.Id), customerRoleId);
            }
            else
            {
                //save new rule
                discountRequirement = new DiscountRequirement()
                {
                    DiscountRequirementRuleSystemName = "DiscountRequirement.MustBeAssignedToCustomerRole"
                };
                discount.DiscountRequirements.Add(discountRequirement);
                _discountService.UpdateDiscount(discount);

                _settingService.SetSetting(string.Format("DiscountRequirement.MustBeAssignedToCustomerRole-{0}", discountRequirement.Id), customerRoleId);
            }
            return Json(new { Result = true, NewRequirementId = discountRequirement.Id }, JsonRequestBehavior.AllowGet);
        }
        
    }
}