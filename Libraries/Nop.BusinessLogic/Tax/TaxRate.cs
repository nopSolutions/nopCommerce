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

using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Tax
{
    /// <summary>
    /// Represents a tax rate
    /// </summary>
    public partial class TaxRate : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the TaxRate class
        /// </summary>
        public TaxRate()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the tax rate identifier
        /// </summary>
        public int TaxRateId { get; set; }

        /// <summary>
        /// Gets or sets the tax category identifier
        /// </summary>
        public int TaxCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state/province identifier
        /// </summary>
        public int StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the zip
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// Gets or sets the percentage
        /// </summary>
        public decimal Percentage { get; set; }

        #endregion 

        #region Custom Properties

        /// <summary>
        /// Gets the tax category
        /// </summary>
        public TaxCategory TaxCategory
        {
            get
            {
                return IoC.Resolve<ITaxCategoryService>().GetTaxCategoryById(this.TaxCategoryId);
            }
        }

        /// <summary>
        /// Gets the country
        /// </summary>
        public Country Country
        {
            get
            {
                return IoC.Resolve<ICountryService>().GetCountryById(this.CountryId);
            }
        }

        /// <summary>
        /// Gets the state/province
        /// </summary>
        public StateProvince StateProvince
        {
            get
            {
                return IoC.Resolve<IStateProvinceService>().GetStateProvinceById(this.StateProvinceId);
            }
        }

        #endregion 
    }

}
