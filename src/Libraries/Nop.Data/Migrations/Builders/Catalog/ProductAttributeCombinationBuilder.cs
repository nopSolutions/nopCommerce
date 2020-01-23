using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductAttributeCombinationBuilder : BaseEntityBuilder<ProductAttributeCombination>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductAttributeCombination.Sku)).AsString(400).Nullable()
                .WithColumn(nameof(ProductAttributeCombination.ManufacturerPartNumber)).AsString(400).Nullable()
                .WithColumn(nameof(ProductAttributeCombination.Gtin)).AsString(400).Nullable()
                .WithColumn(nameof(ProductAttributeCombination.ProductId))
                    .AsInt32()
                    .ForeignKey<Product>();
        }

        #endregion
    }
}