using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductTagBuilder : BaseEntityBuilder<ProductTag>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ProductTag.Name)).AsString(4000).NotNullable();
        }

        #endregion
    }
}