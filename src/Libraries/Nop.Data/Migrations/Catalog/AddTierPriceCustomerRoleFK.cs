using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097657438051845)]
    public class AddTierPriceCustomerRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(TierPrice))
                .ForeignColumn(nameof(TierPrice.CustomerRoleId))
                .ToTable(nameof(CustomerRole))
                .PrimaryColumn(nameof(CustomerRole.Id))
                .OnDelete(Rule.Cascade);

            Create.Index().OnTable(nameof(TierPrice)).OnColumn(nameof(TierPrice.CustomerRoleId)).Ascending().WithOptions().NonClustered();
        }
        
        #endregion
    }
}