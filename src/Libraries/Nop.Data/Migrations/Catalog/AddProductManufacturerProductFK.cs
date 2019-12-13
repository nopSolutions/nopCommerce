using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097620539067595)]
    public class AddProductManufacturerProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductManufacturerTable
                , nameof(ProductManufacturer.ProductId)
                , nameof(Product)
                , nameof(Product.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}