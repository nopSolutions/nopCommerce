using System;

namespace Nop.Plugin.Api.Rest.Models.Requests
{
    /// <summary>
    /// Represents a request to put a product on a customer's wishlist.
    /// </summary>
    public class AddCustomerProductRequest
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the quantity; defaults to 1
        /// </summary>
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Gets or sets the target custom wishlist identifier; null adds the product to the default wishlist
        /// </summary>
        public int? WishlistId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier; null uses the current store
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// Gets or sets the selected product attributes in XML format
        /// </summary>
        public string? AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the price entered by the customer
        /// </summary>
        public decimal? CustomerEnteredPrice { get; set; }

        /// <summary>
        /// Gets or sets the rental start date
        /// </summary>
        public DateTime? RentalStartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the rental end date
        /// </summary>
        public DateTime? RentalEndDateUtc { get; set; }
    }
}
