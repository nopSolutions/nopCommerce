using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.PunchOut;

/// <summary>
/// Represents plugin settings
/// </summary>
public class PunchOutSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the PunchOut plugin is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the time in hours after which the PunchOut session will be expired
    /// </summary>
    public int TimeToExpire { get; set; }

    /// <summary>
    /// Gets or sets a list of restricted customer role identifiers
    /// </summary>
    public List<int> RestrictedCustomerRoleIds { get; set; } = new();
}
