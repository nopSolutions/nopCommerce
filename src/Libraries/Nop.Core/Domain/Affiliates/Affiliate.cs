using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Affiliates;

/// <summary>
/// Represents an affiliate
/// </summary>
public partial class Affiliate : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// Gets or sets the address identifier
    /// </summary>
    public int AddressId { get; set; }

    /// <summary>
    /// Gets or sets the admin comment
    /// </summary>
    public string AdminComment { get; set; }

    /// <summary>
    /// Gets or sets the friendly name for generated affiliate URL (by default affiliate ID is used)
    /// </summary>
    public string FriendlyUrlName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been deleted
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is active
    /// </summary>
    public bool Active { get; set; }
}