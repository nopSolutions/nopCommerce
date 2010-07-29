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
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Tax;

namespace NopSolutions.NopCommerce.Tax
{
    /// <summary>
    /// Free tax provider
    /// </summary>
    public class FixedRateTaxProvider : ITaxProvider
    {
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <param name="error">Error</param>
        /// <returns>Tax</returns>
        public decimal GetTaxRate(CalculateTaxRequest calculateTaxRequest, ref string error)
        {
            decimal taxRate = decimal.Zero;

            int taxClassID = 0;
            if (calculateTaxRequest.TaxClassId > 0)
            {
                taxClassID = calculateTaxRequest.TaxClassId;
            }
            else
            {
                var productVariant = calculateTaxRequest.Item;
                if (productVariant != null)
                {
                    taxClassID = productVariant.TaxCategoryId;
                }
            }
            taxRate = GetTaxRate(taxClassID);

            return taxRate;
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="taxCategoryID">The tax category identifier</param>
        /// <returns>Tax rate</returns>
        protected decimal GetTaxRate(int taxCategoryID)
        {
            decimal rate = SettingManager.GetSettingValueDecimalNative(string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", taxCategoryID));
            return rate;
        }
    }
}
