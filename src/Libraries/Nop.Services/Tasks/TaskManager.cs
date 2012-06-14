using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Nop.Core.Infrastructure;

namespace Nop.Services.Tasks
{
    /// <summary>
    /// Represents task manager
    /// </summary>
    public partial class TaskManager
    {
        private static readonly TaskManager _taskManager = new TaskManager();
        private readonly List<TaskThread> _taskThreads = new List<TaskThread>();

        private TaskManager()
        {
        }
        
        /// <summary>
        /// Initializes the task manager with the property values specified in the configuration file.
        /// </summary>
        public void Initialize()
        {
            this._taskThreads.Clear();

            var taskService = EngineContext.Current.Resolve<IScheduleTaskService>();
            var scheduleTasks = taskService
                .GetAllTasks()
                .OrderBy(x => x.Seconds)
                .ToList();

            //group by threads with the same seconds
            foreach (var scheduleTaskGrouped in scheduleTasks.GroupBy(x => x.Seconds))
            {
                //create a thread
                var taskThread = new TaskThread();
                taskThread.Seconds = scheduleTaskGrouped.Key;
                this._taskThreads.Add(taskThread);
                foreach (var scheduleTask in scheduleTaskGrouped)
                {
                    var task = new Task(scheduleTask);
                    taskThread.AddTask(task);
                }
            }


            //one thread, one task
            //foreach (var scheduleTask in scheduleTasks)
            //{
            //    var taskThread = new TaskThread(scheduleTask);
            //    this._taskThreads.Add(taskThread);
            //    var task = new Task(scheduleTask);
            //    taskThread.AddTask(task);
            //}
        }

        /// <summary>
        /// Starts the task manager
        /// </summary>
        public void Start()
        {
            foreach (var taskThread in this._taskThreads)
            {
                taskThread.InitTimer();
            }
        }

        /// <summary>
        /// Stops the task manager
        /// </summary>
        public void Stop()
        {
            foreach (var taskThread in this._taskThreads)
            {
                taskThread.Dispose();
            }
        }

        /// <summary>
        /// Gets the task mamanger instance
        /// </summary>
        public static TaskManager Instance
        {
            get
            {
                return _taskManager;
            }
        }

        /// <summary>
        /// Gets a list of task threads of this task manager
        /// </summary>
        public IList<TaskThread> TaskThreads
        {
            get
            {
                return new ReadOnlyCollection<TaskThread>(this._taskThreads);
            }
        }
    }
}
