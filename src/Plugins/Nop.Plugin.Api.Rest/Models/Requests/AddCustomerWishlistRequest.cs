namespace Nop.Plugin.Api.Rest.Models.Requests
{
    /// <summary>
    /// Represents a request to create a named wishlist for a customer.
    /// </summary>
    public class AddCustomerWishlistRequest
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string? Name { get; set; }
    }
}
