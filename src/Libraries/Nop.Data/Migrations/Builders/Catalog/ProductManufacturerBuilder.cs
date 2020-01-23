using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductManufacturerBuilder : BaseEntityBuilder<ProductManufacturer>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductManufacturer.ManufacturerId))
                    .AsInt32()
                    .ForeignKey<Manufacturer>()
                .WithColumn(nameof(ProductManufacturer.ProductId))
                    .AsInt32()
                    .ForeignKey<Product>();
        }

        #endregion
    }
}