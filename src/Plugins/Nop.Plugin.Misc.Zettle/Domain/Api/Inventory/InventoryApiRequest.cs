namespace Nop.Plugin.Misc.Zettle.Domain.Api.Inventory
{
    /// <summary>
    /// Represents base request to Inventory API
    /// </summary>
    public abstract class InventoryApiRequest : ApiRequest, IAuthorizedRequest
    {
        /// <summary>
        /// Gets the request base URL
        /// </summary>
        public override string BaseUrl => "https://inventory.izettle.com/";
    }
}