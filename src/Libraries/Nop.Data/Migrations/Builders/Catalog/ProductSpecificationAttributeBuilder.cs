using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductSpecificationAttributeBuilder : BaseEntityBuilder<ProductSpecificationAttribute>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductSpecificationAttribute.CustomValue)).AsString(4000).Nullable()
                .WithColumn(nameof(ProductSpecificationAttribute.ProductId))
                    .AsInt32()
                    .ForeignKey<Product>()
                .WithColumn(nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId))
                    .AsInt32()
                    .ForeignKey<SpecificationAttributeOption>();
        }

        #endregion
    }
}