using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097627662625749)]
    public class AddProductPictureProductPictureFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductPictureTable)
                .ForeignColumn(nameof(ProductPicture.PictureId))
                .ToTable(nameof(Picture))
                .PrimaryColumn(nameof(Picture.Id));


            Create.Index().OnTable(NopMappingDefaults.ProductPictureTable).OnColumn(nameof(ProductPicture.PictureId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}