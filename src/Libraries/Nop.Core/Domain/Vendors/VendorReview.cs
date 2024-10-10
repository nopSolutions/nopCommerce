namespace Nop.Core.Domain.Vendors;

/// <summary>
/// Represents a vendor review
/// </summary>
public partial class VendorReview : BaseEntity
{
    /// <summary>
    /// Gets or sets the vendor identifier
    /// </summary>
    public int VendorId { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the rating
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets the review text
    /// </summary>
    public string ReviewText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the content is approved
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}
