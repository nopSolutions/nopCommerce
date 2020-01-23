using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductBuilder : BaseEntityBuilder<Product>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {

            table
                .WithColumn(nameof(Product.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Product.MetaKeywords)).AsString(400).Nullable()
                .WithColumn(nameof(Product.MetaTitle)).AsString(400).Nullable()
                .WithColumn(nameof(Product.Sku)).AsString(400).Nullable()
                .WithColumn(nameof(Product.ManufacturerPartNumber)).AsString(400).Nullable()
                .WithColumn(nameof(Product.Gtin)).AsString(400).Nullable()
                .WithColumn(nameof(Product.RequiredProductIds)).AsString(1000).Nullable()
                .WithColumn(nameof(Product.AllowedQuantities)).AsString(1000).Nullable();
        }

        #endregion
    }
}