namespace Nop.Plugin.Misc.OmnichannelCore.Domains;

/// <summary>
/// Represents cross-channel stock synchronization status
/// </summary>
public enum OmniStockSyncStatus
{
    /// <summary>
    /// Projection reflects the latest accepted source version
    /// </summary>
    Current = 10,

    /// <summary>
    /// Update was ignored because it was older than the stored source version
    /// </summary>
    StaleIgnored = 20,

    /// <summary>
    /// Projection has a detected difference requiring reconciliation
    /// </summary>
    DriftDetected = 30
}
