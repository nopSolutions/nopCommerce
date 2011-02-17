using System;
using Nop.Core;
using Nop.Services.Discounts;

namespace Nop.Plugin.DiscountRules.BillingCountry
{
    public partial class BillingCountryDiscountRequirementRule : IDiscountRequirementRule
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public string FriendlyName
        {
            get
            {
                //TODO localize
                return "Billing country is";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public string SystemName
        {
            get
            {
                return "BillingCountryIs";
            }
        }

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

            if (request.Customer.BillingAddress == null)
                return false;

            if (request.DiscountRequirement.BillingCountryId == 0)
                return false;

            bool result = request.Customer.BillingAddress.CountryId == request.DiscountRequirement.BillingCountryId;
            return result;
        }
    }
}