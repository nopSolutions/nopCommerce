using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.News.Domain;

namespace Nop.Plugin.Misc.News.Data.Mapping.Builders;

/// <summary>
/// Represents a news item entity builder
/// </summary>
public class NewsItemBuilder : NopEntityBuilder<NewsItem>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(NewsItem.Title)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(NewsItem.Short)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(NewsItem.Full)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(NewsItem.MetaKeywords)).AsString(400).Nullable()
            .WithColumn(nameof(NewsItem.MetaTitle)).AsString(400).Nullable()
            .WithColumn(nameof(NewsItem.LanguageId)).AsInt32().ForeignKey<Language>();
    }

    #endregion
}