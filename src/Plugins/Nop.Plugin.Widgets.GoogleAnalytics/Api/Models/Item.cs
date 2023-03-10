using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api.Models
{
    [JsonObject]
    [Serializable]
    public class Item
    {
        /// <summary>
        /// The ID of the item
        /// </summary>
        [JsonProperty("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// The name of the item
        /// </summary>
        [JsonProperty("item_name")]
        public string ItemName { get; set; }

        /// <summary>
        /// A product affiliation to designate a supplying company or brick and mortar store location
        /// </summary>
        [JsonProperty("affiliation")]
        public string Affiliation { get; set; }

        /// <summary>
        /// The category of the item. If used as part of a category hierarchy or taxonomy then this will be the first category
        /// </summary>
        [JsonProperty("item_category")]
        public string ItemCategory { get; set; }

        /// <summary>
        /// The monetary price of the item, in units of the specified currency parameter
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Item quantity
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

    }
}
