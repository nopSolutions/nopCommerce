using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Models.CategoriesParameters
{
    // JsonProperty is used only for swagger
    public class BaseCategoriesParametersModel
    {
        public BaseCategoriesParametersModel()
        {
            ProductId = null;
            CreatedAtMin = null;
            CreatedAtMax = null;
            UpdatedAtMin = null;
            UpdatedAtMax = null;
            PublishedStatus = null;
        }

        /// <summary>
        /// Show categories created after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_min")]
        public DateTime? CreatedAtMin { get; set; }

        /// <summary>
        /// Show categories created before date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_max")]
        public DateTime? CreatedAtMax { get; set; }

        /// <summary>
        /// Show categories last updated after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("updated_at_min")]
        public DateTime? UpdatedAtMin { get; set; }

        /// <summary>
        /// Show categories last updated before date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("updated_at_max")]
        public DateTime? UpdatedAtMax { get; set; }

        /// <summary>
        /// <ul>
        /// <li>published - Show only published categories</li>
        /// <li>unpublished - Show only unpublished categories</li>
        /// <li>any - Show all categories(default)</li>
        /// </ul>
        /// </summary>
        [JsonProperty("published_status")]
        public bool? PublishedStatus { get; set; }

        /// <summary>
        /// Show only the categories to which the product is mapped to
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }
    }
}