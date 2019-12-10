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
                .ForeignColumn("BillingAddress_Id")
                .ToTable(nameof(Address))
                .PrimaryColumn(nameof(Address.Id));

            Create.Index().OnTable(nameof(Customer)).OnColumn("BillingAddress_Id").Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}