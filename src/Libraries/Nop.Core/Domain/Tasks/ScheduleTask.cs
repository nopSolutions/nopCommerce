using System;

namespace Nop.Core.Domain.Tasks
{
    /// <summary>
    /// Schedule task
    /// </summary>
    public partial class ScheduleTask : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the run period (in seconds)
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Gets or sets the type of appropriate ITask class
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task should be stopped on some error
        /// </summary>
        public bool StopOnError { get; set; }


        /// <summary>
        /// Gets or sets the machine name (instance) that leased this task. It's used when running in web farm (ensure that a task in run only on one machine). It could be null when not running in web farm.
        /// </summary>
        public string LeasedByMachineName { get; set; }
        /// <summary>
        /// Gets or sets the datetime until the task is leased by some machine (instance). It's used when running in web farm (ensure that a task in run only on one machine).
        /// </summary>
        public DateTime? LeasedUntilUtc { get; set; }

        /// <summary>
        /// Gets or sets the datetime when it was started last time
        /// </summary>
        public DateTime? LastStartUtc { get; set; }
        /// <summary>
        /// Gets or sets the datetime when it was finished last time (no matter failed ir success)
        /// </summary>
        public DateTime? LastEndUtc { get; set; }
        /// <summary>
        /// Gets or sets the datetime when it was sucessfully finished last time
        /// </summary>
        public DateTime? LastSuccessUtc { get; set; }
    }
}
