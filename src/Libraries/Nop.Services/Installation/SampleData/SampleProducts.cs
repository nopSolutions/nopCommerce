using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample products
/// </summary>
public partial class SampleProducts
{
    /// <summary>
    /// Gets or sets products
    /// </summary>
    public List<SampleProduct> Products { get; set; } = new();

    /// <summary>
    /// Gets or sets related products
    /// </summary>
    public List<SampleRelatedProduct> RelatedProducts { get; set; } = new();

    #region Nested classes

    /// <summary>
    /// Represents a sample download
    /// </summary>
    public partial class SampleDownload
    {
        /// <summary>
        /// The mime-type of the download
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The filename of the download
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the extension
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the download is new
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Gets or sets a download file name
        /// </summary>
        public string DownloadFileName { get; set; }
    }

    /// <summary>
    /// Represents a related product
    /// </summary>
    public partial class SampleRelatedProduct
    {
        /// <summary>
        /// Gets or sets the first product identifier
        /// </summary>
        public string FirstProductSku { get; set; }

        /// <summary>
        /// Gets or sets the second product identifier
        /// </summary>
        public string SecondProductSku { get; set; }
    }

    /// <summary>
    /// Represents a sample product
    /// </summary>
    public partial class SampleProduct
    {
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
        /// Gets or sets a value indicating whether to show the product on home page
        /// </summary>
        public bool ShowOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product allows customer reviews
        /// </summary>
        public bool AllowCustomerReviews { get; set; }

        /// <summary>
        /// Gets or sets the SKU
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is gift card
        /// </summary>
        public bool IsGiftCard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is downloaded
        /// </summary>
        public bool IsDownload { get; set; }

        /// <summary>
        /// Gets or sets the download identifier
        /// </summary>
        public SampleDownload Download { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this downloadable product can be downloaded unlimited number of times
        /// </summary>
        public bool UnlimitedDownloads { get; set; }

        /// <summary>
        /// Gets or sets the sample download identifier
        /// </summary>
        public SampleDownload SampleDownload { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product has user agreement
        /// </summary>
        public bool HasUserAgreement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is recurring
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// Gets or sets the cycle length
        /// </summary>
        public int RecurringCycleLength { get; set; }

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
        /// Gets or sets a value indicating whether the entity is ship enabled
        /// </summary>
        public bool IsShipEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is free shipping
        /// </summary>
        public bool IsFreeShipping { get; set; }

        /// <summary>
        /// Gets or sets a delivery date identifier
        /// </summary>
        public string DeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets the tax category name
        /// </summary>
        public string TaxCategoryName { get; set; }

        /// <summary>
        /// Gets or sets a product availability range identifier
        /// </summary>
        public string ProductAvailabilityRange { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display stock availability
        /// </summary>
        public bool DisplayStockAvailability { get; set; }

        /// <summary>
        /// Gets or sets the quantity when admin should be notified
        /// </summary>
        public int NotifyAdminForQuantityBelow { get; set; }

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
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the old price
        /// </summary>
        public decimal OldPrice { get; set; }

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
        /// Gets or sets a value indicating whether this product is marked as new
        /// </summary>
        public bool MarkAsNew { get; set; }

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
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the product type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductType ProductType { get; set; }

        /// <summary>
        /// Gets or sets the backorder mode
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public BackorderMode BackorderMode { get; set; }

        /// <summary>
        /// Gets or sets the download activation type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DownloadActivationType DownloadActivationType { get; set; }

        /// <summary>
        /// Gets or sets the gift card type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public GiftCardType GiftCardType { get; set; }

        /// <summary>
        /// Gets or sets the low stock activity
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public LowStockActivity LowStockActivity { get; set; }

        /// <summary>
        /// Gets or sets the value indicating how to manage inventory
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ManageInventoryMethod ManageInventoryMethod { get; set; }

        /// <summary>
        /// Gets or sets the cycle period for recurring products
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RecurringProductCyclePeriod RecurringCyclePeriod { get; set; }

        /// <summary>
        /// Gets or sets the period for rental products
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RentalPricePeriod RentalPricePeriod { get; set; }

        /// <summary>
        /// Gets or sets a value of used product template name
        /// </summary>
        public string ProductTemplateName { get; set; }

        /// <summary>
        /// Gets or sets a category name
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets a list of product pictures
        /// </summary>
        public List<string> ProductPictures { get; set; } = new();

        /// <summary>
        /// Gets or sets product attribute mappings
        /// </summary>
        public List<SampleProductAttributeMapping> ProductAttributeMapping { get; set; } = new();

        /// <summary>
        /// Gets or sets a list of product tags
        /// </summary>
        public List<string> ProductTags { get; set; } = new();

        /// <summary>
        /// Gets or sets a manufacturer name
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Gets or sets a list of product specification attributes
        /// </summary>
        public List<SampleProductSpecificationAttribute> ProductSpecificationAttribute { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of grouped products
        /// </summary>
        public List<SampleProduct> GroupedProducts { get; set; } = new();

        /// <summary>
        /// Gets or sets the e tier prices
        /// </summary>
        public List<SampleTierPrice> TierPrices { get; set; } = new();
    }

    /// <summary>
    /// Represents a sample tier price
    /// </summary>
    public partial class SampleTierPrice
    {
        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }
    }

    /// <summary>
    /// Represents a sample product specification attribute
    /// </summary>
    public partial class SampleProductSpecificationAttribute
    {
        /// <summary>
        /// Gets or sets whether the attribute can be filtered by
        /// </summary>
        public bool AllowFiltering { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute will be shown on the product page
        /// </summary>
        public bool ShowOnProductPage { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the spec attribute name
        /// </summary>
        public string SpecAttributeName { get; set; }

        /// <summary>
        /// Gets or sets the spec attribute option name
        /// </summary>
        public string SpecAttributeOptionName { get; set; }
    }

    /// <summary>
    /// Represents a sample product attribute mapping
    /// </summary>
    public partial class SampleProductAttributeMapping
    {
        /// <summary>
        /// Gets or sets a product attribute name
        /// </summary>
        public string ProductAttributeName { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AttributeControlType AttributeControlType { get; set; }

        /// <summary>
        /// Gets or sets a value a text prompt
        /// </summary>
        public string TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets an attribute values
        /// </summary>
        public List<SampleProductAttributeValue> AttributeValues { get; set; } = new();
    }

    /// <summary>
    /// Represents a product attribute value
    /// </summary>
    public partial class SampleProductAttributeValue
    {
        /// <summary>
        /// Gets or sets the product attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the color RGB value (used with "Color squares" attribute type)
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets the picture ID for image square (used with "Image squares" attribute type)
        /// </summary>
        public string ImageSquaresPictureName { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment (used only with AttributeValueType.Simple)
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the attribute value type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AttributeValueType AttributeValueType { get; set; } = AttributeValueType.Simple;

        public List<string> AttributeValuePictures { get; set; } = new();
    }

    #endregion
}