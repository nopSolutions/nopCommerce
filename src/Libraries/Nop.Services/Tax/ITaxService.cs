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
// Contributor(s): Stephen Kennedy - VAT support, _______. 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial interface ITaxService
    {
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>Tax rate</returns>
        decimal GetTaxRate(ProductVariant productVariant, Customer customer);

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="customer">Customer</param>
        /// <returns>Tax rate</returns>
        decimal GetTaxRate(int taxCategoryId, Customer customer);
        
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="customer">Customer</param>
        /// <returns>Tax rate</returns>
        decimal GetTaxRate(ProductVariant productVariant, int taxCategoryId, 
            Customer customer);
        
        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetProductPrice(ProductVariant productVariant, decimal price,
            out decimal taxRate);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetProductPrice(ProductVariant productVariant, decimal price,
            Customer customer, out decimal taxRate);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetProductPrice(ProductVariant productVariant, decimal price,
            bool includingTax, Customer customer, out decimal taxRate);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetProductPrice(ProductVariant productVariant, int taxCategoryId, decimal price,
            bool includingTax, Customer customer,
            bool priceIncludesTax, out decimal taxRate);

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>VAT Number status</returns>
        VatNumberStatus GetVatNumberStatus(Country country,
            string vatNumber);
        
        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <param name="name">Name (if received)</param>
        /// <param name="address">Address (if received)</param>
        /// <returns>VAT Number status</returns>
        VatNumberStatus GetVatNumberStatus(Country country,
            string vatNumber, out string name, out string address);
    }
}
