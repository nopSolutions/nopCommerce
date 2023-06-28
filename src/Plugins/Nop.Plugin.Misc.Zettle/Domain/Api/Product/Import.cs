using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents the import details
    /// </summary>
    public class Import : ApiResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the number of imported items
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public int? Items { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; set; }

        /// <summary>
        /// Gets or sets the finished date
        /// </summary>
        [JsonProperty(PropertyName = "finished")]
        public DateTime? Finished { get; set; }
    }
}