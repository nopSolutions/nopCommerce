namespace Nop.Core.Domain.Security;

/// <summary>
/// Represents an entity which supports ACL
/// </summary>
public partial interface IAclSupported
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is subject to ACL
    /// </summary>
    bool SubjectToAcl { get; set; }
}