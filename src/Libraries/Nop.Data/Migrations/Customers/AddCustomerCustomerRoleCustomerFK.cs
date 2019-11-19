using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097703237489896)]
    public class AddCustomerCustomerRoleCustomerFK : AutoReversingMigration
    {
        #region Methods
        
        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.CustomerCustomerRoleTable)
                .ForeignColumn(nameof(CustomerCustomerRoleMapping.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));
        }

        #endregion
    }
}