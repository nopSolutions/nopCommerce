using Nop.Core.Domain.News;
using Nop.Web.Models.News;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the news model factory
    /// </summary>
    public partial interface INewsModelFactory
    {
        /// <summary>
        /// Prepare the news comment model
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <returns>News comment model</returns>
        NewsCommentModel PrepareNewsCommentModel(NewsComment newsComment);

        /// <summary>
        /// Prepare the news item model
        /// </summary>
        /// <param name="model">News item model</param>
        /// <param name="newsItem">News item</param>
        /// <param name="prepareComments">Whether to prepare news comment models</param>
        /// <returns>News item model</returns>
        NewsItemModel PrepareNewsItemModel(NewsItemModel model, NewsItem newsItem, bool prepareComments);

        /// <summary>
        /// Prepare the home page news items model
        /// </summary>
        /// <returns>Home page news items model</returns>
        HomepageNewsItemsModel PrepareHomepageNewsItemsModel();

        /// <summary>
        /// Prepare the news item list model
        /// </summary>
        /// <param name="command">News paging filtering model</param>
        /// <returns>News item list model</returns>
        NewsItemListModel PrepareNewsItemListModel(NewsPagingFilteringModel command);
    }
}
