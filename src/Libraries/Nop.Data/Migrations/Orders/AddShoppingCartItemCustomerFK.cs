using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097813093371767)]
    public class AddShoppingCartItemCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ShoppingCartItem))
                .ForeignColumn(nameof(ShoppingCartItem.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));
        }

        #endregion
    }
}