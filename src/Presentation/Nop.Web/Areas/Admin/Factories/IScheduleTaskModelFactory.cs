using Nop.Web.Areas.Admin.Models.Tasks;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the schedule task model factory
    /// </summary>
    public partial interface IScheduleTaskModelFactory
    {
        /// <summary>
        /// Prepare schedule task search model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>Schedule task search model</returns>
        ScheduleTaskSearchModel PrepareScheduleTaskSearchModel(ScheduleTaskSearchModel searchModel);

        /// <summary>
        /// Prepare paged schedule task list model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>Schedule task list model</returns>
        ScheduleTaskListModel PrepareScheduleTaskListModel(ScheduleTaskSearchModel searchModel);
    }
}