namespace Nop.Core.Domain.Blogs;

/// <summary>
/// Blog post comment approved event
/// </summary>
public partial class BlogCommentApprovedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="blogComment">Blog comment</param>
    public BlogCommentApprovedEvent(BlogComment blogComment)
    {
        BlogComment = blogComment;
    }

    /// <summary>
    /// Blog post comment
    /// </summary>
    public BlogComment BlogComment { get; }
}