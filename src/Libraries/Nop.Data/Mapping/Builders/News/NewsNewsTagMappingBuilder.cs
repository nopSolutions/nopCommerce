using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.News
{
    /// <summary>
    /// Represents a news item entity builder
    /// </summary>
    public partial class NewsNewsTagMappingBuilder : NopEntityBuilder<NewsNewsTagMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(NewsNewsTagMapping), nameof(NewsNewsTagMapping.NewsId)))
                    .AsInt32().PrimaryKey().ForeignKey<NewsItem>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(NewsNewsTagMapping), nameof(NewsNewsTagMapping.NewsTagId)))
                    .AsInt32().PrimaryKey().ForeignKey<NewsTags>()
                ;
        }

        #endregion
    }
}