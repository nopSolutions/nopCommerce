using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097703237489897)]
    public class AddCustomerCustomerRoleCustomerRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.CustomerCustomerRoleTable
                , "CustomerRole_Id"
                , nameof(CustomerRole)
                , nameof(CustomerRole.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}