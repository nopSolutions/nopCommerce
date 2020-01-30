using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 01:09:03:8051845")]
    public class AddTierPriceCustomerRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(TierPrice), 
                nameof(TierPrice.CustomerRoleId), 
                nameof(CustomerRole), 
                nameof(CustomerRole.Id), 
                Rule.Cascade);
        }
        
        #endregion
    }
}