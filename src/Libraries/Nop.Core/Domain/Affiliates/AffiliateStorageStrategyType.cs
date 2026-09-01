namespace Nop.Core.Domain.Affiliates;

/// <summary>
/// Affiliate storage strategy type
/// </summary>
public enum AffiliateStorageStrategyType
{
    /// <summary>
    /// If an affiliate already exists, it will not be overwritten
    /// </summary>
    NoOverwrites,
    /// <summary>
    /// Always overwrite existing affiliates with new ones
    /// </summary>
    AlwaysOverwrite,
    /// <summary>
    /// Only overwrite existing affiliates if an order
    /// </summary>
    OverwriteAfterOrder
}