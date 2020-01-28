using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Shipping
{
    [NopMigration("2019/11/19 05:41:32:1984734")]
    public class AddShipmentItemShipmentFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ShipmentItem),
                nameof(ShipmentItem.ShipmentId),
                nameof(Shipment),
                nameof(Shipment.Id),
                Rule.Cascade);
        }

        #endregion
    }
}