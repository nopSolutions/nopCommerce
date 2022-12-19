using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents request to get the import
    /// </summary>
    public class GetImportRequest : ProductApiRequest
    {
        /// <summary>
        /// Gets or sets the import unique identifier as UUID version 1
        /// </summary>
        [JsonIgnore]
        public string ImportUuid { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"organizations/self/import/status/{ImportUuid}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}