using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097698595245359)]
    public class AddCustomerAddressAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.CustomerAddressesTable)
                .ForeignColumn(nameof(CustomerAddressMapping.AddressId))
                .ToTable(nameof(Address))
                .PrimaryColumn(nameof(Address.Id));
        }

        #endregion
    }
}