using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class OrderItemBuilder : BaseEntityBuilder<OrderItem>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderItem.OrderId))
                   .AsInt32()
                   .ForeignKey<Order>()
                .WithColumn(nameof(OrderItem.ProductId))
                   .AsInt32()
                   .ForeignKey<Product>();
        }

        #endregion
    }
}