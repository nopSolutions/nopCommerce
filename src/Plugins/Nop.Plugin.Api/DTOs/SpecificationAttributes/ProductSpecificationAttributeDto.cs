using System;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTO.Base;

namespace Nop.Plugin.Api.DTO.SpecificationAttributes
{
    [JsonObject(Title = "product_specification_attribute")]
    //[Validator(typeof(ProductSpecificationAttributeDtoValidator))]
    public class ProductSpecificationAttributeDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the product id
        /// </summary>
        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        /// <summary>
        ///     Gets or sets the attribute type ID
        /// </summary>
        [JsonProperty("attribute_type_id")]
        public int AttributeTypeId { get; set; }

        /// <summary>
        ///     Gets or sets the specification attribute identifier
        /// </summary>
        [JsonProperty("specification_attribute_option_id")]
        public int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        ///     Gets or sets the custom value
        /// </summary>
        [JsonProperty("custom_value")]
        public string CustomValue { get; set; }

        /// <summary>
        ///     Gets or sets whether the attribute can be filtered by
        /// </summary>
        [JsonProperty("allow_filtering")]
        public bool AllowFiltering { get; set; }

        /// <summary>
        ///     Gets or sets whether the attribute will be shown on the product page
        /// </summary>
        [JsonProperty("show_on_product_page")]
        public bool ShowOnProductPage { get; set; }

        /// <summary>
        ///     Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int DisplayOrder { get; set; }

        /// <summary>
        ///     Gets the attribute control type by casting the AttributeTypeId; sets the AttributeTypeId
        /// </summary>
        [JsonProperty("attribute_type")]
        public string AttributeType
        {
            get => ((SpecificationAttributeType) AttributeTypeId).ToString();
            set => AttributeTypeId = (int) Enum.Parse(typeof(SpecificationAttributeType), value);
        }

        /// <summary>
        ///     Gets or sets the specification attribute
        /// </summary>
        [JsonProperty("specification_attribute_option")]
        public virtual SpecificationAttributeOptionDto SpecificationAttributeOption { get; set; }
    }
}
