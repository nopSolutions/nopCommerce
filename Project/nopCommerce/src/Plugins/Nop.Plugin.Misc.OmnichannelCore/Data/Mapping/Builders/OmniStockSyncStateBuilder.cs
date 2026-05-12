using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.OmnichannelCore.Domains;

namespace Nop.Plugin.Misc.OmnichannelCore.Data.Mapping.Builders;

/// <summary>
/// Represents a stock synchronization state entity builder
/// </summary>
public class OmniStockSyncStateBuilder : NopEntityBuilder<OmniStockSyncState>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(OmniStockSyncState.ProductId)).AsInt32().NotNullable().Indexed()
            .WithColumn(nameof(OmniStockSyncState.Sku)).AsString(400).Nullable().Indexed()
            .WithColumn(nameof(OmniStockSyncState.LastMessageId)).AsGuid().Nullable().Indexed()
            .WithColumn(nameof(OmniStockSyncState.Source)).AsString(100).Nullable();
    }

    #endregion
}
