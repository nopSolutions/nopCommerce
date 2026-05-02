using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Builders.Shipping;

/// <summary>
/// Represents a delivery date entity builder
/// </summary>
public partial class DeliveryDateBuilder : NopEntityBuilder<DeliveryDate>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(DeliveryDate.Name)).AsString(400).NotNullable();
    }

    #endregion
}