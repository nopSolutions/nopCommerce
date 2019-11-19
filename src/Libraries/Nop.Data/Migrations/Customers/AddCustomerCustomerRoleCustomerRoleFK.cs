using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097703237489897)]
    public class AddCustomerCustomerRoleCustomerRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.CustomerCustomerRoleTable)
                .ForeignColumn(nameof(CustomerCustomerRoleMapping.CustomerRoleId))
                .ToTable(nameof(CustomerRole))
                .PrimaryColumn(nameof(CustomerRole.Id));
        }

        #endregion
    }
}