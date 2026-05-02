using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers;

/// <summary>
/// Private messages settings
/// </summary>
public partial class PrivateMessageSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether private messages are allowed
    /// </summary>
    public bool AllowPrivateMessages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a customer should be notified about new private messages
    /// </summary>
    public bool NotifyAboutPrivateMessages { get; set; }

    /// <summary>
    /// Gets or sets maximum length of pm subject
    /// </summary>
    public int PMSubjectMaxLength { get; set; }

    /// <summary>
    /// Gets or sets maximum length of pm message
    /// </summary>
    public int PMTextMaxLength { get; set; }

    /// <summary>
    /// Gets or sets the page size for private messages
    /// </summary>
    public int PrivateMessagesPageSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether an alert should be shown for new private messages
    /// </summary>
    public bool ShowAlertForPM { get; set; }
}
