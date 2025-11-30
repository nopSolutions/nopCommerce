using Newtonsoft.Json;
using Nop.Plugin.Api.Infrastructure;

namespace Nop.Plugin.Api.Models.ProductWarehouseInventoryParameters
{
    public class ProductWarehouseInventoryParametersModel
    {
        public ProductWarehouseInventoryParametersModel()
        {
            SinceId = Constants.Configurations.DefaultSinceId;
            Page = Constants.Configurations.DefaultPageValue;
            Limit = Constants.Configurations.DefaultLimit;
            Fields = string.Empty;
        }

        /// <summary>
        ///     Show all the product-warehouse inventories for this product
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        ///     Show all the product-warehouse inventories for this category
        /// </summary>
        [JsonProperty("warehouse_id")]
        public int? WarehouseId { get; set; }

        /// <summary>
        ///     Restrict results to after the specified ID
        /// </summary>
        [JsonProperty("since_id")]
        public int SinceId { get; set; }

        /// <summary>
        ///     Page to show (default: 1)
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        ///     Amount of results (default: 50) (maximum: 250)
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <summary>
        ///     comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }
    }
}
