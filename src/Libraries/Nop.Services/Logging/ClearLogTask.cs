<<<<<<< HEAD
﻿using System;
using Nop.Core.Domain.Common;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Represents a task to clear [Log] table
    /// </summary>
    public partial class ClearLogTask : IScheduleTask
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ClearLogTask(CommonSettings commonSettings,
            ILogger logger)
        {
            _commonSettings = commonSettings;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async System.Threading.Tasks.Task ExecuteAsync()
        {
            var utcNow = DateTime.UtcNow;
            
            await _logger.ClearLogAsync(_commonSettings.ClearLogOlderThanDays == 0 ? null : utcNow.AddDays(-_commonSettings.ClearLogOlderThanDays));
        }

        #endregion
    }
=======
﻿using System;
using Nop.Core.Domain.Common;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Represents a task to clear [Log] table
    /// </summary>
    public partial class ClearLogTask : IScheduleTask
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ClearLogTask(CommonSettings commonSettings,
            ILogger logger)
        {
            _commonSettings = commonSettings;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async System.Threading.Tasks.Task ExecuteAsync()
        {
            var utcNow = DateTime.UtcNow;
            
            await _logger.ClearLogAsync(_commonSettings.ClearLogOlderThanDays == 0 ? null : utcNow.AddDays(-_commonSettings.ClearLogOlderThanDays));
        }

        #endregion
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}