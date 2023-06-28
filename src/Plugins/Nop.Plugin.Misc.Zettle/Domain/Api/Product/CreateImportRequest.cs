using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents request to create import of multiple product
    /// </summary>
    public class CreateImportRequest : ProductApiRequest
    {
        /// <summary>
        /// Gets or sets the products
        /// </summary>
        [JsonProperty(PropertyName = "products")]
        public List<Product> Products { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        [JsonIgnore]
        public override string Path => "organizations/self/import/v2";

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}