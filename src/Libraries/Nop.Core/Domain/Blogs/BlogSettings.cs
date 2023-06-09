using Nop.Core.Configuration;

namespace Nop.Core.Domain.Blogs
{
    /// <summary>
    /// Blog settings
    /// </summary>
    public partial class BlogSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether blog is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the page size for posts
        /// </summary>
        public int PostsPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether not registered user can leave comments
        /// </summary>
        public bool AllowNotRegisteredUsersToLeaveComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to notify about new blog comments
        /// </summary>
        public bool NotifyAboutNewBlogComments { get; set; }

        /// <summary>
        /// Gets or sets a number of blog tags that appear in the tag cloud
        /// </summary>
        public int NumberOfTags { get; set; }

        /// <summary>
        /// Enable the blog RSS feed link in customers browser address bar
        /// </summary>
        public bool ShowHeaderRssUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether blog comments must be approved
        /// </summary>
        public bool BlogCommentsMustBeApproved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether blog comments will be filtered per store
        /// </summary>
        public bool ShowBlogCommentsPerStore { get; set; }
    }
}