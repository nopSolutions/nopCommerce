using Nop.Plugin.Misc.OmnichannelCore.Domains;

namespace Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;

/// <summary>
/// Represents stock synchronization result
/// </summary>
public record StockSyncResult
{
    /// <summary>
    /// Gets or sets the result code
    /// </summary>
    public string Result { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the update changed the projection
    /// </summary>
    public bool Applied { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the update was stale
    /// </summary>
    public bool Stale { get; set; }

    /// <summary>
    /// Gets or sets the stock synchronization state
    /// </summary>
    public OmniStockSyncState StockSyncState { get; set; }

    /// <summary>
    /// Gets or sets the result detail
    /// </summary>
    public string Detail { get; set; }
}
