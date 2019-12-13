using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Shipping
{
    [Migration(637097820921984734)]
    public class AddShipmentItemShipmentFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ShipmentItem)
                , nameof(ShipmentItem.ShipmentId)
                , nameof(Shipment)
                , nameof(Shipment.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}