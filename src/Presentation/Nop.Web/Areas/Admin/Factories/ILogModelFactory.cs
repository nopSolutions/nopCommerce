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
        /// <returns>Log search model</returns>
        LogSearchModel PrepareLogSearchModel(LogSearchModel searchModel);

        /// <summary>
        /// Prepare paged log list model
        /// </summary>
        /// <param name="searchModel">Log search model</param>
        /// <returns>Log list model</returns>
        LogListModel PrepareLogListModel(LogSearchModel searchModel);

        /// <summary>
        /// Prepare log model
        /// </summary>
        /// <param name="model">Log model</param>
        /// <param name="log">Log</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Log model</returns>
        LogModel PrepareLogModel(LogModel model, Log log, bool excludeProperties = false);
    }
}