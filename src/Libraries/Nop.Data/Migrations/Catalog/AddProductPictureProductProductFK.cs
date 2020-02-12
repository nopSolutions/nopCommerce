using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:19:26:2625750")]
    public class AddProductPictureProductProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductPictureTable,
                nameof(ProductPicture.ProductId),
                nameof(Product),
                nameof(Product.Id),
                Rule.Cascade);
        }
        
        #endregion
    }
}