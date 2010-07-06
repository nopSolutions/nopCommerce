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

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Represents the types of price adjustments can be made (absolute adjustment, relative adjustment, absolute price)
    /// </summary>
    public enum PriceAdjustmentTypeEnum : int
    {
        /// <summary>
        /// Provided adjustment must be between 0 and 100, price will be calculated ((100 - x) / 100) * price
        /// </summary>
        RelativeAdjustment = 0, 

        /// <summary>
        /// Provided adjustment can be any positive decimal value, price will be calculated price - adjustment (result must be >= 0)
        /// </summary>
        AbsoluteAdjustment = 1, 

        /// <summary>
        /// Provided value is an absolute price, price will not be calculated
        /// </summary>
        AbsolutePrice = 2
    }
}
