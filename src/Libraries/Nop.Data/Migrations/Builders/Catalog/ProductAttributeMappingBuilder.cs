using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductAttributeMappingBuilder : BaseEntityBuilder<ProductAttributeMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductAttributeMapping.ProductAttributeId))
                    .AsInt32()
                    .ForeignKey<ProductAttribute>()
                .WithColumn(nameof(ProductAttributeMapping.ProductId))
                .AsInt32()
                .ForeignKey<Product>();
        }

        #endregion
    }
}