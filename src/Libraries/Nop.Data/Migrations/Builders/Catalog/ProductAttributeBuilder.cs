using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductAttributeBuilder : BaseEntityBuilder<ProductAttribute>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ProductAttribute.Name)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}