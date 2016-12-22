using Nop.Core.Domain.News;
using Nop.Web.Models.News;

namespace Nop.Web.Factories
{
    public partial interface INewsModelFactory
    {
        NewsCommentModel PrepareNewsCommentModel(NewsComment newsComment);

        NewsItemModel PrepareNewsItemModel(NewsItemModel model, NewsItem newsItem, bool prepareComments);

        HomePageNewsItemsModel PrepareHomePageNewsItemsModel();

        NewsItemListModel PrepareNewsItemListModel(NewsPagingFilteringModel command);
    }
}
