using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Shipping
{
    [NopMigration("2019/11/19 05:42:48:1126845")]
    public class AddShipmentOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Shipment),
                nameof(Shipment.OrderId),
                nameof(Order),
                nameof(Order.Id),
                Rule.Cascade);
        }

        #endregion
    }
}