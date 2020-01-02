using Nop.Web.Areas.Admin.Models.Logging;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the activity log model factory
    /// </summary>
    public partial interface IActivityLogModelFactory
    {
        /// <summary>
        /// Prepare activity log search model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>Activity log search model</returns>
        ActivityLogSearchModel PrepareActivityLogSearchModel(ActivityLogSearchModel searchModel);

        /// <summary>
        /// Prepare activity log types search model
        /// </summary>
        /// <param name="searchModel">Activity log types search model</param>
        /// <returns>Activity log types search model</returns>
        ActivityLogTypeSearchModel PrepareActivityLogTypeSearchModel(ActivityLogTypeSearchModel searchModel);

        /// <summary>
        /// Prepare paged activity log list model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>Activity log list model</returns>
        ActivityLogListModel PrepareActivityLogListModel(ActivityLogSearchModel searchModel);
    }
}