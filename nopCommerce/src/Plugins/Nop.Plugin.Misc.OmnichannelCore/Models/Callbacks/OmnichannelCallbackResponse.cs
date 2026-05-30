namespace Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;

/// <summary>
/// Represents an internal callback response
/// </summary>
public record OmnichannelCallbackResponse
{
    /// <summary>
    /// Gets or sets the result code
    /// </summary>
    public string Result { get; set; }

    /// <summary>
    /// Gets or sets the message identifier
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier
    /// </summary>
    public string CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the inbox row identifier
    /// </summary>
    public int? InboxId { get; set; }

    /// <summary>
    /// Gets or sets the stock sync state row identifier
    /// </summary>
    public int? StockSyncStateId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the update changed the stock projection
    /// </summary>
    public bool Applied { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the message was a duplicate
    /// </summary>
    public bool Duplicate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the update was stale
    /// </summary>
    public bool Stale { get; set; }

    /// <summary>
    /// Gets or sets the latest stored source version
    /// </summary>
    public long? SourceVersion { get; set; }

    /// <summary>
    /// Gets or sets a response detail
    /// </summary>
    public string Detail { get; set; }
}
