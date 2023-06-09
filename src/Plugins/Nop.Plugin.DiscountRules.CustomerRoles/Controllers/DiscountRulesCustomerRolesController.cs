using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.CustomerRoles.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class DiscountRulesCustomerRolesController : BasePluginController
    {
        #region Fields

        protected readonly ICustomerService _customerService;
        protected readonly IDiscountService _discountService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IPermissionService _permissionService;
        protected readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public DiscountRulesCustomerRolesController(ICustomerService customerService,
            IDiscountService discountService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            _customerService = customerService;
            _discountService = discountService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get errors message from model state
        /// </summary>
        /// <param name="modelState">Model state</param>
        /// <returns>Errors message</returns>
        protected IEnumerable<string> GetErrorsFromModelState(ModelStateDictionary modelState)
        {
            return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure(int discountId, int? discountRequirementId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");

            //load the discount
            var discount = await _discountService.GetDiscountByIdAsync(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            //check whether the discount requirement exists
            if (discountRequirementId.HasValue && await _discountService.GetDiscountRequirementByIdAsync(discountRequirementId.Value) is null)
                return Content("Failed to load requirement.");

            //try to get previously saved restricted customer role identifier
            var restrictedRoleId = await _settingService.GetSettingByKeyAsync<int>(string.Format(DiscountRequirementDefaults.SettingsKey, discountRequirementId ?? 0));

            var model = new RequirementModel
            {
                RequirementId = discountRequirementId ?? 0,
                DiscountId = discountId,
                CustomerRoleId = restrictedRoleId
            };

            //set available customer roles
            model.AvailableCustomerRoles = (await _customerService.GetAllCustomerRolesAsync(true)).Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString(),
                Selected = role.Id == restrictedRoleId
            }).ToList();
            model.AvailableCustomerRoles.Insert(0, new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRole.Select"),
                Value = "0"
            });

            //set the HTML field prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format(DiscountRequirementDefaults.HtmlFieldPrefix, discountRequirementId ?? 0);

            return View("~/Plugins/DiscountRules.CustomerRoles/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(RequirementModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");

            if (ModelState.IsValid)
            {
                //load the discount
                var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId);
                if (discount == null)
                    return NotFound(new { Errors = new[] { "Discount could not be loaded" } });

                //get the discount requirement
                var discountRequirement = await _discountService.GetDiscountRequirementByIdAsync(model.RequirementId);

                //the discount requirement does not exist, so create a new one
                if (discountRequirement == null)
                {
                    discountRequirement = new DiscountRequirement
                    {
                        DiscountId = discount.Id,
                        DiscountRequirementRuleSystemName = DiscountRequirementDefaults.SystemName
                    };

                    await _discountService.InsertDiscountRequirementAsync(discountRequirement);
                }

                //save restricted customer role identifier
                await _settingService.SetSettingAsync(string.Format(DiscountRequirementDefaults.SettingsKey, discountRequirement.Id), model.CustomerRoleId);

                return Ok(new { NewRequirementId = discountRequirement.Id });
            }

            return Ok(new { Errors = GetErrorsFromModelState(ModelState) });
        }

        #endregion
    }
}