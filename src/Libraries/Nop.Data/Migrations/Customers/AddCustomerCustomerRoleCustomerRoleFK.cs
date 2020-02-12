using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Customers
{
    [NopMigration("2019/11/19 02:25:23:7489897")]
    public class AddCustomerCustomerRoleCustomerRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.CustomerCustomerRoleTable,
                "CustomerRole_Id",
                nameof(CustomerRole),
                nameof(CustomerRole.Id),
                Rule.Cascade);
        }

        #endregion
    }
}