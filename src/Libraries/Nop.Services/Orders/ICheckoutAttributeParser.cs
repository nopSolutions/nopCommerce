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
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Checkout attribute parser interface
    /// </summary>
    public partial interface ICheckoutAttributeParser
    {
        /// <summary>
        /// Gets selected checkout attribute identifiers
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected checkout attribute identifiers</returns>
        IList<int> ParseCheckoutAttributeIds(string attributes);

        /// <summary>
        /// Gets selected checkout attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected checkout attributes</returns>
        IList<CheckoutAttribute> ParseCheckoutAttributes(string attributes);

        /// <summary>
        /// Get checkout attribute values
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Checkout attribute values</returns>
        IList<CheckoutAttributeValue> ParseCheckoutAttributeValues(string attributes);

        /// <summary>
        /// Gets selected checkout attribute value
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute value</returns>
        IList<string> ParseValues(string attributes, int checkoutAttributeId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddCheckoutAttribute(string attributes, CheckoutAttribute ca, string value);
    }
}
