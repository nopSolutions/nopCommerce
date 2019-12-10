using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097805896028942)]
    public class AddOrderCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Order))
                .ForeignColumn(nameof(Order.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));

            Create.Index().OnTable(nameof(Order)).OnColumn(nameof(Order.CustomerId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}