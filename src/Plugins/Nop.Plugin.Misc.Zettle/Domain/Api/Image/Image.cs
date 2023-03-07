using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Image
{
    /// <summary>
    /// Represents uploaded image details
    /// </summary>
    public class Image : ApiResponse
    {
        /// <summary>
        /// Gets or sets the image lookup key
        /// </summary>
        [JsonProperty(PropertyName = "imageLookupKey")]
        public string ImageLookupKey { get; set; }

        /// <summary>
        /// Gets or sets a list of URLs to the images stored in service
        /// </summary>
        [JsonProperty(PropertyName = "imageUrls")]
        public List<string> ImageUrls { get; set; }

        /// <summary>
        /// Gets or sets the image source
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }
    }
}