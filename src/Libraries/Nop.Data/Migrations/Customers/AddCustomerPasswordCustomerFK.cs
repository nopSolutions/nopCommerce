using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097707461276491)]
    public class AddCustomerPasswordCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(CustomerPassword))
                .ForeignColumn(nameof(CustomerPassword.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));

            Create.Index().OnTable(nameof(CustomerPassword)).OnColumn(nameof(CustomerPassword.CustomerId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}