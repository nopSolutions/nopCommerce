using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Inventory
{
    /// <summary>
    /// Represents request to get all locations
    /// </summary>
    public class GetLocationsRequest : InventoryApiRequest
    {
        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "organizations/self/locations";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}