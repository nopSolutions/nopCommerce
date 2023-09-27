using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents the products details
    /// </summary>
    public class ProductList : List<Product>, IApiResponse
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error description
        /// </summary>
        [JsonProperty(PropertyName = "error_description")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the developer message
        /// </summary>
        [JsonProperty(PropertyName = "developerMessage")]
        public string DeveloperMessage { get; set; }
    }
}