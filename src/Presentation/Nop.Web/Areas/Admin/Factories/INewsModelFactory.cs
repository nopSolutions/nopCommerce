using System.Threading.Tasks;
using Nop.Core.Domain.News;
using Nop.Web.Areas.Admin.Models.News;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the news model factory
    /// </summary>
    public partial interface INewsModelFactory
    {
        /// <summary>
        /// Prepare news content model
        /// </summary>
        /// <param name="newsContentModel">News content model</param>
        /// <param name="filterByNewsItemId">Filter by news item ID</param>
        /// <returns>News content model</returns>
        Task<NewsContentModel> PrepareNewsContentModelAsync(NewsContentModel newsContentModel, int? filterByNewsItemId);

        /// <summary>
        /// Prepare paged news item list model
        /// </summary>
        /// <param name="searchModel">News item search model</param>
        /// <returns>News item list model</returns>
        Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsItemSearchModel searchModel);

        /// <summary>
        /// Prepare news item model
        /// </summary>
        /// <param name="model">News item model</param>
        /// <param name="newsItem">News item</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>News item model</returns>
        Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool excludeProperties = false);

        /// <summary>
        /// Prepare news comment search model
        /// </summary>
        /// <param name="searchModel">News comment search model</param>
        /// <param name="newsItem">News item</param>
        /// <returns>News comment search model</returns>
        Task<NewsCommentSearchModel> PrepareNewsCommentSearchModelAsync(NewsCommentSearchModel searchModel, NewsItem newsItem);

        /// <summary>
        /// Prepare paged news comment list model
        /// </summary>
        /// <param name="searchModel">News comment search model</param>
        /// <param name="newsItemId">News item ID; pass null to prepare comment models for all news items</param>
        /// <returns>News comment list model</returns>
        Task<NewsCommentListModel> PrepareNewsCommentListModelAsync(NewsCommentSearchModel searchModel, int? newsItemId);
    }
}