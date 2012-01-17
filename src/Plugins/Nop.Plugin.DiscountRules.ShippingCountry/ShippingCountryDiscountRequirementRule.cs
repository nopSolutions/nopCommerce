using System;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Discounts;
using Nop.Services.Localization;

namespace Nop.Plugin.DiscountRules.ShippingCountry
{
    public partial class ShippingCountryDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>true - requirement is met; otherwise, false</returns>
        public bool CheckRequirement(CheckDiscountRequirementRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.DiscountRequirement == null)
                throw new NopException("Discount requirement is not set");

            if (request.Customer == null)
                return false;

            if (request.Customer.ShippingAddress == null)
                return false;

            if (request.DiscountRequirement.ShippingCountryId == 0)
                return false;

            bool result = request.Customer.ShippingAddress.CountryId == request.DiscountRequirement.ShippingCountryId;
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
            string result = "Plugins/DiscountRulesShippingCountry/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);
            return result;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.SelectCountry", "Select country");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.Country", "Shipping country");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.Country.Hint", "Select required shipping country.");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.SelectCountry");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.Country.Hint");
            base.Uninstall();
        }
    }
}