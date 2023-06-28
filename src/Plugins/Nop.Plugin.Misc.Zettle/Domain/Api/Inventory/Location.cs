using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Inventory
{
    /// <summary>
    /// Represents location details
    /// </summary>
    public class Location : ApiResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the location is default
        /// </summary>
        [JsonProperty(PropertyName = "default")]
        public bool? IsDefault { get; set; }
    }
}