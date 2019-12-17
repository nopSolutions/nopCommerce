using Newtonsoft.Json;
using Nop.Plugin.Api.Validators;
using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.SpecificationAttributes
{
    [JsonObject(Title = "specification_attribute")]
    public class SpecificationAttributeDto
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute options
        /// </summary>
        [JsonProperty("specification_attribute_options")]
        public List<SpecificationAttributeOptionDto> SpecificationAttributeOptions { get; set; }
    }
}