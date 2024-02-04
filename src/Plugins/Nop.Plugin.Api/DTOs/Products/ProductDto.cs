using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTO.Base;
using Nop.Plugin.Api.DTO.Images;
using Nop.Plugin.Api.DTO.Languages;
using Nop.Plugin.Api.DTO.SpecificationAttributes;

namespace Nop.Plugin.Api.DTO.Products
{
    [JsonObject(Title = "product")]
    //[Validator(typeof(ProductDtoValidator))]
    public class ProductDto : BaseDto
    {
        private int? _productTypeId;

        /// <summary>
        ///     Gets or sets the values indicating whether this product is visible in catalog or search results.
        ///     It's used when this product is associated to some "grouped" one
        ///     This way associated products could be accessed/added/etc only from a grouped product details page
        /// </summary>
        [JsonProperty("visible_individually")]
        public bool? VisibleIndividually { get; set; }

        /// <summary>
        ///     Gets or sets the name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the short description
        /// </summary>
        [JsonProperty("short_description")]
        public string ShortDescription { get; set; }

        /// <summary>
        ///     Gets or sets the full description
        /// </summary>
        [JsonProperty("full_description")]
        public string FullDescription { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to show the product on home page
        /// </summary>
        [JsonProperty("show_on_home_page")]
        public bool? ShowOnHomepage { get; set; }

        /// <summary>
        ///     Gets or sets the meta keywords
        /// </summary>
        [JsonProperty("meta_keywords")]
        public string MetaKeywords { get; set; }

        /// <summary>
        ///     Gets or sets the meta description
        /// </summary>
        [JsonProperty("meta_description")]
        public string MetaDescription { get; set; }

        /// <summary>
        ///     Gets or sets the meta title
        /// </summary>
        [JsonProperty("meta_title")]
        public string MetaTitle { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product allows customer reviews
        /// </summary>
        [JsonProperty("allow_customer_reviews")]
        public bool? AllowCustomerReviews { get; set; }

        /// <summary>
        ///     Gets or sets the rating sum (approved reviews)
        /// </summary>
        [JsonProperty("approved_rating_sum")]
        public int? ApprovedRatingSum { get; set; }

        /// <summary>
        ///     Gets or sets the rating sum (not approved reviews)
        /// </summary>
        [JsonProperty("not_approved_rating_sum")]
        public int? NotApprovedRatingSum { get; set; }

        /// <summary>
        ///     Gets or sets the total rating votes (approved reviews)
        /// </summary>
        [JsonProperty("approved_total_reviews")]
        public int? ApprovedTotalReviews { get; set; }

        /// <summary>
        ///     Gets or sets the total rating votes (not approved reviews)
        /// </summary>
        [JsonProperty("not_approved_total_reviews")]
        public int? NotApprovedTotalReviews { get; set; }

        /// <summary>
        ///     Gets or sets the SKU
        /// </summary>
        [JsonProperty("sku")]
        public string Sku { get; set; }

        /// <summary>
        ///     Gets or sets the manufacturer part number
        /// </summary>
        [JsonProperty("manufacturer_part_number")]
        public string ManufacturerPartNumber { get; set; }

        /// <summary>
        ///     Gets or sets the Global Trade Item Number (GTIN). These identifiers include UPC (in North America), EAN (in
        ///     Europe), JAN (in Japan), and ISBN (for books).
        /// </summary>
        [JsonProperty("gtin")]
        public string Gtin { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product is gift card
        /// </summary>
        [JsonProperty("is_gift_card")]
        public bool? IsGiftCard { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product requires that other products are added to the cart (Product X
        ///     requires Product Y)
        /// </summary>
        [JsonProperty("require_other_products")]
        public bool? RequireOtherProducts { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether required products are automatically added to the cart
        /// </summary>
        [JsonProperty("automatically_add_required_products")]
        public bool? AutomaticallyAddRequiredProducts { get; set; }

        /// <summary>
        ///     Gets or sets a list of ids of products that are required to be added to the cart by this product
        /// </summary>
        [JsonProperty("required_product_ids", Required = Required.DisallowNull)]
        public IList<int> RequiredProductIds { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product is download
        /// </summary>
        [JsonProperty("is_download")]
        public bool? IsDownload { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this downloadable product can be downloaded unlimited number of times
        /// </summary>
        [JsonProperty("unlimited_downloads")]
        public bool? UnlimitedDownloads { get; set; }

        /// <summary>
        ///     Gets or sets the maximum number of downloads
        /// </summary>
        [JsonProperty("max_number_of_downloads")]
        public int? MaxNumberOfDownloads { get; set; }

        /// <summary>
        ///     Gets or sets the number of days during customers keeps access to the file.
        /// </summary>
        [JsonProperty("download_expiration_days")]
        public int? DownloadExpirationDays { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product has a sample download file
        /// </summary>
        [JsonProperty("has_sample_download")]
        public bool? HasSampleDownload { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product has user agreement
        /// </summary>
        [JsonProperty("has_user_agreement")]
        public bool? HasUserAgreement { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product is recurring
        /// </summary>
        [JsonProperty("is_recurring")]
        public bool? IsRecurring { get; set; }

        /// <summary>
        ///     Gets or sets the cycle length
        /// </summary>
        [JsonProperty("recurring_cycle_length")]
        public int? RecurringCycleLength { get; set; }

        /// <summary>
        ///     Gets or sets the total cycles
        /// </summary>
        [JsonProperty("recurring_total_cycles")]
        public int? RecurringTotalCycles { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product is rental
        /// </summary>
        [JsonProperty("is_rental")]
        public bool? IsRental { get; set; }

        /// <summary>
        ///     Gets or sets the rental length for some period (price for this period)
        /// </summary>
        [JsonProperty("rental_price_length")]
        public int? RentalPriceLength { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the entity is ship enabled
        /// </summary>
        [JsonProperty("is_ship_enabled")]
        public bool? IsShipEnabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the entity is free shipping
        /// </summary>
        [JsonProperty("is_free_shipping")]
        public bool? IsFreeShipping { get; set; }

        /// <summary>
        ///     Gets or sets a value this product should be shipped separately (each item)
        /// </summary>
        [JsonProperty("ship_separately")]
        public bool? ShipSeparately { get; set; }

        /// <summary>
        ///     Gets or sets the additional shipping charge
        /// </summary>
        [JsonProperty("additional_shipping_charge")]
        public decimal? AdditionalShippingCharge { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product is marked as tax exempt
        /// </summary>
        [JsonProperty("is_tax_exempt")]
        public bool? IsTaxExempt { get; set; }

        /// <summary>
        ///     Gets or sets the tax category Id
        /// </summary>
        [JsonProperty("tax_category_id")]
        public int? TaxCategoryId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product is telecommunications or broadcasting or electronic services
        /// </summary>
        [JsonProperty("is_telecommunications_or_broadcasting_or_electronic_services")]
        public bool? IsTelecommunicationsOrBroadcastingOrElectronicServices { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether multiple warehouses are used for this product
        /// </summary>
        [JsonProperty("use_multiple_warehouses")]
        public bool? UseMultipleWarehouses { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating how to manage inventory.
        ///     0 - do not track inventory
        ///     1 - track inventory
        ///     2 - track invetory by attributes
        /// </summary>
        [JsonProperty("manage_inventory_method_id")]
        public int? ManageInventoryMethodId { get; set; }

        /// <summary>
        ///     Gets or sets the stock quantity
        /// </summary>
        [JsonProperty("stock_quantity")]
        public int? StockQuantity { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to display stock availability
        /// </summary>
        [JsonProperty("display_stock_availability")]
        public bool? DisplayStockAvailability { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to display stock quantity
        /// </summary>
        [JsonProperty("display_stock_quantity")]
        public bool? DisplayStockQuantity { get; set; }

        /// <summary>
        ///     Gets or sets the minimum stock quantity
        /// </summary>
        [JsonProperty("min_stock_quantity")]
        public int? MinStockQuantity { get; set; }

        /// <summary>
        ///     Gets or sets the low stock activity identifier
        ///     0 - Nothing
        ///     1 - Disable buy button
        ///     2 - Unpublish
        /// </summary>
        [JsonProperty("low_stock_activity_id")]
        public int LowStockActivityId { get; set; }

        /// <summary>
        ///     Gets or sets the quantity when admin should be notified
        /// </summary>
        [JsonProperty("notify_admin_for_quantity_below")]
        public int? NotifyAdminForQuantityBelow { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to back in stock subscriptions are allowed
        /// </summary>
        [JsonProperty("allow_back_in_stock_subscriptions")]
        public bool? AllowBackInStockSubscriptions { get; set; }

        /// <summary>
        ///     Gets or sets the order minimum quantity
        /// </summary>
        [JsonProperty("order_minimum_quantity")]
        public int? OrderMinimumQuantity { get; set; }

        /// <summary>
        ///     Gets or sets the order maximum quantity
        /// </summary>
        [JsonProperty("order_maximum_quantity")]
        public int? OrderMaximumQuantity { get; set; }

        /// <summary>
        ///     Gets or sets the comma seperated list of allowed quantities. null or empty if any quantity is allowed
        /// </summary>
        [JsonProperty("allowed_quantities")]
        public string AllowedQuantities { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether we allow adding to the cart/wishlist only attribute combinations that exist
        ///     and have stock greater than zero.
        ///     This option is used only when we have "manage inventory" set to "track inventory by product attributes"
        /// </summary>
        [JsonProperty("allow_adding_only_existing_attribute_combinations")]
        public bool? AllowAddingOnlyExistingAttributeCombinations { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to disable buy (Add to cart) button
        /// </summary>
        [JsonProperty("disable_buy_button")]
        public bool? DisableBuyButton { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to disable "Add to wishlist" button
        /// </summary>
        [JsonProperty("disable_wishlist_button")]
        public bool? DisableWishlistButton { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this item is available for Pre-Order
        /// </summary>
        [JsonProperty("available_for_pre_order")]
        public bool? AvailableForPreOrder { get; set; }

        /// <summary>
        ///     Gets or sets the start date and time of the product availability (for pre-order products)
        /// </summary>
        [JsonProperty("pre_order_availability_start_date_time_utc")]
        public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to show "Call for Pricing" or "Call for quote" instead of price
        /// </summary>
        [JsonProperty("call_for_price")]
        public bool? CallForPrice { get; set; }

        /// <summary>
        ///     Gets or sets the price
        /// </summary>
        [JsonProperty("price")]
        public decimal? Price { get; set; }

        /// <summary>
        ///     Gets or sets the old price
        /// </summary>
        [JsonProperty("old_price")]
        public decimal? OldPrice { get; set; }

        /// <summary>
        ///     Gets or sets the product cost
        /// </summary>
        [JsonProperty("product_cost")]
        public decimal? ProductCost { get; set; }

        /// <summary>
        ///     Gets or sets the product special price
        /// </summary>
        [JsonProperty("special_price")]
        public decimal? SpecialPrice { get; set; }

        /// <summary>
        ///     Gets or sets the start date and time of the special price
        /// </summary>
        [JsonProperty("special_price_start_date_time_utc")]
        public DateTime? SpecialPriceStartDateTimeUtc { get; set; }

        /// <summary>
        ///     Gets or sets the end date and time of the special price
        /// </summary>
        [JsonProperty("special_price_end_date_time_utc")]
        public DateTime? SpecialPriceEndDateTimeUtc { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a customer enters price
        /// </summary>
        [JsonProperty("customer_enters_price")]
        public bool? CustomerEntersPrice { get; set; }

        /// <summary>
        ///     Gets or sets the minimum price entered by a customer
        /// </summary>
        [JsonProperty("minimum_customer_entered_price")]
        public decimal? MinimumCustomerEnteredPrice { get; set; }

        /// <summary>
        ///     Gets or sets the maximum price entered by a customer
        /// </summary>
        [JsonProperty("maximum_customer_entered_price")]
        public decimal? MaximumCustomerEnteredPrice { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether base price (PAngV) is enabled. Used by German users.
        /// </summary>
        [JsonProperty("baseprice_enabled")]
        public bool? BasepriceEnabled { get; set; }

        /// <summary>
        ///     Gets or sets an amount in product for PAngV
        /// </summary>
        [JsonProperty("baseprice_amount")]
        public decimal? BasepriceAmount { get; set; }

        /// <summary>
        ///     Gets or sets a reference amount for PAngV
        /// </summary>
        [JsonProperty("baseprice_base_amount")]
        public decimal? BasepriceBaseAmount { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this product has tier prices configured
        ///     <remarks>
        ///         The same as if we run this.TierPrices.Count > 0
        ///         We use this property for performance optimization:
        ///         if this property is set to false, then we do not need to load tier prices navigation property
        ///     </remarks>
        /// </summary>
        [JsonProperty("has_tier_prices")]
        public bool? HasTierPrices { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this product has discounts applied
        ///     <remarks>
        ///         The same as if we run this.AppliedDiscounts.Count > 0
        ///         We use this property for performance optimization:
        ///         if this property is set to false, then we do not need to load Applied Discounts navigation property
        ///     </remarks>
        /// </summary>
        [JsonProperty("has_discounts_applied")]
        public bool? HasDiscountsApplied { get; set; }

        /// <summary>
        ///     Gets or sets the weight
        /// </summary>
        [JsonProperty("weight")]
        public decimal? Weight { get; set; }

        /// <summary>
        ///     Gets or sets the length
        /// </summary>
        [JsonProperty("length")]
        public decimal? Length { get; set; }

        /// <summary>
        ///     Gets or sets the width
        /// </summary>
        [JsonProperty("width")]
        public decimal? Width { get; set; }

        /// <summary>
        ///     Gets or sets the height
        /// </summary>
        [JsonProperty("height")]
        public decimal? Height { get; set; }

        /// <summary>
        ///     Gets or sets the available start date and time
        /// </summary>
        [JsonProperty("available_start_date_time_utc")]
        public DateTime? AvailableStartDateTimeUtc { get; set; }

        /// <summary>
        ///     Gets or sets the available end date and time
        /// </summary>
        [JsonProperty("available_end_date_time_utc")]
        public DateTime? AvailableEndDateTimeUtc { get; set; }

        /// <summary>
        ///     Gets or sets a display order.
        ///     This value is used when sorting associated products (used with "grouped" products)
        ///     This value is used when sorting home page products
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the entity is published
        /// </summary>
        [JsonProperty("published")]
        public bool? Published { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        [JsonProperty("deleted")]
        public bool? Deleted { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of product creation
        /// </summary>
        [JsonProperty("created_on_utc")]
        public DateTime? CreatedOnUtc { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of product update
        /// </summary>
        [JsonProperty("updated_on_utc")]
        public DateTime? UpdatedOnUtc { get; set; }

        /// <summary>
        ///     Gets or sets the product type
        /// </summary>
        [ProductTypeValidation]
        [JsonProperty("product_type")]
        public string ProductType
        {
            get
            {
                var productTypeId = _productTypeId;
                if (productTypeId != null)
                {
                    return ((ProductType) productTypeId).ToString();
                }

                return null;
            }
            set
            {
                ProductType productTypeId;
                if (Enum.TryParse(value, out productTypeId))
                {
                    _productTypeId = (int) productTypeId;
                }
                else
                {
                    _productTypeId = null;
                }
            }
        }

        [JsonProperty("parent_grouped_product_id")]
        public int? ParentGroupedProductId { get; set; }

        [JsonProperty("role_ids")]
        public List<int> RoleIds { get; set; }

        [JsonProperty("discount_ids")]
        public List<int> DiscountIds { get; set; }

        [JsonProperty("store_ids")]
        public List<int> StoreIds { get; set; }

        [JsonProperty("manufacturer_ids")]
        public List<int> ManufacturerIds { get; set; }

        [ImageCollectionValidation]
        [JsonProperty("images")]
        public List<ImageMappingDto> Images { get; set; }

        [JsonProperty("attributes")]
        public List<ProductAttributeMappingDto> ProductAttributeMappings { get; set; }

        [JsonProperty("product_attribute_combinations")]
        public List<ProductAttributeCombinationDto> ProductAttributeCombinations { get; set; }


        [JsonProperty("product_specification_attributes")]
        public List<ProductSpecificationAttributeDto> ProductSpecificationAttributes { get; set; }

        [JsonProperty("associated_product_ids")]
        public List<int> AssociatedProductIds { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [ValidateVendor]
        [JsonProperty("vendor_id")]
        public int? VendorId { get; set; }

        [JsonProperty("se_name")]
        public string SeName { get; set; }
    }
}
