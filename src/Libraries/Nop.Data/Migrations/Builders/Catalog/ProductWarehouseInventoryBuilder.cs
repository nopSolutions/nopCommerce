using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductWarehouseInventoryBuilder : BaseEntityBuilder<ProductWarehouseInventory>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductWarehouseInventory.ProductId))
                    .AsInt32()
                    .ForeignKey<Product>()
                .WithColumn(nameof(ProductWarehouseInventory.WarehouseId))
                    .AsInt32()
                    .ForeignKey<Warehouse>();
        }

        #endregion
    }
}