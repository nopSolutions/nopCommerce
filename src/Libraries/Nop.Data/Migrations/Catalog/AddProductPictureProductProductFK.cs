using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097627662625750)]
    public class AddProductPictureProductProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductPictureTable)
                .ForeignColumn(nameof(ProductPicture.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));

            Create.Index().OnTable(NopMappingDefaults.ProductPictureTable).OnColumn(nameof(ProductPicture.ProductId)).Ascending().WithOptions().NonClustered();
        }
        
        #endregion
    }
}