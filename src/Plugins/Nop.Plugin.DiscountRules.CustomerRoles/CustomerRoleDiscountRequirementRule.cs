using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Discounts;

namespace Nop.Plugin.DiscountRules.CustomerRoles
{
    public partial class CustomerRoleDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public override string FriendlyName
        {
            get
            {
                return "Must be assigned to customer role";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public override string SystemName
        {
            get
            {
                return "DiscountRequirement.MustBeAssignedToCustomerRole";
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

            var customerRoles = request.Customer.CustomerRoles.Where(cr => cr.Active).ToList();
            if (customerRoles.Count == 0)
                return false;

            if (request.DiscountRequirement.RestrictedToCustomerRoles == null ||
                request.DiscountRequirement.RestrictedToCustomerRoles.Count == 0)
                return false;

            foreach (var restrCustomerRole in request.DiscountRequirement.RestrictedToCustomerRoles)
            {
                bool found = false;
                foreach (var customerRole in customerRoles)
                    if (restrCustomerRole == customerRole)
                        found = true;
                if (!found)
                    return false;
            }

            return true;
        }
    }
}