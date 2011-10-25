using System.Collections.Generic;
using Nop.Core.Domain.Tasks;

namespace Nop.Services.Tasks
{
    /// <summary>
    /// Task service interface
    /// </summary>
    public partial interface IScheduleTaskService
    {
        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="task">Task</param>
        void DeleteTask(ScheduleTask task);

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>Task</returns>
        ScheduleTask GetTaskById(int taskId);

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <returns>Tasks</returns>
        IList<ScheduleTask> GetAllTasks();

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        void InsertTask(ScheduleTask task);

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        void UpdateTask(ScheduleTask task);
    }
}
