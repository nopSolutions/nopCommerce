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
using Nop.Core.Domain;

namespace Nop.Services
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class ProductAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this product variant attribute should have values
        /// </summary>
        /// <param name="productVariantAttribute">Product variant attribute</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                return false;

            if (productVariantAttribute.AttributeControlType == AttributeControlTypeEnum.TextBox ||
                productVariantAttribute.AttributeControlType == AttributeControlTypeEnum.MultilineTextbox ||
                productVariantAttribute.AttributeControlType == AttributeControlTypeEnum.Datepicker)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
