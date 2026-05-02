namespace Nop.Core.Domain.Security;

/// <summary>
/// Represents a permission record
/// </summary>
public partial class PermissionRecord : BaseEntity
{
    /// <summary>
    /// Gets or sets the permission name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the permission system name
    /// </summary>
    public string SystemName { get; set; }

    /// <summary>
    /// Gets or sets the permission category
    /// </summary>
    public string Category { get; set; }
}