using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097804609436788)]
    public class AddOrderItemOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(OrderItem)
                , nameof(OrderItem.OrderId)
                , nameof(Order)
                , nameof(Order.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}