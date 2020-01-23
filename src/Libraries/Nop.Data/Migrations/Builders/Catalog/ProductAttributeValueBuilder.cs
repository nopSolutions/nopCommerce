using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductAttributeValueBuilder : BaseEntityBuilder<ProductAttributeValue>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(ProductAttributeValue.ColorSquaresRgb)).AsString(100).Nullable()
                .WithColumn(nameof(ProductAttributeValue.ProductAttributeMappingId))
                    .AsInt32()
                    .ForeignKey<ProductAttributeMapping>();
        }

        #endregion
    }
}