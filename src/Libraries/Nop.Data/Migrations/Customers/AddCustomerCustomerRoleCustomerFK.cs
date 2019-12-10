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
                .ForeignColumn("Customer_Id")
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));

            Create.Index().OnTable(NopMappingDefaults.CustomerCustomerRoleTable).OnColumn("Customer_Id").Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}