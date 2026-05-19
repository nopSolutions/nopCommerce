namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents a product review
/// </summary>
public partial class ProductReview : BaseEntity
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the content is approved
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Gets or sets the title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the review text
    /// </summary>
    public string ReviewText { get; set; }

    /// <summary>
    /// Gets or sets the reply text
    /// </summary>
    public string ReplyText { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether the customer is already notified of the reply to review
    /// </summary>
    public bool CustomerNotifiedOfReply { get; set; }

    /// <summary>
    /// Review rating
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Review helpful votes total
    /// </summary>
    public int HelpfulYesTotal { get; set; }

    /// <summary>
    /// Review not helpful votes total
    /// </summary>
    public int HelpfulNoTotal { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}