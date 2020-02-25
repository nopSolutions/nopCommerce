using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:07:33:9067594")]
    public class AddProductManufacturerManufacturerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductManufacturerTable,
                nameof(ProductManufacturer.ManufacturerId),
                nameof(Manufacturer),
                nameof(Manufacturer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}