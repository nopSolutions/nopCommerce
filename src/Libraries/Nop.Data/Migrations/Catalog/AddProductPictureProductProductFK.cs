using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097627662625750)]
    public class AddProductPictureProductProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductPictureTable
                , nameof(ProductPicture.ProductId)
                , nameof(Product)
                , nameof(Product.Id)
                , Rule.Cascade);
        }
        
        #endregion
    }
}