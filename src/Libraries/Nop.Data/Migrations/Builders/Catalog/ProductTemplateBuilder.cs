using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductTemplateBuilder : BaseEntityBuilder<ProductTemplate>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductTemplate.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(ProductTemplate.ViewPath)).AsString(400).NotNullable();
        }

        #endregion
    }
}