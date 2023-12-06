using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Inventory
{
    /// <summary>
    /// Represents location inventory balance details
    /// </summary>
    public class LocationInventoryBalance : ApiResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "locationUuid")]
        public string LocationUuid { get; set; }

        /// <summary>
        /// Gets or sets the tracked products
        /// </summary>
        [JsonProperty(PropertyName = "trackedProducts")]
        public List<string> TrackedProducts { get; set; }

        /// <summary>
        /// Gets or sets the tracked variants
        /// </summary>
        [JsonProperty(PropertyName = "variants")]
        public List<InventoryBalance> Variants { get; set; }

        /// <summary>
        /// Gets or sets the latest update date
        /// </summary>
        [JsonProperty(PropertyName = "latest")]
        public DateTime? LatestUpdate { get; set; }
    }
}