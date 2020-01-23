using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class OrderNoteBuilder : BaseEntityBuilder<OrderNote>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderNote.Note)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(OrderNote.OrderId))
                   .AsInt32()
                   .ForeignKey<Order>();
        }

        #endregion
    }
}