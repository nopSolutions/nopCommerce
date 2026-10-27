using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.News.Domain;

namespace Nop.Plugin.Misc.News.Data.Mapping.Builders;

/// <summary>
/// Represents a news comment entity builder
/// </summary>
public class NewsCommentBuilder : NopEntityBuilder<NewsComment>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(NewsComment.CustomerId)).AsInt32().Nullable().ForeignKey<Customer>(onDelete: Rule.SetNull)
            .WithColumn(nameof(NewsComment.NewsItemId)).AsInt32().ForeignKey<NewsItem>()
            .WithColumn(nameof(NewsComment.StoreId)).AsInt32().ForeignKey<Store>();
    }

    #endregion
}