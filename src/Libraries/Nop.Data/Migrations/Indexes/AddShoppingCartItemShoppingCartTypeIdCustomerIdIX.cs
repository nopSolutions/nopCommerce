using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037691")]
    public class AddShoppingCartItemShoppingCartTypeIdCustomerIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_ShoppingCartItem_ShoppingCartTypeId_CustomerId", nameof(ShoppingCartItem),
                i => i.Ascending(), nameof(ShoppingCartItem.ShoppingCartTypeId), nameof(ShoppingCartItem.CustomerId));
        }

        #endregion
    }
}