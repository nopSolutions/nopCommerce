namespace Nop.Plugin.Misc.Zettle.Domain;

/// <summary>
/// Represents an inventory balance change type enumeration
/// </summary>
public enum InventoryBalanceChangeType
{
    /// <summary>
    /// Start tracking
    /// </summary>
    StartTracking,

    /// <summary>
    /// Purchase
    /// </summary>
    Purchase,

    /// <summary>
    /// Re-stock
    /// </summary>
    Restock,

    /// <summary>
    /// Void
    /// </summary>
    Void
}