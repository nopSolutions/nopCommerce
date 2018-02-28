using System.Collections.Generic;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Kendoui;

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
        /// <param name="model">Activity log search model</param>
        /// <returns>Activity log search model</returns>
        ActivityLogSearchModel PrepareActivityLogSearchModel(ActivityLogSearchModel model);

        /// <summary>
        /// Prepare paged activity log list model for the grid
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <param name="command">Pagination parameters</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareActivityLogListGridModel(ActivityLogSearchModel searchModel, DataSourceRequest command);
    }
}