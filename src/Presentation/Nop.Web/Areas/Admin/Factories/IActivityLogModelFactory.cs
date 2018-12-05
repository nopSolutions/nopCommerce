using System.Collections.Generic;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.DataTables;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the activity log model factory
    /// </summary>
    public partial interface IActivityLogModelFactory
    {
        /// <summary>
        /// Prepare activity log type models
        /// </summary>
        /// <returns>List of activity log type models</returns>
        IList<ActivityLogTypeModel> PrepareActivityLogTypeModels();

        /// <summary>
        /// Prepare activity log search model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>Activity log search model</returns>
        ActivityLogSearchModel PrepareActivityLogSearchModel(ActivityLogSearchModel searchModel);

        /// <summary>
        /// Prepare activity log type datatables models
        /// </summary>
        /// <param name="searchModel">Activity log types search model</param>
        /// <returns>Datatables model</returns>
        DataTablesModel PrepareActivityLogTypeGridModel(ActivityLogTypeSearchModel searchModel);

        /// <summary>
        /// Prepare activity log types search model
        /// </summary>
        /// <param name="searchModel">Activity log types search model</param>
        /// <returns>Activity log types search model</returns>
        ActivityLogTypeSearchModel PrepareActivityLogTypeSearchModel(ActivityLogTypeSearchModel searchModel);

        /// <summary>
        /// Prepare activity log datatables models
        /// </summary>
        /// <param name="searchModel">Activity log types search model</param>
        /// <returns>Datatables model</returns>
        DataTablesModel PrepareActivityLogGridModel(ActivityLogSearchModel searchModel);

        /// <summary>
        /// Prepare paged activity log list model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>Activity log list model</returns>
        ActivityLogListModel PrepareActivityLogListModel(ActivityLogSearchModel searchModel);
    }
}