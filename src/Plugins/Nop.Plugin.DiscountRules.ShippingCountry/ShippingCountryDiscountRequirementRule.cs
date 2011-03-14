using System;
using System.Security.Principal;
using Nop.Core;
using Nop.Core.Domain.Discounts;
using Nop.Core.Plugins;
using Nop.Services.Discounts;

namespace Nop.Plugin.DiscountRules.ShippingCountry
{
    public partial class ShippingCountryDiscountRequirementRule : IDiscountRequirementRule
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public string FriendlyName
        {
            get
            {
                //TODO localize
                return "Shipping country is";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public string SystemName
        {
            get
            {
                return "ShippingCountryIs";
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

            if (request.Customer.ShippingAddress == null)
                return false;

            if (request.DiscountRequirement.ShippingCountryId == 0)
                return false;

            bool result = request.Customer.ShippingAddress.CountryId == request.DiscountRequirement.ShippingCountryId;
            return result;
        }

        #region IPlugin Members

        public string Name
        {
            get { return FriendlyName; }
        }

        public int SortOrder
        {
            get { return 1; }
        }

        public bool IsAuthorized(IPrincipal user)
        {
            return true;
        }

        public int CompareTo(IPlugin other)
        {
            return SortOrder - other.SortOrder;
        }
        #endregion
    }
}