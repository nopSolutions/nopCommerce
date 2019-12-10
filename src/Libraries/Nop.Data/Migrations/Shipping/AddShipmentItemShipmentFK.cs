using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Migrations.Shipping
{
    [Migration(637097820921984734)]
    public class AddShipmentItemShipmentFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ShipmentItem))
                .ForeignColumn(nameof(ShipmentItem.ShipmentId))
                .ToTable(nameof(Shipment))
                .PrimaryColumn(nameof(Shipment.Id))
                .OnDelete(Rule.Cascade);

            Create.Index().OnTable(nameof(ShipmentItem)).OnColumn(nameof(ShipmentItem.ShipmentId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}