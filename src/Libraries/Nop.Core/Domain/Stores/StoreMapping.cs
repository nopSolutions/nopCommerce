namespace Nop.Core.Domain.Stores;

/// <summary>
/// Represents a store mapping record
/// </summary>
public partial class StoreMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the entity identifier
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets the entity name
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }
}