using Nop.Core;

namespace Nop.Plugin.Misc.PunchOut.Domain;

/// <summary>
/// Represents a PunchOut identity
/// </summary>
public class PunchOutIdentity : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier used for PunchOut identity
    /// </summary>
    public string Identity { get; set; }

    /// <summary>
    /// Gets or sets the shared secret hash for the PunchOut identity
    /// </summary>
    public string SharedSecretHash { get; set; }
}
