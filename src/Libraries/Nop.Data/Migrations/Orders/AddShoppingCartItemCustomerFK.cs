using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097813093371767)]
    public class AddShoppingCartItemCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ShoppingCartItem)
                , nameof(ShoppingCartItem.CustomerId)
                , nameof(Customer)
                , nameof(Customer.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}