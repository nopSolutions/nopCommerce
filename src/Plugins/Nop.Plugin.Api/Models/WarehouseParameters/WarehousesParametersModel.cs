using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Infrastructure;

namespace Nop.Plugin.Api.Models.WarehousesParameters
{
    public class WarehousesParametersModel
    {
        public WarehousesParametersModel()
        {
            Ids = null;
            ProductId = null;
            //Limit = Constants.Configurations.DefaultLimit;
            //Page = Constants.Configurations.DefaultPageValue;
            Fields = string.Empty;
        }

        /// <summary>
        ///     Show all the product-warehouse inventories for this product
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        ///     A comma-separated list of category ids
        /// </summary>
        [JsonProperty("ids")]
        public List<int> Ids { get; set; }

        ///// <summary>
        /////     Amount of results (default: 50) (maximum: 250)
        ///// </summary>
        //[JsonProperty("limit")]
        //public int Limit { get; set; }

        ///// <summary>
        /////     Page to show (default: 1)
        ///// </summary>
        //[JsonProperty("page")]
        //public int Page { get; set; }

        /// <summary>
        ///     comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }
    }
}