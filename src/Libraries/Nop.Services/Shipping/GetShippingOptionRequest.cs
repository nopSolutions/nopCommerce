using System.Collections.Generic;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;

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
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets a shopping cart items
        /// </summary>
        public virtual IList<ShoppingCartItem> Items { get; set; }

        /// <summary>
        /// Gets or sets a shipping address
        /// </summary>
        public virtual Address ShippingAddress { get; set; }

        /// <summary>
        /// Shipped from country
        /// </summary>
        public virtual Country CountryFrom { get; set; }

        /// <summary>
        /// Shipped from state/province
        /// </summary>
        public virtual StateProvince StateProvinceFrom { get; set; }

        /// <summary>
        /// Shipped from zip/postal code
        /// </summary>
        public virtual string ZipPostalCodeFrom { get; set; }

        #region Methods
        /// <summary>
        /// Get dimensions
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="length">Length</param>
        /// <param name="height">Height</param>
        public virtual void GetDimensions(out decimal width, out decimal length, out decimal height)
        {
            width = length = height = decimal.Zero;
            foreach (var shoppingCartItem in this.Items)
            {
                var productVariant = shoppingCartItem.ProductVariant;
                if (productVariant != null)
                {
                    width += productVariant.Width * shoppingCartItem.Quantity;
                    length += productVariant.Length * shoppingCartItem.Quantity;
                    height += productVariant.Height * shoppingCartItem.Quantity;
                }
            }
        }

        /// <summary>
        /// Gets total width
        /// </summary>
        /// <returns>Total width</returns>
        public virtual decimal GetTotalWidth()
        {
            decimal length, width, height = 0;
            GetDimensions(out width, out length, out height);
            return width;
        }

        /// <summary>
        /// Gets total length
        /// </summary>
        /// <returns>Total length</returns>
        public virtual decimal GetTotalLength()
        {
            decimal length, width, height = 0;
            GetDimensions(out width, out length, out height);
            return length;
        }

        /// <summary>
        /// Gets total height
        /// </summary>
        /// <returns>Total height</returns>
        public virtual decimal GetTotalHeight()
        {
            decimal length, width, height = 0;
            GetDimensions(out width, out length, out height);
            return height;
        }

        #endregion

    }
}
