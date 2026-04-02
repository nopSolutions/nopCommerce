namespace Nop.Core.Domain.Common;

/// <summary>
/// Search term record (for statistics)
/// </summary>
public partial class SearchTerm : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// Gets or sets the keyword
    /// </summary>
    public string Keyword { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been deleted
    /// </summary>
    public bool Deleted { get; set; }
}