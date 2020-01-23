using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductProductTagMappingBuilder : BaseEntityBuilder<ProductProductTagMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductProductTagMapping.ProductId))
                    .AsInt32()
                    .PrimaryKey()
                    .ForeignKey<Product>()
                .WithColumn(nameof(ProductProductTagMapping.ProductTagId))
                    .AsInt32()
                    .PrimaryKey()
                    .ForeignKey<ProductTag>();
        }

        #endregion
    }
}