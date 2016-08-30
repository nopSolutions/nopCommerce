using System;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Logging
{
    /// <summary>
    /// Represents an activity log record
    /// </summary>
    public partial class ActivityLog : BaseEntity
    {
        /// <summary>
        /// Gets or sets the activity log type identifier
        /// </summary>
        public int ActivityLogTypeId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the activity comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the activity log type
        /// </summary>
        public virtual ActivityLogType ActivityLogType { get; set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets the ip address
        /// </summary>
        public virtual string IpAddress { get; set; }
    }
}
