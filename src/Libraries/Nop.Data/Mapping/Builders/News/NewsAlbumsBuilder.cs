using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.News
{
    /// <summary>
    /// Represents a news item entity builder
    /// </summary>
    public partial class NewsAlbumsBuilder : NopEntityBuilder<NewsAlbums>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(NewsAlbums.NewsId)).AsInt32().ForeignKey<NewsItem>()
                .WithColumn(nameof(NewsAlbums.ThumbPath)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(NewsAlbums.OriginalPath)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(NewsAlbums.Remark)).AsString(64).Nullable()
                ;
        }

        #endregion
    }
}