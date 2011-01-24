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


using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Represents a request for getting shipping rate options
    /// </summary>
    public partial class GetShippingOptionRequest
    {
        public GetShippingOptionRequest()
        {
            this.Items = new List<ShoppingCartItem>();
        }

        /// <summary>
        /// Gets or sets a customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets a shopping cart items
        /// </summary>
        public IList<ShoppingCartItem> Items { get; set; }

        /// <summary>
        /// Gets or sets a shipping address
        /// </summary>
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// Shipped from country
        /// </summary>
        public Country CountryFrom { get; set; }

        /// <summary>
        /// Shipped from state/province
        /// </summary>
        public StateProvince StateProvinceFrom { get; set; }

        /// <summary>
        /// Shipped from zip/postal code
        /// </summary>
        public string ZipPostalCodeFrom { get; set; }

        #region Methods

        /// <summary>
        /// Gets total weight
        /// </summary>
        /// <returns>Total weight</returns>
        public decimal GetTotalWidth()
        {
            decimal totalWidth = decimal.Zero;
            foreach (var shoppingCartItem in this.Items)
            {
                var productVariant = shoppingCartItem.ProductVariant;
                if (productVariant != null)
                    totalWidth += productVariant.Width * shoppingCartItem.Quantity;
            }
            return totalWidth;
        }

        /// <summary>
        /// Gets total length
        /// </summary>
        /// <returns>Total length</returns>
        public decimal GetTotalLength()
        {
            decimal totalLength = decimal.Zero;
            foreach (var shoppingCartItem in this.Items)
            {
                var productVariant = shoppingCartItem.ProductVariant;
                if (productVariant != null)
                    totalLength += productVariant.Length * shoppingCartItem.Quantity;
            }
            return totalLength;
        }

        /// <summary>
        /// Gets total height
        /// </summary>
        /// <returns>Total height</returns>
        public decimal GetTotalHeight()
        {
            decimal totalHeight = decimal.Zero;
            foreach (var shoppingCartItem in this.Items)
            {
                var productVariant = shoppingCartItem.ProductVariant;
                if (productVariant != null)
                    totalHeight += productVariant.Height * shoppingCartItem.Quantity;
            }
            return totalHeight;
        }

        #endregion

    }
}
