
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Blogs
{
    public class BlogSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether blog is enabled
        /// </summary>
        public bool BlogEnabled { get; set; }

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
    }
}