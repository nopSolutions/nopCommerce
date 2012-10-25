using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.HadSpentAmount.Models;
using Nop.Services.Discounts;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.DiscountRules.HadSpentAmount.Controllers
{
    [AdminAuthorize]
    public class DiscountRulesHadSpentAmountController : Controller
    {
        private readonly IDiscountService _discountService;

        public DiscountRulesHadSpentAmountController(IDiscountService discountService)
        {
            this._discountService = discountService;
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

            var model = new RequirementModel();
            model.RequirementId = discountRequirementId.HasValue ? discountRequirementId.Value : 0;
            model.DiscountId = discountId;
            model.SpentAmount = discountRequirement != null ? discountRequirement.SpentAmount : decimal.Zero;

            //add a prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("DiscountRulesHadSpentAmount{0}", discountRequirementId.HasValue ? discountRequirementId.Value.ToString() : "0");

            return View("Nop.Plugin.DiscountRules.HadSpentAmount.Views.DiscountRulesHadSpentAmount.Configure", model);
        }

        [HttpPost]
        public ActionResult Configure(int discountId, int? discountRequirementId, decimal spentAmount)
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
                discountRequirement.SpentAmount = spentAmount;
                _discountService.UpdateDiscount(discount);
            }
            else
            {
                //save new rule
                discountRequirement = new DiscountRequirement()
                {
                    DiscountRequirementRuleSystemName = "DiscountRequirement.HadSpentAmount",
                    SpentAmount = spentAmount,
                };
                discount.DiscountRequirements.Add(discountRequirement);
                _discountService.UpdateDiscount(discount);
            }
            return Json(new { Result = true, NewRequirementId = discountRequirement.Id }, JsonRequestBehavior.AllowGet);
        }
        
    }
}