using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.CustomerRoles.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Controllers
{
    [AdminAuthorize]
    public class DiscountRulesCustomerRolesController : BasePluginController
    {
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        public DiscountRulesCustomerRolesController(IDiscountService discountService,
            ICustomerService customerService, ISettingService settingService,
            IPermissionService permissionService)
        {
            this._discountService = discountService;
            this._customerService = customerService;
            this._settingService = settingService;
            this._permissionService = permissionService;
        }

        public ActionResult Configure(int discountId, int? discountRequirementId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            DiscountRequirement discountRequirement = null;
            if (discountRequirementId.HasValue)
            {
                discountRequirement = discount.DiscountRequirements.FirstOrDefault(dr => dr.Id == discountRequirementId.Value);
                if (discountRequirement == null)
                    return Content("Failed to load requirement.");
            }

            var restrictedToCustomerRoleId = _settingService.GetSettingByKey<int>(string.Format("DiscountRequirement.MustBeAssignedToCustomerRole-{0}", discountRequirementId.HasValue ? discountRequirementId.Value : 0));
            
            var model = new RequirementModel();
            model.RequirementId = discountRequirementId.HasValue ? discountRequirementId.Value : 0;
            model.DiscountId = discountId;
            model.CustomerRoleId = restrictedToCustomerRoleId;
            //customer roles
            //TODO localize "Select customer role"
            model.AvailableCustomerRoles.Add(new SelectListItem { Text = "Select customer role", Value = "0" });
            foreach (var cr in _customerService.GetAllCustomerRoles(true))
                model.AvailableCustomerRoles.Add(new SelectListItem { Text = cr.Name, Value = cr.Id.ToString(), Selected = discountRequirement != null && cr.Id == restrictedToCustomerRoleId });

            //add a prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("DiscountRulesCustomerRoles{0}", discountRequirementId.HasValue ? discountRequirementId.Value.ToString() : "0");

            return View("~/Plugins/DiscountRules.CustomerRoles/Views/DiscountRulesCustomerRoles/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult Configure(int discountId, int? discountRequirementId, int customerRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            DiscountRequirement discountRequirement = null;
            if (discountRequirementId.HasValue)
                discountRequirement = discount.DiscountRequirements.FirstOrDefault(dr => dr.Id == discountRequirementId.Value);

            if (discountRequirement != null)
            {
                //update existing rule
                _settingService.SetSetting(string.Format("DiscountRequirement.MustBeAssignedToCustomerRole-{0}", discountRequirement.Id), customerRoleId);
            }
            else
            {
                //save new rule
                discountRequirement = new DiscountRequirement
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