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
using NopSolutions.NopCommerce.BusinessLogic.Directory;

namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Represents a shipping method
    /// </summary>
    public partial class ShippingMethod : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the shipping method identifier
        /// </summary>
        public int ShippingMethodId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the restricted countries
        /// </summary>
        public virtual ICollection<Country> NpRestrictedCountries { get; set; }

        #endregion
    }
}