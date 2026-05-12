using Nop.Data.Mapping;
using Nop.Plugin.Misc.OmnichannelCore.Domains;

namespace Nop.Plugin.Misc.OmnichannelCore.Data.Mapping;

/// <summary>
/// Plugin table naming compatibility
/// </summary>
public class OmnichannelCoreNameCompatibility : INameCompatibility
{
    /// <summary>
    /// Gets the table names for plugin entities
    /// </summary>
    public Dictionary<Type, string> TableNames => new()
    {
        [typeof(OmniOutboxMessage)] = "OmniOutboxMessage",
        [typeof(OmniInboxMessage)] = "OmniInboxMessage",
        [typeof(OmniOrderFulfillment)] = "OmniOrderFulfillment",
        [typeof(OmniStockSyncState)] = "OmniStockSyncState"
    };

    /// <summary>
    /// Gets the column name overrides
    /// </summary>
    public Dictionary<(Type, string), string> ColumnName => new();
}
