using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents request to get the single product
    /// </summary>
    public class GetProductRequest : ProductApiRequest, IConditionalRequest
    {
        /// <summary>
        /// Gets or sets the product unique identifier as UUID version 1
        /// </summary>
        [JsonIgnore]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the ETag header value
        /// </summary>
        [JsonIgnore]
        public string ETag { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"organizations/self/products/{Uuid}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}