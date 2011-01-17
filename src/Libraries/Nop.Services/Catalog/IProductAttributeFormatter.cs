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

using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute formatter interface
    /// </summary>
    public partial interface IProductAttributeFormatter
    {
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="attributes">Attributes</param>
        /// <returns>Attributes</returns>
        string FormatAttributes(ProductVariant productVariant, string attributes);

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="attributes">Attributes</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <returns>Attributes</returns>
        string FormatAttributes(ProductVariant productVariant, string attributes,
            Customer customer, string serapator);

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="attributes">Attributes</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>Attributes</returns>
        string FormatAttributes(ProductVariant productVariant, string attributes,
            Customer customer, string serapator, bool htmlEncode);

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="attributes">Attributes</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <returns>Attributes</returns>
        string FormatAttributes(ProductVariant productVariant, string attributes,
            Customer customer, string serapator, bool htmlEncode, bool renderPrices);

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="attributes">Attributes</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="renderProductAttributes">A value indicating whether to render product attributes</param>
        /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
        /// <returns>Attributes</returns>
        string FormatAttributes(ProductVariant productVariant, string attributes,
            Customer customer, string serapator, bool htmlEncode, bool renderPrices,
            bool renderProductAttributes, bool renderGiftCardAttributes);
    }
}
