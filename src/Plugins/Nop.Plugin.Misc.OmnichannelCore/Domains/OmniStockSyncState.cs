using Nop.Core;

namespace Nop.Plugin.Misc.OmnichannelCore.Domains;

/// <summary>
/// Represents cross-channel stock projection state
/// </summary>
public class OmniStockSyncState : BaseEntity
{
    /// <summary>
    /// Gets or sets the nopCommerce product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product SKU
    /// </summary>
    public string Sku { get; set; }

    /// <summary>
    /// Gets or sets the warehouse identifier
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Gets or sets the source quantity on hand
    /// </summary>
    public int QuantityOnHand { get; set; }

    /// <summary>
    /// Gets or sets the source version used for stale update detection
    /// </summary>
    public long SourceVersion { get; set; }

    /// <summary>
    /// Gets or sets the last processed message identifier
    /// </summary>
    public Guid? LastMessageId { get; set; }

    /// <summary>
    /// Gets or sets the source system
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the synchronization status identifier
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Gets or sets the last seen date and time
    /// </summary>
    public DateTime LastSeenOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the updated date and time
    /// </summary>
    public DateTime? UpdatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the synchronization status
    /// </summary>
    public OmniStockSyncStatus Status
    {
        get => (OmniStockSyncStatus)StatusId;
        set => StatusId = (int)value;
    }
}
