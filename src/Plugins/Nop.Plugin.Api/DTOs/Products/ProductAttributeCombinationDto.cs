using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;

namespace Nop.Plugin.Api.DTO.Products
{
    [JsonObject(Title = "product_attribute_combination")]
    //[Validator(typeof(ProductAttributeCombinationDtoValidator))]
    public class ProductAttributeCombinationDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the product identifier
        /// </summary>
        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        /// <summary>
        ///     Gets or sets the attributes
        /// </summary>
        [JsonProperty("attributes_xml")]
        public string AttributesXml { get; set; }

        /// <summary>
        ///     Gets or sets the stock quantity
        /// </summary>
        [JsonProperty("stock_quantity")]
        public int StockQuantity { get; set; }

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
        ///     Gets or sets the attribute combination price. This way a store owner can override the default product price when
        ///     this attribute combination is added to the cart. For example, you can give a discount this way.
        /// </summary>
        [JsonProperty("overridden_price")]
        public decimal? OverriddenPrice { get; set; }

        /// <summary>
        ///     Gets or sets the identifier of picture associated with this combination
        /// </summary>
        [JsonProperty("picture_id")]
        public int PictureId { get; set; }
    }
}
