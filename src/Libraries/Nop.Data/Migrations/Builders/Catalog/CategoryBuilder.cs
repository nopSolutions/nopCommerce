using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Builders
{
    public partial class CategoryBuilder : BaseEntityBuilder<Category>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Category.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Category.MetaKeywords)).AsString(400).Nullable()
                .WithColumn(nameof(Category.MetaTitle)).AsString(400).Nullable()
                .WithColumn(nameof(Category.PriceRanges)).AsString(400).Nullable()
                .WithColumn(nameof(Category.PageSizeOptions)).AsString(400).Nullable();
        }

        #endregion
    }
}