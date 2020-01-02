using Newtonsoft.Json;

namespace Nop.Plugin.Api.Models.ProductManufacturerMappingsParameters
{
    // JsonProperty is used only for swagger
    public class BaseManufacturerMappingsParametersModel
    {
        /// <summary>
        /// Show all the product-manufacturer mappings for this product
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        /// Show all the product-manufacturer mappings for this manufacturer
        /// </summary>
        [JsonProperty("manufacturer_id")]
        public int? ManufacturerId { get; set; } 
    }
}