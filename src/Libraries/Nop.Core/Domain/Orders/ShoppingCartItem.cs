using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a shopping cart item
    /// </summary>
    public partial class ShoppingCartItem : BaseEntity
    {
        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart type identifier
        /// </summary>
        public int ShoppingCartTypeId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

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
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }
        
        /// <summary>
        /// Gets the log type
        /// </summary>
        public ShoppingCartType ShoppingCartType
        {
            get
            {
                return (ShoppingCartType)this.ShoppingCartTypeId;
            }
            set
            {
                this.ShoppingCartTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the product variant
        /// </summary>
        public virtual ProductVariant ProductVariant { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets a value indicating whether the shopping cart item is free shipping
        /// </summary>
        public bool IsFreeShipping
        {
            get
            {
                var productVariant = this.ProductVariant;
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
                var productVariant = this.ProductVariant;
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
                var productVariant = this.ProductVariant;
                if (productVariant != null)
                    additionalShippingCharge = productVariant.AdditionalShippingCharge * Quantity;
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
                var productVariant = this.ProductVariant;
                if (productVariant != null)
                    return productVariant.IsTaxExempt;
                return false;
            }
        }
    }
}
