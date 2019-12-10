using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097620539067594)]
    public class AddProductManufacturerManufacturerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductManufacturerTable)
                .ForeignColumn(nameof(ProductManufacturer.ManufacturerId))
                .ToTable(nameof(Manufacturer))
                .PrimaryColumn(nameof(Manufacturer.Id));

            Create.Index().OnTable(NopMappingDefaults.ProductManufacturerTable).OnColumn(nameof(ProductManufacturer.ManufacturerId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}