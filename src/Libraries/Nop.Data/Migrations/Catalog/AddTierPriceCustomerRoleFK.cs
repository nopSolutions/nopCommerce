using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097657438051845)]
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