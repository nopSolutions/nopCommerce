namespace Nop.Core.Domain.Blogs;

/// <summary>
/// Represents a blog post tag
/// </summary>
public partial class BlogPostTag
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the tagged product count
    /// </summary>
    public int BlogPostCount { get; set; }
}