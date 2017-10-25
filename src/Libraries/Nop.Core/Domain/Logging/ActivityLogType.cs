namespace Nop.Core.Domain.Logging
{
    /// <summary>
    /// 活动日志类别
    /// Represents an activity log type record
    /// </summary>
    public partial class ActivityLogType : BaseEntity
    {
        /// <summary>
        /// 系统关键字
        /// Gets or sets the system keyword
        /// </summary>
        public string SystemKeyword { get; set; }

        /// <summary>
        /// 名称
        /// Gets or sets the display name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// Gets or sets a value indicating whether the activity log type is enabled
        /// </summary>
        public bool Enabled { get; set; }
    }
}
