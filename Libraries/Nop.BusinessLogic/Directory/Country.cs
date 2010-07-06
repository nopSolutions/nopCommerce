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
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;



namespace NopSolutions.NopCommerce.BusinessLogic.Directory
{
    /// <summary>
    /// Represents a country
    /// </summary>
    public partial class Country : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the Country class
        /// </summary>
        public Country()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether registration is allowed to this country
        /// </summary>
        public bool AllowsRegistration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether billing is allowed to this country
        /// </summary>
        public bool AllowsBilling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shipping is allowed to this country
        /// </summary>
        public bool AllowsShipping { get; set; }

        /// <summary>
        /// Gets or sets the two letter ISO code
        /// </summary>
        public string TwoLetterIsoCode { get; set; }

        /// <summary>
        /// Gets or sets the three letter ISO code
        /// </summary>
        public string ThreeLetterIsoCode { get; set; }

        /// <summary>
        /// Gets or sets the numeric ISO code
        /// </summary>
        public int NumericIsoCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the state/provinces
        /// </summary>
        public List<StateProvince> StateProvinces
        {
            get
            {
                return StateProvinceManager.GetStateProvincesByCountryId(this.CountryId);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the state/provinces
        /// </summary>
        public virtual ICollection<StateProvince> NpStateProvinces { get; set; }

        /// <summary>
        /// Gets the restricted payment methods
        /// </summary>
        public virtual ICollection<PaymentMethod> NpRestrictedPaymentMethods { get; set; }

        /// <summary>
        /// Gets the restricted shipping methods
        /// </summary>
        public virtual ICollection<ShippingMethod> NpRestrictedShippingMethods { get; set; }

        #endregion
    }

}
