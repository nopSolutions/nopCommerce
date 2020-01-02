using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Base;

namespace Nop.Plugin.Api.DTOs.Products
{
    [JsonObject(Title = "attribute")]
    //[Validator(typeof(ProductDtoValidator))]
    public class ProductAttributeMappingDto : BaseDto
    {
        private List<ProductAttributeValueDto> _productAttributeValues;

        /// <summary>
        /// Gets or sets the product attribute identifier
        /// </summary>
        [JsonProperty("product_attribute_id")]
        public int ProductAttributeId { get; set; }

        [JsonProperty("product_attribute_name")]
        public string ProductAttributeName { get; set; }

        /// <summary>
        /// Gets or sets a value a text prompt
        /// </summary>
        [JsonProperty("text_prompt")]
        public string TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        [JsonProperty("is_required")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        [JsonProperty("attribute_control_type_id")]
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the default value (for textbox and multiline textbox)
        /// </summary>
        [JsonProperty("default_value")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        [JsonProperty("attribute_control_type_name")]
        public string AttributeControlType
        {
            get
            {
                return ((AttributeControlType)this.AttributeControlTypeId).ToString();
            }
            set
            {
                AttributeControlType attributeControlTypeId;
                if (Enum.TryParse(value, out attributeControlTypeId))
                {
                    this.AttributeControlTypeId = (int)attributeControlTypeId;
                }
            }
        }

        /// <summary>
        /// Gets the product attribute values
        /// </summary>
        [JsonProperty("attribute_values")]
        public List<ProductAttributeValueDto> ProductAttributeValues
        {
            get { return _productAttributeValues; }
            set { _productAttributeValues = value; }
        }
    }
}