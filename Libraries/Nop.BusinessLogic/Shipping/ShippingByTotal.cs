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
using System.Collections.Generic;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Represents a ShippingByTotal
    /// </summary>
    public partial class ShippingByTotal : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ShippingByTotal class
        /// </summary>
        public ShippingByTotal()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the ShippingByTotal identifier
        /// </summary>
        public int ShippingByTotalId { get; set; }

        /// <summary>
        /// Gets or sets the shipping method identifier
        /// </summary>
        public int ShippingMethodId { get; set; }

        /// <summary>
        /// Gets or sets the "from" value
        /// </summary>
        public decimal From { get; set; }

        /// <summary>
        /// Gets or sets the "to" value
        /// </summary>
        public decimal To { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use percentage
        /// </summary>
        public bool UsePercentage { get; set; }

        /// <summary>
        /// Gets or sets the shipping charge percentage
        /// </summary>
        public decimal ShippingChargePercentage { get; set; }

        /// <summary>
        /// Gets or sets the shipping charge amount
        /// </summary>
        public decimal ShippingChargeAmount { get; set; }

        #endregion 

        #region Custom Properties
        /// <summary>
        /// Gets the shipping method
        /// </summary>
        public ShippingMethod ShippingMethod
        {
            get
            {
                return IoCFactory.Resolve<IShippingMethodManager>().GetShippingMethodById(this.ShippingMethodId);
            }
        }
        #endregion
    }

}
