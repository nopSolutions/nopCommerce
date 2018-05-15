namespace Nop.Core.Domain.Blogs
{
    /// <summary>
    /// Blog post comment approved event
    /// </summary>
    public class BlogCommentApprovedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        public BlogCommentApprovedEvent(BlogComment blogComment)
        {
            this.BlogComment = blogComment;
        }

        /// <summary>
        /// Blog post comment
        /// </summary>
        public BlogComment BlogComment { get; private set; }
    }
}