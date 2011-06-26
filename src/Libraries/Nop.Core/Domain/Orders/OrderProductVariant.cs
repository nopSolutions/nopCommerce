using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order product variant
    /// </summary>
    public partial class OrderProductVariant : BaseEntity
    {
        private ICollection<GiftCard> _associatedGiftCards;

        /// <summary>
        /// Gets or sets the order product variant identifier
        /// </summary>
        public virtual Guid OrderProductVariantGuid { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public virtual int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public virtual int ProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public virtual int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (incl tax)
        /// </summary>
        public virtual decimal UnitPriceInclTax { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (excl tax)
        /// </summary>
        public virtual decimal UnitPriceExclTax { get; set; }

        /// <summary>
        /// Gets or sets the price in primary store currency (incl tax)
        /// </summary>
        public virtual decimal PriceInclTax { get; set; }

        /// <summary>
        /// Gets or sets the price in primary store currency (excl tax)
        /// </summary>
        public virtual decimal PriceExclTax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount (incl tax)
        /// </summary>
        public virtual decimal DiscountAmountInclTax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount (excl tax)
        /// </summary>
        public virtual decimal DiscountAmountExclTax { get; set; }

        /// <summary>
        /// Gets or sets the attribute description
        /// </summary>
        public virtual string AttributeDescription { get; set; }

        /// <summary>
        /// Gets or sets the product variant attributes in XML format
        /// </summary>
        public virtual string AttributesXml { get; set; }
        
        /// <summary>
        /// Gets or sets the download count
        /// </summary>
        public virtual int DownloadCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether download is activated
        /// </summary>
        public virtual bool IsDownloadActivated { get; set; }

        /// <summary>
        /// Gets or sets a license download identifier (in case this is a downloadable product)
        /// </summary>
        public virtual int? LicenseDownloadId { get; set; }
        
        /// <summary>
        /// Gets the order
        /// </summary>
        public virtual Order Order { get; set; }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public virtual ProductVariant ProductVariant { get; set; }

        /// <summary>
        /// Gets or sets the associated gift card
        /// </summary>
        public virtual ICollection<GiftCard> AssociatedGiftCards
        {
            get { return _associatedGiftCards ?? (_associatedGiftCards = new List<GiftCard>()); }
            protected set { _associatedGiftCards = value; }
        }
    }
}
