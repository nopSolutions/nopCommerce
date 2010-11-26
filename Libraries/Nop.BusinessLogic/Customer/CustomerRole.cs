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

using System.Collections.Generic;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;

namespace NopSolutions.NopCommerce.BusinessLogic.CustomerManagement
{
    /// <summary>
    /// Represents a customer role
    /// </summary>
    public partial class CustomerRole : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        public int CustomerRoleId { get; set; }

        /// <summary>
        /// Gets or sets the customer role name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is marked as free shiping
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is marked as tax exempt
        /// </summary>
        public bool TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the customers of the role
        /// </summary>
        public List<Customer> Customers
        {
            get
            {
                return IoC.Resolve<ICustomerService>().GetCustomersByCustomerRoleId(this.CustomerRoleId);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the discounts
        /// </summary>
        public virtual ICollection<Discount> NpDiscounts { get; set; }
        /// <summary>
        /// Gets the customers
        /// </summary>
        public virtual ICollection<Customer> NpCustomers { get; set; }
        
        #endregion
    }

}