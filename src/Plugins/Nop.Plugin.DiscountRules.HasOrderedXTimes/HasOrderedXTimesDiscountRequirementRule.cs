using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Plugins;
using Nop.Services.Discounts;

namespace Nop.Plugin.DiscountRules.HasOrderedXTimes
{
    public class HasOrderedXTimesDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        public DiscountRequirementValidationResult CheckRequirement(DiscountRequirementValidationRequest request)
        {
            throw new NotImplementedException();
        }

        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            //configured in RouteProvider.cs
            string result = "Plugins/DiscountRulesHasOrderedXTimes/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);
            return result;
        }

    }
}
