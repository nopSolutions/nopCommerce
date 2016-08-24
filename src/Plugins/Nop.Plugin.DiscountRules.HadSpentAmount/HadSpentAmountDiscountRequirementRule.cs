using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;

namespace Nop.Plugin.DiscountRules.HadSpentAmount
{
    public partial class HadSpentAmountDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;

        public HadSpentAmountDiscountRequirementRule(ILocalizationService localizationService,
            ISettingService settingService, 
            IOrderService orderService)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._orderService = orderService;
        }

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>Result</returns>
        public DiscountRequirementValidationResult CheckRequirement(DiscountRequirementValidationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            //invalid by default
            var result = new DiscountRequirementValidationResult();

            var spentAmountRequirement = _settingService.GetSettingByKey<decimal>(string.Format("DiscountRequirement.HadSpentAmount-{0}", request.DiscountRequirementId));
            if (spentAmountRequirement == decimal.Zero)
            {
                //valid
                result.IsValid = true;
                return result;
            }

            if (request.Customer == null || request.Customer.IsGuest())
                return result;
            var orders = _orderService.SearchOrders(storeId: request.Store.Id, 
                customerId: request.Customer.Id,
                osIds: new List<int>() { (int)OrderStatus.Complete });
            decimal spentAmount = orders.Sum(o => o.OrderTotal);
            if (spentAmount > spentAmountRequirement)
            {
                result.IsValid = true;
            }
            else
            {
                result.UserError = _localizationService.GetResource("Plugins.DiscountRules.HadSpentAmount.NotEnough");
            }
            return result;
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            //configured in RouteProvider.cs
            string result = "Plugins/DiscountRulesHadSpentAmount/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);
            return result;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HadSpentAmount.Fields.Amount", "Required spent amount");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HadSpentAmount.Fields.Amount.Hint", "Discount will be applied if customer has spent/purchased x.xx amount.");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HadSpentAmount.NotEnough", "Sorry, this offer requires more money spent (previously placed orders)");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HadSpentAmount.Fields.Amount");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HadSpentAmount.Fields.Amount.Hint");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HadSpentAmount.NotEnough");
            base.Uninstall();
        }
    }
}