using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Abc.Plugin.DiscountRules.BuyOne50OffGrouped
{
    public class BuyOne50OffGroupedDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly IDiscountService _discountService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IUrlHelperFactory _urlHelperFactory;
        protected readonly IWebHelper _webHelper;

        public BuyOne50OffGroupedDiscountRequirementRule(IActionContextAccessor actionContextAccessor,
            IDiscountService discountService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            ISettingService settingService,
            IUrlHelperFactory urlHelperFactory,
            IWebHelper webHelper)
        {
            _actionContextAccessor = actionContextAccessor;
            _discountService = discountService;
            _localizationService = localizationService;
            _urlHelperFactory = urlHelperFactory;
            _webHelper = webHelper;
        }


        public async Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
        {
            //invalid by default
            var result = new DiscountRequirementValidationResult();

            if (request.Customer == null)
                return result;

            return result;
        }

        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            return urlHelper.Action("Configure", "BuyOne50OffGrouped",
                new { discountId = discountId, discountRequirementId = discountRequirementId }, _webHelper.GetCurrentRequestProtocol());
        }

        public override async Task InstallAsync()
        {
            // TODO: locales

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //discount requirements
            var discountRequirements = (await _discountService.GetAllDiscountRequirementsAsync())
                .Where(discountRequirement => discountRequirement.DiscountRequirementRuleSystemName == "DiscountRequirement.BuyOne50OffGrouped");
            foreach (var discountRequirement in discountRequirements)
            {
                await _discountService.DeleteDiscountRequirementAsync(discountRequirement, false);
            }

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.DiscountRules.CustomerRoles");

            await base.UninstallAsync();
        }
    }
}