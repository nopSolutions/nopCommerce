using System;

namespace Nop.Plugin.Api.Rest.Models
{
    /// <summary>
    /// Represents a single product a customer keeps on their wishlist.
    /// </summary>
    public class CustomerProductDto
    {
        /// <summary>
        /// Gets or sets the wishlist line identifier (shopping cart item identifier)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the owner of the line
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product name
        /// </summary>
        public string? ProductName { get; set; }

        /// <summary>
        /// Gets or sets the product SKU
        /// </summary>
        public string? ProductSku { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the owning custom wishlist identifier; null means the default (unnamed) wishlist
        /// </summary>
        public int? WishlistId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the selected product attributes in XML format
        /// </summary>
        public string? AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the price entered by the customer
        /// </summary>
        public decimal CustomerEnteredPrice { get; set; }

        /// <summary>
        /// Gets or sets the rental start date; null if the product is not rented
        /// </summary>
        public DateTime? RentalStartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the rental end date; null if the product is not rented
        /// </summary>
        public DateTime? RentalEndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }
    }
}
