using Nop.Core.Domain.Common;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents a product
/// </summary>
public partial class Product : BaseEntity, ILocalizedEntity, ISlugSupported, IAclSupported, IStoreMappingSupported, IDiscountSupported<DiscountProductMapping>, ISoftDeletedEntity
{
    /// <summary>
    /// Gets or sets the product type identifier
    /// </summary>
    public int ProductTypeId { get; set; }

    /// <summary>
    /// Gets or sets the parent product identifier. It's used to identify associated products (only with "grouped" products)
    /// </summary>
    public int ParentGroupedProductId { get; set; }

    /// <summary>
    /// Gets or sets the values indicating whether this product is visible in catalog or search results.
    /// It's used when this product is associated to some "grouped" one
    /// This way associated products could be accessed/added/etc only from a grouped product details page
    /// </summary>
    public bool VisibleIndividually { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the short description
    /// </summary>
    public string ShortDescription { get; set; }

    /// <summary>
    /// Gets or sets the full description
    /// </summary>
    public string FullDescription { get; set; }

    /// <summary>
    /// Gets or sets the admin comment
    /// </summary>
    public string AdminComment { get; set; }

    /// <summary>
    /// Gets or sets a value of used product template identifier
    /// </summary>
    public int ProductTemplateId { get; set; }

    /// <summary>
    /// Gets or sets a vendor identifier
    /// </summary>
    public int VendorId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the product on home page
    /// </summary>
    public bool ShowOnHomepage { get; set; }

    /// <summary>
    /// Gets or sets the meta keywords
    /// </summary>
    public string MetaKeywords { get; set; }

    /// <summary>
    /// Gets or sets the meta description
    /// </summary>
    public string MetaDescription { get; set; }

    /// <summary>
    /// Gets or sets the meta title
    /// </summary>
    public string MetaTitle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product allows customer reviews
    /// </summary>
    public bool AllowCustomerReviews { get; set; }

    /// <summary>
    /// Gets or sets the rating sum (approved reviews)
    /// </summary>
    public int ApprovedRatingSum { get; set; }

    /// <summary>
    /// Gets or sets the rating sum (not approved reviews)
    /// </summary>
    public int NotApprovedRatingSum { get; set; }

    /// <summary>
    /// Gets or sets the total rating votes (approved reviews)
    /// </summary>
    public int ApprovedTotalReviews { get; set; }

    /// <summary>
    /// Gets or sets the total rating votes (not approved reviews)
    /// </summary>
    public int NotApprovedTotalReviews { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is subject to ACL
    /// </summary>
    public bool SubjectToAcl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }

    /// <summary>
    /// Gets or sets the SKU
    /// </summary>
    public string Sku { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer part number
    /// </summary>
    public string ManufacturerPartNumber { get; set; }

    /// <summary>
    /// Gets or sets the Global Trade Item Number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).
    /// </summary>
    public string Gtin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is gift card
    /// </summary>
    public bool IsGiftCard { get; set; }

    /// <summary>
    /// Gets or sets the gift card type identifier
    /// </summary>
    public int GiftCardTypeId { get; set; }

    /// <summary>
    /// Gets or sets gift card amount that can be used after purchase. If not specified, then product price will be used.
    /// </summary>
    public decimal? OverriddenGiftCardAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product requires that other products are added to the cart (Product X requires Product Y)
    /// </summary>
    public bool RequireOtherProducts { get; set; }

    /// <summary>
    /// Gets or sets a required product identifiers (comma separated)
    /// </summary>
    public string RequiredProductIds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether required products are automatically added to the cart
    /// </summary>
    public bool AutomaticallyAddRequiredProducts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is download
    /// </summary>
    public bool IsDownload { get; set; }

    /// <summary>
    /// Gets or sets the download identifier
    /// </summary>
    public int DownloadId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this downloadable product can be downloaded unlimited number of times
    /// </summary>
    public bool UnlimitedDownloads { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of downloads
    /// </summary>
    public int MaxNumberOfDownloads { get; set; }

    /// <summary>
    /// Gets or sets the number of days during customers keeps access to the file.
    /// </summary>
    public int? DownloadExpirationDays { get; set; }

    /// <summary>
    /// Gets or sets the download activation type
    /// </summary>
    public int DownloadActivationTypeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product has a sample download file
    /// </summary>
    public bool HasSampleDownload { get; set; }

    /// <summary>
    /// Gets or sets the sample download identifier
    /// </summary>
    public int SampleDownloadId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product has user agreement
    /// </summary>
    public bool HasUserAgreement { get; set; }

    /// <summary>
    /// Gets or sets the text of license agreement
    /// </summary>
    public string UserAgreementText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is recurring
    /// </summary>
    public bool IsRecurring { get; set; }

    /// <summary>
    /// Gets or sets the cycle length
    /// </summary>
    public int RecurringCycleLength { get; set; }

    /// <summary>
    /// Gets or sets the cycle period
    /// </summary>
    public int RecurringCyclePeriodId { get; set; }

    /// <summary>
    /// Gets or sets the total cycles
    /// </summary>
    public int RecurringTotalCycles { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is rental
    /// </summary>
    public bool IsRental { get; set; }

    /// <summary>
    /// Gets or sets the rental length for some period (price for this period)
    /// </summary>
    public int RentalPriceLength { get; set; }

    /// <summary>
    /// Gets or sets the rental period (price for this period)
    /// </summary>
    public int RentalPricePeriodId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is ship enabled
    /// </summary>
    public bool IsShipEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is free shipping
    /// </summary>
    public bool IsFreeShipping { get; set; }

    /// <summary>
    /// Gets or sets a value this product should be shipped separately (each item)
    /// </summary>
    public bool ShipSeparately { get; set; }

    /// <summary>
    /// Gets or sets the additional shipping charge
    /// </summary>
    public decimal AdditionalShippingCharge { get; set; }

    /// <summary>
    /// Gets or sets a delivery date identifier
    /// </summary>
    public int DeliveryDateId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is marked as tax exempt
    /// </summary>
    public bool IsTaxExempt { get; set; }

    /// <summary>
    /// Gets or sets the tax category identifier
    /// </summary>
    public int TaxCategoryId { get; set; }        

    /// <summary>
    /// Gets or sets a value indicating how to manage inventory
    /// </summary>
    public int ManageInventoryMethodId { get; set; }

    /// <summary>
    /// Gets or sets a product availability range identifier
    /// </summary>
    public int ProductAvailabilityRangeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether multiple warehouses are used for this product
    /// </summary>
    public bool UseMultipleWarehouses { get; set; }

    /// <summary>
    /// Gets or sets a warehouse identifier
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Gets or sets the stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display stock availability
    /// </summary>
    public bool DisplayStockAvailability { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display stock quantity
    /// </summary>
    public bool DisplayStockQuantity { get; set; }

    /// <summary>
    /// Gets or sets the minimum stock quantity
    /// </summary>
    public int MinStockQuantity { get; set; }

    /// <summary>
    /// Gets or sets the low stock activity identifier
    /// </summary>
    public int LowStockActivityId { get; set; }

    /// <summary>
    /// Gets or sets the quantity when admin should be notified
    /// </summary>
    public int NotifyAdminForQuantityBelow { get; set; }

    /// <summary>
    /// Gets or sets a value backorder mode identifier
    /// </summary>
    public int BackorderModeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to back in stock subscriptions are allowed
    /// </summary>
    public bool AllowBackInStockSubscriptions { get; set; }

    /// <summary>
    /// Gets or sets the order minimum quantity
    /// </summary>
    public int OrderMinimumQuantity { get; set; }

    /// <summary>
    /// Gets or sets the order maximum quantity
    /// </summary>
    public int OrderMaximumQuantity { get; set; }

    /// <summary>
    /// Gets or sets the comma separated list of allowed quantities. null or empty if any quantity is allowed
    /// </summary>
    public string AllowedQuantities { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we allow adding to the cart/wishlist only attribute combinations that exist and have stock greater than zero.
    /// This option is used only when we have "manage inventory" set to "track inventory by product attributes"
    /// </summary>
    public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display attribute combination images only
    /// </summary>
    public bool DisplayAttributeCombinationImagesOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this product is returnable (a customer is allowed to submit return request with this product)
    /// </summary>
    public bool NotReturnable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable buy (Add to cart) button
    /// </summary>
    public bool DisableBuyButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable "Add to wishlist" button
    /// </summary>
    public bool DisableWishlistButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this item is available for Pre-Order
    /// </summary>
    public bool AvailableForPreOrder { get; set; }

    /// <summary>
    /// Gets or sets the start date and time of the product availability (for pre-order products)
    /// </summary>
    public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show "Call for Pricing" or "Call for quote" instead of price
    /// </summary>
    public bool CallForPrice { get; set; }

    /// <summary>
    /// Gets or sets the price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the old price
    /// </summary>
    public decimal OldPrice { get; set; }

    /// <summary>
    /// Gets or sets the product cost
    /// </summary>
    public decimal ProductCost { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a customer enters price
    /// </summary>
    public bool CustomerEntersPrice { get; set; }

    /// <summary>
    /// Gets or sets the minimum price entered by a customer
    /// </summary>
    public decimal MinimumCustomerEnteredPrice { get; set; }

    /// <summary>
    /// Gets or sets the maximum price entered by a customer
    /// </summary>
    public decimal MaximumCustomerEnteredPrice { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether base price (PAngV) is enabled. Used by German users.
    /// </summary>
    public bool BasepriceEnabled { get; set; }

    /// <summary>
    /// Gets or sets an amount in product for PAngV
    /// </summary>
    public decimal BasepriceAmount { get; set; }

    /// <summary>
    /// Gets or sets a unit of product for PAngV (MeasureWeight entity)
    /// </summary>
    public int BasepriceUnitId { get; set; }

    /// <summary>
    /// Gets or sets a reference amount for PAngV
    /// </summary>
    public decimal BasepriceBaseAmount { get; set; }

    /// <summary>
    /// Gets or sets a reference unit for PAngV (MeasureWeight entity)
    /// </summary>
    public int BasepriceBaseUnitId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this product is marked as new
    /// </summary>
    public bool MarkAsNew { get; set; }

    /// <summary>
    /// Gets or sets the start date and time of the new product (set product as "New" from date). Leave empty to ignore this property
    /// </summary>
    public DateTime? MarkAsNewStartDateTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets the end date and time of the new product (set product as "New" to date). Leave empty to ignore this property
    /// </summary>
    public DateTime? MarkAsNewEndDateTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets the weight
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Gets or sets the length
    /// </summary>
    public decimal Length { get; set; }

    /// <summary>
    /// Gets or sets the width
    /// </summary>
    public decimal Width { get; set; }

    /// <summary>
    /// Gets or sets the height
    /// </summary>
    public decimal Height { get; set; }

    /// <summary>
    /// Gets or sets the available start date and time
    /// </summary>
    public DateTime? AvailableStartDateTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets the available end date and time
    /// </summary>
    public DateTime? AvailableEndDateTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets a display order.
    /// This value is used when sorting associated products (used with "grouped" products)
    /// This value is used when sorting home page products
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been deleted
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time of product creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of product update
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the product type
    /// </summary>
    public ProductType ProductType
    {
        get => (ProductType)ProductTypeId;
        set => ProductTypeId = (int)value;
    }

    /// <summary>
    /// Gets or sets the backorder mode
    /// </summary>
    public BackorderMode BackorderMode
    {
        get => (BackorderMode)BackorderModeId;
        set => BackorderModeId = (int)value;
    }

    /// <summary>
    /// Gets or sets the download activation type
    /// </summary>
    public DownloadActivationType DownloadActivationType
    {
        get => (DownloadActivationType)DownloadActivationTypeId;
        set => DownloadActivationTypeId = (int)value;
    }

    /// <summary>
    /// Gets or sets the gift card type
    /// </summary>
    public GiftCardType GiftCardType
    {
        get => (GiftCardType)GiftCardTypeId;
        set => GiftCardTypeId = (int)value;
    }

    /// <summary>
    /// Gets or sets the low stock activity
    /// </summary>
    public LowStockActivity LowStockActivity
    {
        get => (LowStockActivity)LowStockActivityId;
        set => LowStockActivityId = (int)value;
    }

    /// <summary>
    /// Gets or sets the value indicating how to manage inventory
    /// </summary>
    public ManageInventoryMethod ManageInventoryMethod
    {
        get => (ManageInventoryMethod)ManageInventoryMethodId;
        set => ManageInventoryMethodId = (int)value;
    }

    /// <summary>
    /// Gets or sets the cycle period for recurring products
    /// </summary>
    public RecurringProductCyclePeriod RecurringCyclePeriod
    {
        get => (RecurringProductCyclePeriod)RecurringCyclePeriodId;
        set => RecurringCyclePeriodId = (int)value;
    }

    /// <summary>
    /// Gets or sets the period for rental products
    /// </summary>
    public RentalPricePeriod RentalPricePeriod
    {
        get => (RentalPricePeriod)RentalPricePeriodId;
        set => RentalPricePeriodId = (int)value;
    }
}