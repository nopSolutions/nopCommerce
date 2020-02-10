using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647939")]
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