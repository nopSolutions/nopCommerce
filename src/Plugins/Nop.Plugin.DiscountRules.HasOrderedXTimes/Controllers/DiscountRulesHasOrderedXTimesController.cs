using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.HasOrderedXTimes.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.DiscountRules.HasOrderedXTimes.Controllers
{
    [AdminAuthorize]
    public class DiscountRulesHasOrderedXTimesController : BasePluginController
    {
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        public DiscountRulesHasOrderedXTimesController(IDiscountService discountService,
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

            var model = new RequirementModel();
            model.RequirementId = discountRequirementId.HasValue ? discountRequirementId.Value : 0;
            model.DiscountId = discountId;
            model.OrderCount = 0;

            //add a prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("DiscountRulesCustomerRoles{0}", discountRequirementId.HasValue ? discountRequirementId.Value.ToString() : "0");

            return View("~/Plugins/DiscountRules.HasOrderedXTimes/Views/DiscountRulesHasOrderedXTimes/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult Configure(int discountId, int? discountRequirementId, int orderCount)
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
                _settingService.SetSetting(string.Format("DiscountRequirement.MustHaveOrderCount-{0}", discountRequirement.Id), orderCount);
            }
            else
            {
                //save new rule
                discountRequirement = new DiscountRequirement
                {
                    DiscountRequirementRuleSystemName = "DiscountRequirement.MustHaveOrderCount"
                };
                discount.DiscountRequirements.Add(discountRequirement);
                _discountService.UpdateDiscount(discount);

                _settingService.SetSetting(string.Format("DiscountRequirement.MustHaveOrderCount-{0}", discountRequirement.Id), orderCount);
            }
            return Json(new { Result = true, NewRequirementId = discountRequirement.Id }, JsonRequestBehavior.AllowGet);
        }
    }
}