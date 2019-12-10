using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Migrations.Shipping
{
    [Migration(637097821681126845)]
    public class AddShipmentOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Shipment))
                .ForeignColumn(nameof(Shipment.OrderId))
                .ToTable(nameof(Order))
                .PrimaryColumn(nameof(Order.Id))
                .OnDelete(Rule.Cascade);

            Create.Index().OnTable(nameof(Shipment)).OnColumn(nameof(Shipment.OrderId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}