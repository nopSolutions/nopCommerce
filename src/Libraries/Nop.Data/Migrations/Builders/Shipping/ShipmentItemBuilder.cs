using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ShipmentItemBuilder : BaseEntityBuilder<ShipmentItem>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShipmentItem.ShipmentId))
                    .AsInt32()
                    .ForeignKey<Shipment>();
        }

        #endregion
    }
}