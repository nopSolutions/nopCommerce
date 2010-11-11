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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// "ShippingByWeightAndCountry" service interface
    /// </summary>
    public partial interface IShippingByWeightAndCountryService
    {
        /// <summary>
        /// Gets a ShippingByWeightAndCountry
        /// </summary>
        /// <param name="shippingByWeightAndCountryId">ShippingByWeightAndCountry identifier</param>
        /// <returns>ShippingByWeightAndCountry</returns>
        ShippingByWeightAndCountry GetById(int shippingByWeightAndCountryId);

        /// <summary>
        /// Deletes a ShippingByWeightAndCountry
        /// </summary>
        /// <param name="shippingByWeightAndCountryId">ShippingByWeightAndCountry identifier</param>
        void DeleteShippingByWeightAndCountry(int shippingByWeightAndCountryId);

        /// <summary>
        /// Gets all ShippingByWeightAndCountrys
        /// </summary>
        /// <returns>ShippingByWeightAndCountry collection</returns>
        List<ShippingByWeightAndCountry> GetAll();

        /// <summary>
        /// Inserts a ShippingByWeightAndCountry
        /// </summary>
        /// <param name="shippingByWeightAndCountry">ShippingByWeightAndCountry</param>
        void InsertShippingByWeightAndCountry(ShippingByWeightAndCountry shippingByWeightAndCountry);

        /// <summary>
        /// Updates the ShippingByWeightAndCountry
        /// </summary>
        /// <param name="shippingByWeightAndCountry">ShippingByWeightAndCountry</param>
        void UpdateShippingByWeightAndCountry(ShippingByWeightAndCountry shippingByWeightAndCountry);

        /// <summary>
        /// Gets all ShippingByWeightAndCountrys by shipping method identifier
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <returns>ShippingByWeightAndCountry collection</returns>
        List<ShippingByWeightAndCountry> GetAllByShippingMethodIdAndCountryId(int shippingMethodId,
            int countryId);
                
        /// <summary>
        /// Gets or sets a value indicating whether to calculate per weight unit (e.g. per lb)
        /// </summary>
        bool CalculatePerWeightUnit {get;set;}

    }
}
