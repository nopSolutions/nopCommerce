namespace Nop.Services.Messages;

/// <summary>
/// Message structure
/// </summary>
public struct NotifyData
{
    /// <summary>
    /// Message type (success/warning/error)
    /// </summary>
    public NotifyType Type { get; set; }

    /// <summary>
    /// Message text
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Get a sets a value indicating whether the message should not be HTML encoded
    /// </summary>
    public bool Encode { get; set; }

    /// <summary>
    /// Get or sets the time, in milliseconds, on which notification will close automatically
    /// </summary>
    public int Timeout { get; set; }
}