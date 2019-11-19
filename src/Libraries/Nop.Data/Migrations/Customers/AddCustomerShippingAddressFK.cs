using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097705651641382)]
    public class AddCustomerShippingAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Customer))
                .ForeignColumn(nameof(Customer.ShippingAddressId))
                .ToTable(nameof(Address))
                .PrimaryColumn(nameof(Address.Id));
        }

        #endregion
    }
}