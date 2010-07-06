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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;


namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents a shopping cart item
    /// </summary>
    public partial class ShoppingCartItem : BaseEntity
    {
        #region Fields
        private ProductVariant _cachedProductVariant;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the shopping cart class
        /// </summary>
        public ShoppingCartItem()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the shopping cart item identifier
        /// </summary>
        public int ShoppingCartItemId { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart type identifier
        /// </summary>
        public int ShoppingCartTypeId { get; set; }

        /// <summary>
        /// Gets or sets the customer session identifier
        /// </summary>
        public Guid CustomerSessionGuid { get; set; }

        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public int ProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the product variant attributes
        /// </summary>
        public string AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the price enter by a customer
        /// </summary>
        public decimal CustomerEnteredPrice { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOn { get; set; }
        #endregion 

        #region Custom Properties

        /// <summary>
        /// Gets the log type
        /// </summary>
        public ShoppingCartTypeEnum ShoppingCartType
        {
            get
            {
                return (ShoppingCartTypeEnum)this.ShoppingCartTypeId;
            }
        }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public ProductVariant ProductVariant
        {
            get
            {
                if (_cachedProductVariant == null)
                {
                    _cachedProductVariant = ProductManager.GetProductVariantById(this.ProductVariantId);
                }
                return _cachedProductVariant;
            }
        }

        /// <summary>
        /// Gets the total weight
        /// </summary> 
        public decimal TotalWeight
        {
            get
            {
                decimal totalWeight = decimal.Zero;
                ProductVariant productVariant = this.ProductVariant;
                if (productVariant != null)
                {
                    decimal attributesTotalWeight = decimal.Zero;

                    var pvaValues = ProductAttributeHelper.ParseProductVariantAttributeValues(this.AttributesXml);
                    foreach (ProductVariantAttributeValue pvaValue in pvaValues)
                    {
                        attributesTotalWeight += pvaValue.WeightAdjustment;
                    }
                    decimal unitWeight = productVariant.Weight + attributesTotalWeight;
                    totalWeight = unitWeight * Quantity;
                }
                return totalWeight;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the shopping cart item is free shipping
        /// </summary>
        public bool IsFreeShipping
        {
            get
            {
                ProductVariant productVariant = this.ProductVariant;
                if (productVariant != null)
                    return productVariant.IsFreeShipping;
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the shopping cart item is ship enabled
        /// </summary>
        public bool IsShipEnabled
        {
            get
            {
                ProductVariant productVariant = this.ProductVariant;
                if (productVariant != null)
                    return productVariant.IsShipEnabled;
                return false;
            }
        }

        /// <summary>
        /// Gets the additional shipping charge
        /// </summary> 
        public decimal AdditionalShippingCharge
        {
            get
            {
                decimal additionalShippingCharge = decimal.Zero;
                ProductVariant productVariant = this.ProductVariant;
                if (productVariant != null)
                {
                    additionalShippingCharge = productVariant.AdditionalShippingCharge * Quantity;
                }
                return additionalShippingCharge;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the shopping cart item is tax exempt
        /// </summary>
        public bool IsTaxExempt
        {
            get
            {
                ProductVariant productVariant = this.ProductVariant;
                if (productVariant != null)
                    return productVariant.IsTaxExempt;
                return false;
            }
        }

        /// <summary>
        /// Gets a customer session of cart item
        /// </summary>
        public CustomerSession CustomerSession
        {
            get
            {
                return CustomerManager.GetCustomerSessionByGuid(this.CustomerSessionGuid);
            }
        }
        #endregion
    }
}
