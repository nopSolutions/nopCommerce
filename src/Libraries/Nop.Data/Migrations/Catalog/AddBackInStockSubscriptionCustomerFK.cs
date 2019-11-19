using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097608748261631)]
    public  class AddBackInStockSubscriptionCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(BackInStockSubscription))
                .ForeignColumn(nameof(BackInStockSubscription.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));
        }

        #endregion
    }
}