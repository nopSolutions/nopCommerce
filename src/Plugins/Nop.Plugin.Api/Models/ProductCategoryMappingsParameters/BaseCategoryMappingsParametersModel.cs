using Newtonsoft.Json;

namespace Nop.Plugin.Api.Models.ProductCategoryMappingsParameters
{
    // JsonProperty is used only for swagger
    public class BaseCategoryMappingsParametersModel
    {
        /// <summary>
        ///     Show all the product-category mappings for this product
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        ///     Show all the product-category mappings for this category
        /// </summary>
        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }
    }
}
