using System.Collections.Generic;

namespace Nop.Core.Domain.Logging
{
    /// <summary>
    /// Represents an activity log type record
    /// </summary>
    public partial class ActivityLogType : BaseEntity
    {
        private ICollection<ActivityLog> _activityLog;
        
        #region Properties

        /// <summary>
        /// Gets or sets the system keyword
        /// </summary>
        public virtual string SystemKeyword { get; set; }

        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the activity log type is enabled
        /// </summary>
        public virtual bool Enabled { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the activity log
        /// </summary>
        public virtual ICollection<ActivityLog> ActivityLog
        {
            get { return _activityLog ?? (_activityLog = new List<ActivityLog>()); }
            protected set { _activityLog = value; }
        }

        #endregion
    }
}
