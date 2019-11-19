using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097705651641381)]
    public class AddCustomerBillingAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Customer))
                .ForeignColumn(nameof(Customer.BillingAddressId))
                .ToTable(nameof(Address))
                .PrimaryColumn(nameof(Address.Id));
        }

        #endregion
    }
}