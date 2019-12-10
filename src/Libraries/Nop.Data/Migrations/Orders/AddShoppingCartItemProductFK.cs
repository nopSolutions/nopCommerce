using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097813093371768)]
    public class AddShoppingCartItemProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ShoppingCartItem))
                .ForeignColumn(nameof(ShoppingCartItem.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));

            Create.Index().OnTable(nameof(ShoppingCartItem)).OnColumn(nameof(ShoppingCartItem.ProductId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}