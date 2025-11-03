using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTO.Base;
using Nop.Plugin.Api.DTO.Products;

namespace Nop.Plugin.Api.DTO.OrderItems
{
    //[Validator(typeof(OrderItemDtoValidator))]
    [JsonObject(Title = "order_item")]
    public class OrderItemDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the selected attributes
        /// </summary>
        [JsonProperty("product_attributes")]
        public ICollection<ProductItemAttributeDto> Attributes { get; set; }

        /// <summary>
        ///     Gets or sets the quantity
        /// </summary>
        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        ///     Gets or sets the unit price in primary store currency (incl tax)
        /// </summary>
        [JsonProperty("unit_price_incl_tax")]
        public decimal? UnitPriceInclTax { get; set; }

        /// <summary>
        ///     Gets or sets the unit price in primary store currency (excl tax)
        /// </summary>
        [JsonProperty("unit_price_excl_tax")]
        public decimal? UnitPriceExclTax { get; set; }

        /// <summary>
        ///     Gets or sets the price in primary store currency (incl tax)
        /// </summary>
        [JsonProperty("price_incl_tax")]
        public decimal? PriceInclTax { get; set; }

        /// <summary>
        ///     Gets or sets the price in primary store currency (excl tax)
        /// </summary>
        [JsonProperty("price_excl_tax")]
        public decimal? PriceExclTax { get; set; }

        /// <summary>
        ///     Gets or sets the discount amount (incl tax)
        /// </summary>
        [JsonProperty("discount_amount_incl_tax")]
        public decimal? DiscountAmountInclTax { get; set; }

        /// <summary>
        ///     Gets or sets the discount amount (excl tax)
        /// </summary>
        [JsonProperty("discount_amount_excl_tax")]
        public decimal? DiscountAmountExclTax { get; set; }

        /// <summary>
        ///     Gets or sets the original cost of this order item (when an order was placed), qty 1
        /// </summary>
        [JsonProperty("original_product_cost")]
        public decimal? OriginalProductCost { get; set; }

        /// <summary>
        ///     Gets or sets the attribute description
        /// </summary>
        [JsonProperty("attribute_description")]
        public string AttributeDescription { get; set; }

        /// <summary>
        ///     Gets or sets the download count
        /// </summary>
        [JsonProperty("download_count")]
        public int? DownloadCount { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether download is activated
        /// </summary>
        [JsonProperty("isDownload_activated")]
        public bool? IsDownloadActivated { get; set; }

        /// <summary>
        ///     Gets or sets a license download identifier (in case this is a downloadable product)
        /// </summary>
        [JsonProperty("license_download_id")]
        public int? LicenseDownloadId { get; set; }

        /// <summary>
        ///     Gets or sets the total weight of one item
        ///     It's nullable for compatibility with the previous version of nopCommerce where was no such property
        /// </summary>
        [JsonProperty("item_weight")]
        public decimal? ItemWeight { get; set; }

        /// <summary>
        ///     Gets or sets the rental product start date (null if it's not a rental product)
        /// </summary>
        [JsonProperty("rental_start_date_utc")]
        public DateTime? RentalStartDateUtc { get; set; }

        /// <summary>
        ///     Gets or sets the rental product end date (null if it's not a rental product)
        /// </summary>
        [JsonProperty("rental_end_date_utc")]
        public DateTime? RentalEndDateUtc { get; set; }

        /// <summary>
        ///     Gets the product
        /// </summary>
        [JsonProperty("product")]
        [DoNotMap]
        public ProductDto Product { get; set; }

        [JsonProperty("product_id")]
        public int? ProductId { get; set; }
    }
}
