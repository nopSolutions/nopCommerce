using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Orders;

/// <summary>
/// Represents a order note entity builder
/// </summary>
public partial class OrderNoteBuilder : NopEntityBuilder<OrderNote>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(OrderNote.Note)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(OrderNote.OrderId)).AsInt32().ForeignKey<Order>();
    }

    #endregion
}