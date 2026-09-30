using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// A named wishlist the authenticated customer keeps their products in
/// </summary>
/// <remarks>
/// The default, unnamed wishlist has no record of its own in nopCommerce, so it is not listed here.
/// Its lines carry a null <see cref="WishlistItemDto.WishlistId"/> and are reachable through
/// GET /api/rest/customer/me/wishlist.
/// </remarks>
public record CustomerWishlistDto
{
    /// <summary>
    /// Gets or sets the wishlist identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the owning customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the wishlist name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the number of lines the wishlist currently holds
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation (UTC)
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}

/// <summary>
/// A single product line the authenticated customer has on their wishlist
/// </summary>
public record WishlistItemDto
{
    /// <summary>
    /// Gets or sets the wishlist line identifier
    /// </summary>
    public int Id { get; set; }

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
    /// Gets or sets the owning named wishlist identifier; null for the default unnamed wishlist
    /// </summary>
    public int? WishlistId { get; set; }

    /// <summary>
    /// Gets or sets the store the line was added from
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the selected product attributes in XML format
    /// </summary>
    public string? AttributesXml { get; set; }

    /// <summary>
    /// Gets or sets the price the customer entered
    /// </summary>
    public decimal CustomerEnteredPrice { get; set; }

    /// <summary>
    /// Gets or sets the rental start date; null when the product is not rented
    /// </summary>
    public DateTime? RentalStartDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the rental end date; null when the product is not rented
    /// </summary>
    public DateTime? RentalEndDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation (UTC)
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance update (UTC)
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }
}
