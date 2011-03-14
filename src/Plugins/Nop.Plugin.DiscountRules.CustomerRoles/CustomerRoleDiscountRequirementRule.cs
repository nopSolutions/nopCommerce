using System;
using System.Security.Principal;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Discounts;

namespace Nop.Plugin.DiscountRules.CustomerRoles
{
    public partial class CustomerRoleDiscountRequirementRule : IDiscountRequirementRule
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public string FriendlyName
        {
            get
            {
                //TODO localize
                return "Must be assigned to customer role";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public string SystemName
        {
            get
            {
                return "MustBeAssignedToCustomerRole";
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
            
            var customerRoles = request.Customer.CustomerRoles; //TODO use ICustomerService.GetCustomerRolesByCustomerId
            if (customerRoles == null ||
                customerRoles.Count == 0)
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