using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using System;
using System.Linq;

namespace Nop.Plugin.DiscountRules.WeightInCart
{
    public partial class WeightInCartDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly ISettingService _settingService;

        public WeightInCartDiscountRequirementRule(ISettingService settingService)
        {
            this._settingService = settingService;
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

            var weightInCartRange = _settingService.GetSettingByKey<string>(string.Format("DiscountRequirement.WeightInCartRange-{0}", request.DiscountRequirementId));
            if (string.IsNullOrWhiteSpace(weightInCartRange))
            {
                //valid
                result.IsValid = true;
                return result;
            }

            if (request.Customer == null)
                return result;

            var arrWeightInRange = weightInCartRange.Split('-');
            var minWeight = Convert.ToDecimal(arrWeightInRange[0]);
            var maxWeight = Convert.ToDecimal(arrWeightInRange[1]);

            var cart = request.Customer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart);
            var totalWeightInCart = decimal.Zero;

            foreach (var sci in cart)
            {
                //Present in appliedProducts
                if (sci.Product.AppliedDiscounts.SelectMany(x => x.DiscountRequirements).FirstOrDefault(i => i.Id == request.DiscountRequirementId) != null)
                    totalWeightInCart += sci.Product.Weight;
            }

            result.IsValid = totalWeightInCart >= minWeight && totalWeightInCart < maxWeight;
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
            string result = "Plugins/DiscountRulesWeightInCart/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);
            return result;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.WeightInCart.Fields.WeightRange", "Vul gewicht range in.", "nl-NL");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.WeightInCart.Fields.WeightRange.Hint", "Specificeer de range in gescheiden door een min-teken, zonder spaties. {Min gewicht}-{Max gewicht}. bijvoorbeeld, 50-60 of 80-100.", "nl-NL");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.WeightInCart.Fields.WeightRange", "Enter weight range.", "en-US");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.WeightInCart.Fields.WeightRange.Hint", "Specify the range with a hyphen, without any spaces. {Min weight}-{Max weight}. for example, 50-60 or 80-100.", "en-US");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.WeightInCart.Fields.WeightRange");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.WeightInCart.Fields.WeightRange.Hint");
            base.Uninstall();
        }
    }
}