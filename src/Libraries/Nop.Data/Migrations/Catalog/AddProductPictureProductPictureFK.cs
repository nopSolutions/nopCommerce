using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097627662625749)]
    public class AddProductPictureProductPictureFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductPictureTable
                , nameof(ProductPicture.PictureId)
                , nameof(Picture)
                , nameof(Picture.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}