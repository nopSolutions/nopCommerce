using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the schedule task search model
        /// </returns>
        Task<ScheduleTaskSearchModel> PrepareScheduleTaskSearchModelAsync(ScheduleTaskSearchModel searchModel);

        /// <summary>
        /// Prepare paged schedule task list model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the schedule task list model
        /// </returns>
        Task<ScheduleTaskListModel> PrepareScheduleTaskListModelAsync(ScheduleTaskSearchModel searchModel);
    }
}