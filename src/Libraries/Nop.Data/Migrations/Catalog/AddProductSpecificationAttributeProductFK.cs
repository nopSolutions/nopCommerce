using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097645462261986)]
    public class AddProductSpecificationAttributeProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductSpecificationAttributeTable)
                .ForeignColumn(nameof(ProductSpecificationAttribute.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));

            Create.Index().OnTable(NopMappingDefaults.ProductSpecificationAttributeTable).OnColumn(nameof(ProductSpecificationAttribute.ProductId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}