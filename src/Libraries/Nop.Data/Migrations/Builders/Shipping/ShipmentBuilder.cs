using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ShipmentBuilder : BaseEntityBuilder<Shipment>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Shipment.OrderId))
                    .AsInt32()
                    .ForeignKey<Order>();
        }

        #endregion
    }
}