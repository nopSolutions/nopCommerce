using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:19:26:2625749")]
    public class AddProductPictureProductPictureFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductPictureTable,
                nameof(ProductPicture.PictureId),
                nameof(Picture),
                nameof(Picture.Id),
                Rule.Cascade);
        }

        #endregion
    }
}