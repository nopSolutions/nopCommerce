using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ProductSpecificationAttributesParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ProductSpecificationAttributesParametersModel>))]
    public class ProductSpecificationAttributesParametersModel
    {
        public ProductSpecificationAttributesParametersModel()
        {
            Limit = Constants.Configurations.DefaultLimit;
            Page = Constants.Configurations.DefaultPageValue;
            SinceId = Constants.Configurations.DefaultSinceId;
            Fields = string.Empty;
        }

        /// <summary>
        ///     Product Id
        /// </summary>
        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        /// <summary>
        ///     Specification Attribute Option Id
        /// </summary>
        [JsonProperty("specification_attribute_option_id")]
        public int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        ///     Allow Filtering
        /// </summary>
        [JsonProperty("allow_filtering")]
        public bool? AllowFiltering { get; set; }

        /// <summary>
        ///     Show on Product Page
        /// </summary>
        [JsonProperty("show_on_product_page")]
        public bool? ShowOnProductPage { get; set; }

        /// <summary>
        ///     Amount of results (default: 50) (maximum: 250)
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <summary>
        ///     Page to show (default: 1)
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        ///     Restrict results to after the specified ID
        /// </summary>
        [JsonProperty("since_id")]
        public int SinceId { get; set; }

        /// <summary>
        ///     comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }
    }
}
