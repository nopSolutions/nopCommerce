using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Models.ProductsParameters
{
    // JsonProperty is used only for swagger
    public class BaseProductsParametersModel
    {
        public BaseProductsParametersModel()
        {
            CreatedAtMin = null;
            CreatedAtMax = null;
            UpdatedAtMin = null;
            UpdatedAtMax = null;
            PublishedStatus = null;
            VendorName = null;
            CategoryId = null;
        }

        /// <summary>
        /// Show products created after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_min")]
        public DateTime? CreatedAtMin { get; set; }

        /// <summary>
        /// Show products created before date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_max")]
        public DateTime? CreatedAtMax { get; set; }

        /// <summary>
        /// Show products last updated after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("updated_at_min")]
        public DateTime? UpdatedAtMin { get; set; }

        /// <summary>
        /// Show products last updated before date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("updated_at_max")]
        public DateTime? UpdatedAtMax { get; set; }

        /// <summary>
        /// <ul>
        /// <li>published - Show only published products</li>
        /// <li>unpublished - Show only unpublished products</li>
        /// <li>any - Show all products (default)</li>
        /// </ul>
        /// </summary>
        [JsonProperty("published_status")]
        public bool? PublishedStatus { get; set; }

        /// <summary>
        /// Filter by product vendor
        /// </summary>
        [JsonProperty("vendor_name")]
        public string VendorName { get; set; }

        /// <summary>
        /// Show only the products mapped to the specified category
        /// </summary>
        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }
    }
}