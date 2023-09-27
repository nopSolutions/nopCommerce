using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents request to delete the single product
    /// </summary>
    public class DeleteProductRequest : ProductApiRequest
    {
        /// <summary>
        /// Gets or sets the product unique identifier as UUID version 1
        /// </summary>
        [JsonIgnore]
        public string ProductUuid { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"organizations/self/products/{ProductUuid}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Delete;
    }
}