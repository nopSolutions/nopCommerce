using FluentMigrator;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647939)]
    public class AddCustomerCustomerRoleMappingCustomerIdIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Customer_CustomerRole_Mapping_Customer_Id", NopMappingDefaults.CustomerCustomerRoleTable,
                i => i.Ascending(), "Customer_Id");
        }

        #endregion
    }
}