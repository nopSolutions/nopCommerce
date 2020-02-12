using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Customers
{
    [NopMigration("2019/11/19 02:25:23:7489896")]
    public class AddCustomerCustomerRoleCustomerFK : AutoReversingMigration
    {
        #region Methods
        
        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.CustomerCustomerRoleTable,
                "Customer_Id",
                nameof(Customer),
                nameof(Customer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}