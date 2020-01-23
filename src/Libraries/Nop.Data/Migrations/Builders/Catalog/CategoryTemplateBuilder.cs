using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Builders
{
    public class CategoryTemplateBuilder : BaseEntityBuilder<CategoryTemplate>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table) 
        {
            table
                .WithColumn(nameof(CategoryTemplate.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(CategoryTemplate.ViewPath)).AsString(400).NotNullable();
        }

        #endregion
    }
}