using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097620539067594)]
    public class AddProductManufacturerManufacturerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductManufacturerTable
                , nameof(ProductManufacturer.ManufacturerId)
                , nameof(Manufacturer)
                , nameof(Manufacturer.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}