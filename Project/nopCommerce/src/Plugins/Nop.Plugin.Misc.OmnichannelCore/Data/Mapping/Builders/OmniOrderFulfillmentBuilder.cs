using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.OmnichannelCore.Domains;

namespace Nop.Plugin.Misc.OmnichannelCore.Data.Mapping.Builders;

/// <summary>
/// Represents an order fulfillment entity builder
/// </summary>
public class OmniOrderFulfillmentBuilder : NopEntityBuilder<OmniOrderFulfillment>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(OmniOrderFulfillment.OrderGuid)).AsGuid().NotNullable().Indexed()
            .WithColumn(nameof(OmniOrderFulfillment.MessageId)).AsGuid().Nullable().Indexed()
            .WithColumn(nameof(OmniOrderFulfillment.ExternalRequestId)).AsString(100).Nullable().Indexed()
            .WithColumn(nameof(OmniOrderFulfillment.TrackingNumber)).AsString(100).Nullable()
            .WithColumn(nameof(OmniOrderFulfillment.Reason)).AsString(int.MaxValue).Nullable();
    }

    #endregion
}
