using Nop.Web.Areas.Admin.Models.Home;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the home models factory
    /// </summary>
    public partial interface IHomeModelFactory
    {
        /// <summary>
        /// Prepare dashboard model
        /// </summary>
        /// <param name="model">Dashboard model</param>
        /// <returns>Dashboard model</returns>
        DashboardModel PrepareDashboardModel(DashboardModel model);

        /// <summary>
        /// Prepare popular search term search model
        /// </summary>
        /// <param name="model">Popular search term search model</param>
        /// <returns>Popular search term search model</returns>
        PopularSearchTermSearchModel PreparePopularSearchTermSearchModel(PopularSearchTermSearchModel model);

        /// <summary>
        /// Prepare paged popular search term list model
        /// </summary>
        /// <param name="searchModel">Popular search term search model</param>
        /// <returns>Popular search term list model</returns>
        PopularSearchTermListModel PreparePopularSearchTermListModel(PopularSearchTermSearchModel searchModel);

        /// <summary>
        /// Prepare nopCommerce news model
        /// </summary>
        /// <returns>nopCommerce news model</returns>
        NopCommerceNewsModel PrepareNopCommerceNewsModel();

        /// <summary>
        /// Prepare common statistics model
        /// </summary>
        /// <returns>Common statistics model</returns>
        CommonStatisticsModel PrepareCommonStatisticsModel();
    }
}