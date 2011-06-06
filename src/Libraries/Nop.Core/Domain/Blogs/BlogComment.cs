using System;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Blogs
{
    /// <summary>
    /// Represents a blog comment
    /// </summary>
    public partial class BlogComment : CustomerContent
    {
        /// <summary>
        /// Gets or sets the comment text
        /// </summary>
        public virtual string CommentText { get; set; }

        /// <summary>
        /// Gets or sets the blog post identifier
        /// </summary>
        public virtual int BlogPostId { get; set; }

        /// <summary>
        /// Gets or sets the blog post
        /// </summary>
        public virtual BlogPost BlogPost { get; set; }
    }
}