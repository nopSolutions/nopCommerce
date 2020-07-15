using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.News
{
    /// <summary>
    /// Represents a news item entity builder
    /// </summary>
    public partial class NewsCategoryBuilder : NopEntityBuilder<NewsCategory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(NewsCategory.Title)).AsString(64).NotNullable()
                .WithColumn(nameof(NewsCategory.SubTitle)).AsString(512).Nullable()
                .WithColumn(nameof(NewsCategory.ClassList)).AsAnsiString(512).Nullable()
                .WithColumn(nameof(NewsCategory.ClassLayer)).AsAnsiString(512).Nullable()
                .WithColumn(nameof(NewsCategory.ImageUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(NewsCategory.MetaTitle)).AsString(256).Nullable()
                .WithColumn(nameof(NewsCategory.MetaDescription)).AsString(512).Nullable()
                .WithColumn(nameof(NewsCategory.MetaKeywords)).AsString(512).Nullable()
                .WithColumn(nameof(NewsCategory.ViewIndexTemplateName)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(NewsCategory.ViewListTemplateName)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(NewsCategory.ViewDetailTemplateName)).AsAnsiString(64).Nullable()
                ;
        }

        #endregion
    }
}