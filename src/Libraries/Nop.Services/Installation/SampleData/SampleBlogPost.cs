namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample blog post
/// </summary>
public partial class SampleBlogPost
{
    /// <summary>
    /// Gets or sets the blog post title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the blog post body
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the blog post overview. If specified, then it's used on the blog page instead of the "Body"
    /// </summary>
    public string BodyOverview { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the blog post comments are allowed 
    /// </summary>
    public bool AllowComments { get; set; } = true;

    /// <summary>
    /// Gets or sets the blog tags
    /// </summary>
    public string Tags { get; set; }

    /// <summary>
    /// Gets or sets the list of comments
    /// </summary>
    public List<string> Comments { get; set; } = new();
}
