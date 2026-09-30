namespace Nop.Plugin.Api.Rest.Models.Requests
{
    /// <summary>
    /// Represents a request to change a product already on a customer's wishlist. Omitted properties are left as they are.
    /// </summary>
    public class UpdateCustomerProductRequest
    {
        /// <summary>
        /// Gets or sets the new quantity; null leaves the quantity unchanged. Use the delete endpoint to remove the line instead.
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the custom wishlist to move the line to; null leaves it where it is, 0 moves it to the default wishlist
        /// </summary>
        public int? WishlistId { get; set; }
    }
}
