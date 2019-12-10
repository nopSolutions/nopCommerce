using FluentMigrator;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097804609436788)]
    public class AddOrderItemOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(OrderItem))
                .ForeignColumn(nameof(OrderItem.OrderId))
                .ToTable(nameof(Order))
                .PrimaryColumn(nameof(Order.Id));

            Create.Index().OnTable(nameof(OrderItem)).OnColumn(nameof(OrderItem.OrderId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}