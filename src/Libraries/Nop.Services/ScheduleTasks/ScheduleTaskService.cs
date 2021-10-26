using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;

namespace Nop.Services.ScheduleTasks
{
    /// <summary>
    /// Task service
    /// </summary>
    public partial class ScheduleTaskService : IScheduleTaskService
    {
        #region Fields

        private readonly IRepository<ScheduleTask> _taskRepository;

        #endregion

        #region Ctor

        public ScheduleTaskService(IRepository<ScheduleTask> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual async Task DeleteTaskAsync(ScheduleTask task)
        {
            await _taskRepository.DeleteAsync(task, false);
        }

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the schedule task
        /// </returns>
        public virtual async Task<ScheduleTask> GetTaskByIdAsync(int taskId)
        {
            return await _taskRepository.GetByIdAsync(taskId, _ => default);
        }

        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the schedule task
        /// </returns>
        public virtual async Task<ScheduleTask> GetTaskByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;

            var query = _taskRepository.Table;
            query = query.Where(st => st.Type == type);
            query = query.OrderByDescending(t => t.Id);

            var task = await query.FirstOrDefaultAsync();

            return task;
        }

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of schedule task
        /// </returns>
        public virtual async Task<IList<ScheduleTask>> GetAllTasksAsync(bool showHidden = false)
        {
            var tasks = await _taskRepository.GetAllAsync(query =>
            {
                if (!showHidden)
                    query = query.Where(t => t.Enabled);

                query = query.OrderByDescending(t => t.Seconds);

                return query;
            });

            return tasks;
        }

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual async Task InsertTaskAsync(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (task.Enabled && !task.LastEnabledUtc.HasValue)
                task.LastEnabledUtc = DateTime.UtcNow;

            await _taskRepository.InsertAsync(task, false);
        }

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual async Task UpdateTaskAsync(ScheduleTask task)
        {
            await _taskRepository.UpdateAsync(task, false);
        }

        #endregion
    }
}