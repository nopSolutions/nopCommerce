using System;

namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// 活动信息
    /// Represents a campaign
    /// </summary>
    public partial class Campaign : BaseEntity
    {
        /// <summary>
        /// 名称
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 主题
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the store identifier  which subscribers it will be sent to; set 0 for all newsletter subscribers
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier  which subscribers it will be sent to; set 0 for all newsletter subscribers
        /// </summary>
        public int CustomerRoleId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time in UTC before which this email should not be sent
        /// </summary>
        public DateTime? DontSendBeforeDateUtc { get; set; }
    }
}
