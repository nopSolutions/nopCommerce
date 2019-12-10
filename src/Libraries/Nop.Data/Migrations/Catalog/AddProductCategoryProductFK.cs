using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097618625689397)]
    public class AddProductCategoryProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductCategoryTable)
                .ForeignColumn(nameof(ProductCategory.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));

            Create.Index().OnTable(NopMappingDefaults.ProductCategoryTable).OnColumn(nameof(ProductCategory.ProductId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}