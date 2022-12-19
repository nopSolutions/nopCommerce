using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Inventory
{
    /// <summary>
    /// Represents request to get location inventory balance
    /// </summary>
    public class GetLocationInventoryBalanceRequest : InventoryApiRequest
    {
        /// <summary>
        /// Gets or sets the location type
        /// </summary>
        [JsonIgnore]
        public string LocationType { get; set; } = "STORE";

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"organizations/self/inventory/locations?type={LocationType.ToLower()}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}