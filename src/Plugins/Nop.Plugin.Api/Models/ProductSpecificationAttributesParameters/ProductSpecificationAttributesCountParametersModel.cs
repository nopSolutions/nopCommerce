using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ProductSpecificationAttributes
{
    // JsonProperty is used only for swagger
    [ModelBinder(typeof(ParametersModelBinder<ProductSpecifcationAttributesCountParametersModel>))]
    public class ProductSpecifcationAttributesCountParametersModel
    {
        public ProductSpecifcationAttributesCountParametersModel()
        {

        }

        /// <summary>
        /// Product Id
        /// </summary>
        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        /// <summary>
        /// Specification Attribute Option Id
        /// </summary>
        [JsonProperty("specification_attribute_option_id")]
        public int SpecificationAttributeOptionId { get; set; }
    }
}