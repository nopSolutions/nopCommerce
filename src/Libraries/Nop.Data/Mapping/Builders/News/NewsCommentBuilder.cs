using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.News;

/// <summary>
/// Represents a news comment entity builder
/// </summary>
public partial class NewsCommentBuilder : NopEntityBuilder<NewsComment>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(NewsComment.CustomerId)).AsInt32().ForeignKey<Customer>()
            .WithColumn(nameof(NewsComment.NewsItemId)).AsInt32().ForeignKey<NewsItem>()
            .WithColumn(nameof(NewsComment.StoreId)).AsInt32().ForeignKey<Store>();
    }

    #endregion
}