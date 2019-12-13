using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097703237489896)]
    public class AddCustomerCustomerRoleCustomerFK : AutoReversingMigration
    {
        #region Methods
        
        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.CustomerCustomerRoleTable
                , "Customer_Id"
                , nameof(Customer)
                , nameof(Customer.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}