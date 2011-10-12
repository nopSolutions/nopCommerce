using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product variant
    /// </summary>
    public partial class ProductVariant : BaseEntity, ILocalizedEntity
    {
        private ICollection<ProductVariantAttribute> _productVariantAttributes;
        private ICollection<ProductVariantAttributeCombination> _productVariantAttributeCombinations;
        private ICollection<TierPrice> _tierPrices;
        private ICollection<Discount> _appliedDiscounts;


        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public virtual int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU
        /// </summary>
        public virtual string Sku { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public virtual string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer part number
        /// </summary>
        public virtual string ManufacturerPartNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant is gift card
        /// </summary>
        public virtual bool IsGiftCard { get; set; }

        /// <summary>
        /// Gets or sets the gift card type identifier
        /// </summary>
        public virtual int GiftCardTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant requires that other product variants are added to the cart (Product X requires Product Y)
        /// </summary>
        public virtual bool RequireOtherProducts { get; set; }

        /// <summary>
        /// Gets or sets a required product variant identifiers (comma separated)
        /// </summary>
        public virtual string RequiredProductVariantIds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether required product variants are automatically added to the cart
        /// </summary>
        public virtual bool AutomaticallyAddRequiredProductVariants { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the product variant is download
        /// </summary>
        public virtual bool IsDownload { get; set; }

        /// <summary>
        /// Gets or sets the download identifier
        /// </summary>
        public virtual int DownloadId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this downloadable product can be downloaded unlimited number of times
        /// </summary>
        public virtual bool UnlimitedDownloads { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of downloads
        /// </summary>
        public virtual int MaxNumberOfDownloads { get; set; }

        /// <summary>
        /// Gets or sets the number of days during customers keeps access to the file.
        /// </summary>
        public virtual int? DownloadExpirationDays { get; set; }

        /// <summary>
        /// Gets or sets the download activation type
        /// </summary>
        public virtual int DownloadActivationTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant has a sample download file
        /// </summary>
        public virtual bool HasSampleDownload { get; set; }

        /// <summary>
        /// Gets or sets the sample download identifier
        /// </summary>
        public virtual int SampleDownloadId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product has user agreement
        /// </summary>
        public virtual bool HasUserAgreement { get; set; }

        /// <summary>
        /// Gets or sets the text of license agreement
        /// </summary>
        public virtual string UserAgreementText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant is recurring
        /// </summary>
        public virtual bool IsRecurring { get; set; }

        /// <summary>
        /// Gets or sets the cycle length
        /// </summary>
        public virtual int RecurringCycleLength { get; set; }

        /// <summary>
        /// Gets or sets the cycle period
        /// </summary>
        public virtual int RecurringCyclePeriodId { get; set; }

        /// <summary>
        /// Gets or sets the total cycles
        /// </summary>
        public virtual int RecurringTotalCycles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is ship enabled
        /// </summary>
        public virtual bool IsShipEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is free shipping
        /// </summary>
        public virtual bool IsFreeShipping { get; set; }

        /// <summary>
        /// Gets or sets the additional shipping charge
        /// </summary>
        public virtual decimal AdditionalShippingCharge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant is marked as tax exempt
        /// </summary>
        public virtual bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the tax category identifier
        /// </summary>
        public virtual int TaxCategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how to manage inventory
        /// </summary>
        public virtual int ManageInventoryMethodId { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
        public virtual int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display stock availability
        /// </summary>
        public virtual bool DisplayStockAvailability { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display stock quantity
        /// </summary>
        public virtual bool DisplayStockQuantity { get; set; }
        
        /// <summary>
        /// Gets or sets the minimum stock quantity
        /// </summary>
        public virtual int MinStockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the low stock activity identifier
        /// </summary>
        public virtual int LowStockActivityId { get; set; }

        /// <summary>
        /// Gets or sets the quantity when admin should be notified
        /// </summary>
        public virtual int NotifyAdminForQuantityBelow { get; set; }

        /// <summary>
        /// Gets or sets a value backorder mode identifier
        /// </summary>
        public virtual int BackorderModeId { get; set; }

        /// <summary>
        /// Gets or sets the order minimum quantity
        /// </summary>
        public virtual int OrderMinimumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the order maximum quantity
        /// </summary>
        public virtual int OrderMaximumQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable buy (Add to cart) button
        /// </summary>
        public virtual bool DisableBuyButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable "Add to wishlist" button
        /// </summary>
        public virtual bool DisableWishlistButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show "Call for Pricing" or "Call for quote" instead of price
        /// </summary>
        public virtual bool CallForPrice { get; set; }
        
        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public virtual decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the old price
        /// </summary>
        public virtual decimal OldPrice { get; set; }

        /// <summary>
        /// Gets or sets the product cost
        /// </summary>
        public virtual decimal ProductCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a customer enters price
        /// </summary>
        public virtual bool CustomerEntersPrice { get; set; }

        /// <summary>
        /// Gets or sets the minimum price entered by a customer
        /// </summary>
        public virtual decimal MinimumCustomerEnteredPrice { get; set; }

        /// <summary>
        /// Gets or sets the maximum price entered by a customer
        /// </summary>
        public virtual decimal MaximumCustomerEnteredPrice { get; set; }

        /// <summary>
        /// Gets or sets the weight
        /// </summary>
        public virtual decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public virtual decimal Length { get; set; }

        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public virtual decimal Width { get; set; }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public virtual decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public virtual int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the available start date and time
        /// </summary>
        public virtual DateTime? AvailableStartDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the shipped end date and time
        /// </summary>
        public virtual DateTime? AvailableEndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public virtual DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets the full product name
        /// </summary>
        public virtual string FullProductName
        {
            get
            {
                Product product = this.Product;
                if (product != null)
                {
                    if (!string.IsNullOrWhiteSpace(this.Name))
                        return product.Name + " (" + this.Name + ")";
                    return product.Name;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the backorder mode
        /// </summary>
        public virtual BackorderMode BackorderMode
        {
            get
            {
                return (BackorderMode)this.BackorderModeId;
            }
            set
            {
                this.BackorderModeId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the download activation type
        /// </summary>
        public virtual DownloadActivationType DownloadActivationType
        {
            get
            {
                return (DownloadActivationType)this.DownloadActivationTypeId;
            }
            set
            {
                this.DownloadActivationTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the gift card type
        /// </summary>
        public virtual GiftCardType GiftCardType
        {
            get
            {
                return (GiftCardType)this.GiftCardTypeId;
            }
            set
            {
                this.GiftCardTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the low stock activity
        /// </summary>
        public virtual LowStockActivity LowStockActivity
        {
            get
            {
                return (LowStockActivity)this.LowStockActivityId;
            }
            set
            {
                this.LowStockActivityId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating how to manage inventory
        /// </summary>
        public virtual ManageInventoryMethod ManageInventoryMethod
        {
            get
            {
                return (ManageInventoryMethod)this.ManageInventoryMethodId;
            }
            set
            {
                this.ManageInventoryMethodId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the cycle period for recurring products
        /// </summary>
        public virtual RecurringProductCyclePeriod RecurringCyclePeriod
        {
            get
            {
                return (RecurringProductCyclePeriod)this.RecurringCyclePeriodId;
            }
            set
            {
                this.RecurringCyclePeriodId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the product variant attributes
        /// </summary>
        public virtual ICollection<ProductVariantAttribute> ProductVariantAttributes
        {
            get { return _productVariantAttributes ?? (_productVariantAttributes = new List<ProductVariantAttribute>()); }
            protected set { _productVariantAttributes = value; }
        }

        /// <summary>
        /// Gets or sets the product variant attribute combinations
        /// </summary>
        public virtual ICollection<ProductVariantAttributeCombination> ProductVariantAttributeCombinations
        {
            get { return _productVariantAttributeCombinations ?? (_productVariantAttributeCombinations = new List<ProductVariantAttributeCombination>()); }
            protected set { _productVariantAttributeCombinations = value; }
        }

        /// <summary>
        /// Gets or sets the tier prices
        /// </summary>
        public virtual ICollection<TierPrice> TierPrices
        {
            get { return _tierPrices ?? (_tierPrices = new List<TierPrice>()); }
            protected set { _tierPrices = value; }
        }

        /// <summary>
        /// Gets or sets the collection of applied discounts
        /// </summary>
        public virtual ICollection<Discount> AppliedDiscounts
        {
            get { return _appliedDiscounts ?? (_appliedDiscounts = new List<Discount>()); }
            protected set { _appliedDiscounts = value; }
        }
    }
}
