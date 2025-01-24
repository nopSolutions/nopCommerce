namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample news item
/// </summary>
public partial class SampleNewsItem
{
    /// <summary>
    /// Gets or sets the news title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the short text
    /// </summary>
    public string Short { get; set; }

    /// <summary>
    /// Gets or sets the full text
    /// </summary>
    public string Full { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the news item is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the news post comments are allowed 
    /// </summary>
    public bool AllowComments { get; set; }

    /// <summary>
    /// Gets or sets the news comments
    /// </summary>
    public List<SampleNewsComment> NewsComments { get; set; } = new();

    #region Nested class

    /// <summary>
    /// Represents a sample news comment
    /// </summary>
    public partial class SampleNewsComment
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
        /// Gets or sets a value indicating whether the comment is approved
        /// </summary>
        public bool IsApproved { get; set; }
    }

    #endregion
}
