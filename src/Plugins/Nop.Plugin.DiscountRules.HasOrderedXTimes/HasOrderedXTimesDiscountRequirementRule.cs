using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Orders;

namespace Nop.Plugin.DiscountRules.HasOrderedXTimes
{
    public class HasOrderedXTimesDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;

        public HasOrderedXTimesDiscountRequirementRule(IOrderService orderService, ISettingService settingService)
        {
            this._orderService = orderService;
            this._settingService = settingService;
        }

        public DiscountRequirementValidationResult CheckRequirement(DiscountRequirementValidationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            //invalid
            var result = new DiscountRequirementValidationResult();

            var orderRequirement =
                _settingService.GetSettingByKey<int>(string.Format("DiscountRequirement.MustHaveOrderCount-{0}",
                    request.DiscountRequirementId));
            var comparisonOperator = _settingService.GetSettingByKey<ComparisonOperators>(string.Format("DiscountRequirement.MustHaveOrderCount-comparisonOperator-{0}", request.DiscountRequirementId));

            if (orderRequirement == 0)
            {
                result.IsValid = true;
                return result;
            }

            //Not valid for guest accounts, since order count can not be tracked. 
            if (request.Customer == null || request.Customer.IsGuest())
            {
                return result;
            }

            var orders = _orderService.SearchOrders(storeId: request.Store.Id, customerId: request.Customer.Id,
                osIds: new List<int>() { (int)OrderStatus.Complete });
            var orderCount = orders.Count;

            if (Comparison(orderRequirement, orderCount, comparisonOperator))
            {
                result.IsValid = true;
            }
                //TODO This should be localized resource.
            else
            {
                result.UserError = "Invalid order requirement";
            }

            return result;
        }

        private bool Comparison(int orderRequirement, int orderCount, ComparisonOperators comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperators.EqualTo:
                    return orderRequirement == orderCount;
                case ComparisonOperators.GreaterThan:
                    return orderCount > orderRequirement;
                case ComparisonOperators.GreaterThanOrEqualTo:
                    return orderCount >= orderRequirement;
                case ComparisonOperators.LessThan:
                    return orderCount < orderRequirement;
                case ComparisonOperators.LessThanOrEqualTo:
                    return orderCount <= orderRequirement;
                default:
                    return false;
            }
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
