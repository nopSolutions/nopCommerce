namespace Nop.Core.Domain.Orders;


/// <summary>
/// Represents a custom wishlist item
/// </summary>
public partial class CustomWishlist : BaseEntity
{
    /// <summary>
    /// Gets or sets a name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}
