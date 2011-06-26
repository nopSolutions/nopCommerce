using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a news comment
    /// </summary>
    public partial class NewsComment : CustomerContent
    {
        /// <summary>
        /// Gets or sets the comment title
        /// </summary>
        public virtual string CommentTitle { get; set; }

        /// <summary>
        /// Gets or sets the comment text
        /// </summary>
        public virtual string CommentText { get; set; }

        /// <summary>
        /// Gets or sets the news item identifier
        /// </summary>
        public virtual int NewsItemId { get; set; }

        /// <summary>
        /// Gets or sets the news item
        /// </summary>
        public virtual NewsItem NewsItem { get; set; }
    }
}