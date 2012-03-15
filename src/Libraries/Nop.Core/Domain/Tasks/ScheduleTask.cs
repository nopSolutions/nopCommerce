
using System;

namespace Nop.Core.Domain.Tasks
{
    public class ScheduleTask : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the run period (in seconds)
        /// </summary>
        public virtual int Seconds { get; set; }

        /// <summary>
        /// Gets or sets the type of appropriate ITask class
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task is enabled
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task should be stopped on some error
        /// </summary>
        public virtual bool StopOnError { get; set; }

        public virtual DateTime? LastStartUtc { get; set; }

        public virtual DateTime? LastEndUtc { get; set; }

        public virtual DateTime? LastSuccessUtc { get; set; }
    }
}
