using FluentMigrator;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 11:35:09:1647939")]
    public class AddCustomerCustomerRoleMappingCustomerIdIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            //Create.Index("IX_Customer_CustomerRole_Mapping_Customer_Id").OnTable(NameCompatibilityManager.GetTableName(typeof(CustomerCustomerRoleMapping)))
            //    .OnColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerCustomerRoleMapping), nameof(CustomerCustomerRoleMapping.CustomerId))).Ascending()
            //    .WithOptions().NonClustered();
        }

        #endregion
    }
}