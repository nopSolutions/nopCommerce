using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:28:29:3371768")]
    public class AddShoppingCartItemProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ShoppingCartItem),
                nameof(ShoppingCartItem.ProductId),
                nameof(Product),
                nameof(Product.Id),
                Rule.Cascade);
        }

        #endregion
    }
}