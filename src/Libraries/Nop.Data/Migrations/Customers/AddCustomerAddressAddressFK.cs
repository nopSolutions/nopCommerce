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
                .ForeignColumn("Address_Id")
                .ToTable(nameof(Address))
                .PrimaryColumn(nameof(Address.Id));

            Create.Index().OnTable(NopMappingDefaults.CustomerAddressesTable).OnColumn("Address_Id").Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}