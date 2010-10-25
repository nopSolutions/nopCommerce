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
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Shipping rate computation method manager
    /// </summary>
    public partial interface IShippingRateComputationMethodManager
    {
        /// <summary>
        /// Deletes a shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethodId">Shipping rate computation method identifier</param>
        void DeleteShippingRateComputationMethod(int shippingRateComputationMethodId);

        /// <summary>
        /// Gets a shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethodId">Shipping rate computation method identifier</param>
        /// <returns>Shipping rate computation method</returns>
        ShippingRateComputationMethod GetShippingRateComputationMethodById(int shippingRateComputationMethodId);

        /// <summary>
        /// Gets all shipping rate computation methods
        /// </summary>
        /// <returns>Shipping rate computation method collection</returns>
        List<ShippingRateComputationMethod> GetAllShippingRateComputationMethods();

        /// <summary>
        /// Gets all shipping rate computation methods
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Shipping rate computation method collection</returns>
        List<ShippingRateComputationMethod> GetAllShippingRateComputationMethods(bool showHidden);

        /// <summary>
        /// Inserts a shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethod">Shipping rate computation method</param>
        void InsertShippingRateComputationMethod(ShippingRateComputationMethod shippingRateComputationMethod);

        /// <summary>
        /// Updates the shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethod">Shipping rate computation method</param>
        void UpdateShippingRateComputationMethod(ShippingRateComputationMethod shippingRateComputationMethod);
        
        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        /// <param name="shippingRateComputationMethodId">The shipping rate computation method identifier</param>
        /// <returns>A shipping rate computation method type</returns>
        ShippingRateComputationMethodTypeEnum GetShippingRateComputationMethodTypeEnum(int shippingRateComputationMethodId);
        
    }
}