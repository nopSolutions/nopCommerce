//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using Nop.Core;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts
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
                throw new NopException("Customer is not set");
            
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
    }
}