namespace Nop.Core.Domain.Common;

/// <summary>
/// Search term record (for statistics)
/// </summary>
public partial class SearchTerm : BaseEntity
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
    /// Gets or sets search count
    /// </summary>
    public int Count { get; set; }
}