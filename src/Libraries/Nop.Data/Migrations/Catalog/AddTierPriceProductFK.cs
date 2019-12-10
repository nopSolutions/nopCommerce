using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097657438051844)]
    public class AddTierPriceProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(TierPrice))
                .ForeignColumn(nameof(TierPrice.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));

            Create.Index().OnTable(nameof(TierPrice)).OnColumn(nameof(TierPrice.ProductId)).Ascending().WithOptions().NonClustered();
        }
        
        #endregion
    }
}