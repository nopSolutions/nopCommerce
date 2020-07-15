using System;

namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a news comment
    /// </summary>
    public partial class NewsComment : BaseEntity
    {
        /// <summary>
        /// Gets or sets the comment title
        /// </summary>
        public string CommentTitle { get; set; }

        /// <summary>
        /// Gets or sets the comment text
        /// </summary>
        public string CommentText { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? RepliedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the news item identifier
        /// </summary>
        public int NewsItemId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the comment is approved
        /// </summary>
        public bool IsApproved { get; set; }
        /// <summary>
        /// 是否展现
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}