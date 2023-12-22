using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Framework.Models.DataTables;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the home models factory
/// </summary>
public partial interface IHomeModelFactory
{
    /// <summary>
    /// Prepare dashboard model
    /// </summary>
    /// <param name="model">Dashboard model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    Task<DashboardModel> PrepareDashboardModelAsync(DashboardModel model);

    /// <summary>
    /// Prepare popular search term report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    Task<DataTablesModel> PreparePopularSearchTermReportModelAsync(DataTablesModel model);

    /// <summary>
    /// Prepare bestsellers brief by amount report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    Task<DataTablesModel> PrepareBestsellersBriefReportByAmountModelAsync(DataTablesModel model);

    /// <summary>
    /// Prepare bestsellers brief by quantity report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    Task<DataTablesModel> PrepareBestsellersBriefReportByQuantityModelAsync(DataTablesModel model);

    /// <summary>
    /// Prepare latest orders model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    Task<DataTablesModel> PrepareLatestOrdersModelAsync(DataTablesModel model);

    /// <summary>
    /// Prepare nopCommerce news model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the nopCommerce news model
    /// </returns>
    Task<NopCommerceNewsModel> PrepareNopCommerceNewsModelAsync();

    /// <summary>
    /// Prepare incomplete orders report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    Task<DataTablesModel> PrepareOrderIncompleteModelAsync(DataTablesModel model);

    /// <summary>
    /// Prepare order average report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    Task<DataTablesModel> PrepareOrderAverageModelAsync(DataTablesModel model);
}