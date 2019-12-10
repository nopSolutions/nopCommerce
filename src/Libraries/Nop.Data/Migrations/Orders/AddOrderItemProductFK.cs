using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097804609436789)]
    public class AddOrderItemProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(OrderItem))
                .ForeignColumn(nameof(OrderItem.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));

            Create.Index().OnTable(nameof(OrderItem)).OnColumn(nameof(OrderItem.ProductId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}