using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents request to create the single product
    /// </summary>
    public class CreateProductRequest : Product, IApiRequest, IAuthorizedRequest
    {
        /// <summary>
        /// Gets the request base URL
        /// </summary>
        [JsonIgnore]
        public string BaseUrl => "https://products.izettle.com/";

        /// <summary>
        /// Gets the request path
        /// </summary>
        [JsonIgnore]
        public string Path => "organizations/self/products";

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public string Method => HttpMethods.Post;
    }
}