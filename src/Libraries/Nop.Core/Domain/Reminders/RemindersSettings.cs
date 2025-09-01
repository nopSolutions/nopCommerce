using Nop.Core.Configuration;

namespace Nop.Core.Domain.Reminders;

/// <summary>
/// Reminders settings
/// </summary>
public partial class RemindersSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value that indicates whether abandoned cart reminders are enabled
    /// </summary>
    public bool AbandonedCartEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether pending order reminders are enabled
    /// </summary>
    public bool PendingOrdersEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether incomplete registration reminders are enabled
    /// </summary>
    public bool IncompleteRegistrationEnabled { get; set; }
}
