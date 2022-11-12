<<<<<<< HEAD
﻿using Nop.Services.ScheduleTasks;

namespace Nop.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : IScheduleTask
    {
        #region Fields

        private readonly StoreHttpClient _storeHttpClient;

        #endregion

        #region Ctor

        public KeepAliveTask(StoreHttpClient storeHttpClient)
        {
            _storeHttpClient = storeHttpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _storeHttpClient.KeepAliveAsync();
        }

        #endregion
    }
=======
﻿using Nop.Services.ScheduleTasks;

namespace Nop.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : IScheduleTask
    {
        #region Fields

        private readonly StoreHttpClient _storeHttpClient;

        #endregion

        #region Ctor

        public KeepAliveTask(StoreHttpClient storeHttpClient)
        {
            _storeHttpClient = storeHttpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _storeHttpClient.KeepAliveAsync();
        }

        #endregion
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}