using System;

namespace Nop.Plugin.Api.Rest.Models
{
    /// <summary>
    /// Represents a named wishlist a customer keeps their products in.
    /// </summary>
    public class CustomerWishlistDto
    {
        /// <summary>
        /// Gets or sets the custom wishlist identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the owner of the wishlist
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the number of product lines currently held by this wishlist
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
