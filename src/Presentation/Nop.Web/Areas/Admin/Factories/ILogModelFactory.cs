using Nop.Core.Domain.Logging;
using Nop.Web.Areas.Admin.Models.Logging;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the log model factory
    /// </summary>
    public partial interface ILogModelFactory
    {
        /// <summary>
        /// Prepare log search model
        /// </summary>
        /// <param name="searchModel">Log search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the log search model
        /// </returns>
        Task<LogSearchModel> PrepareLogSearchModelAsync(LogSearchModel searchModel);

        /// <summary>
        /// Prepare paged log list model
        /// </summary>
        /// <param name="searchModel">Log search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the log list model
        /// </returns>
        Task<LogListModel> PrepareLogListModelAsync(LogSearchModel searchModel);

        /// <summary>
        /// Prepare log model
        /// </summary>
        /// <param name="model">Log model</param>
        /// <param name="log">Log</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the log model
        /// </returns>
        Task<LogModel> PrepareLogModelAsync(LogModel model, Log log, bool excludeProperties = false);
    }
}