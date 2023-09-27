using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents request to delete multiple products
    /// </summary>
    public class DeleteProductsRequest : ProductApiRequest
    {
        /// <summary>
        /// Gets or sets the list of product unique identifier as UUID version 1
        /// </summary>
        [JsonIgnore]
        public List<string> ProductUuids { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"organizations/self/products?uuid={string.Join("&uuid=", ProductUuids)}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Delete;
    }
}