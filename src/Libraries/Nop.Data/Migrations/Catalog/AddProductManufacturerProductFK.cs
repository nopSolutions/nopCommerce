using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097620539067595)]
    public class AddProductManufacturerProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductManufacturerTable)
                .ForeignColumn(nameof(ProductManufacturer.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));
        }

        #endregion
    }
}