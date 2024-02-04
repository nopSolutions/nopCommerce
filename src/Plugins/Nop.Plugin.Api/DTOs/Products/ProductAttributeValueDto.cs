using System;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTO.Base;
using Nop.Plugin.Api.DTO.Images;

namespace Nop.Plugin.Api.DTO.Products
{
    [JsonObject(Title = "attribute_value")]
    //[Validator(typeof(ProductDtoValidator))]
    public class ProductAttributeValueDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the attribute value type identifier
        /// </summary>
        [JsonProperty("type_id")]
        public int? AttributeValueTypeId { get; set; }

        /// <summary>
        ///     Gets or sets the associated product identifier (used only with AttributeValueType.AssociatedToProduct)
        /// </summary>
        [JsonProperty("associated_product_id")]
        public int? AssociatedProductId { get; set; }

        /// <summary>
        ///     Gets or sets the product attribute name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the color RGB value (used with "Color squares" attribute type)
        /// </summary>
        [JsonProperty("color_squares_rgb")]
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        ///     Gets or sets the picture ID for image square (used with "Image squares" attribute type)
        /// </summary>
        [JsonProperty("image_squares_image")]
        public ImageDto ImageSquaresImage { get; set; }

        /// <summary>
        ///     Gets or sets the price adjustment (used only with AttributeValueType.Simple)
        /// </summary>
        [JsonProperty("price_adjustment")]
        public decimal? PriceAdjustment { get; set; }

        /// <summary>
        ///     Gets or sets the weight adjustment (used only with AttributeValueType.Simple)
        /// </summary>
        [JsonProperty("weight_adjustment")]
        public decimal? WeightAdjustment { get; set; }

        /// <summary>
        ///     Gets or sets the attibute value cost (used only with AttributeValueType.Simple)
        /// </summary>
        [JsonProperty("cost")]
        public decimal? Cost { get; set; }

        /// <summary>
        ///     Gets or sets the quantity of associated product (used only with AttributeValueType.AssociatedToProduct)
        /// </summary>
        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        [JsonProperty("is_pre_selected")]
        public bool? IsPreSelected { get; set; }

        /// <summary>
        ///     Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }

        /// <summary>
        ///     Gets or sets the picture (identifier) associated with this value. This picture should replace a product main
        ///     picture once clicked (selected).
        /// </summary>
        [JsonIgnore]
        public int? PictureId { get; set; }

        [JsonProperty("product_image_id")]
        public int? ProductPictureId { get; set; }

        /// <summary>
        ///     Gets or sets the attribute value type
        /// </summary>
        [JsonProperty("type")]
        public string AttributeValueType
        {
            get => ((AttributeValueType) AttributeValueTypeId).ToString();
            set
            {
                AttributeValueType attributeValueTypeId;
                if (Enum.TryParse(value, out attributeValueTypeId))
                {
                    AttributeValueTypeId = (int) attributeValueTypeId;
                }
            }
        }
    }
}
