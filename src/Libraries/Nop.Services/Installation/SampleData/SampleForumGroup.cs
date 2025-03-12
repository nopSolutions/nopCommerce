namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample forum group
/// </summary>
public partial class SampleForumGroup
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the forums
    /// </summary>
    public List<SampleForum> Forums { get; set; } = new();

    #region Nested class

    /// <summary>
    /// Represents a sample forum
    /// </summary>
    public partial class SampleForum
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }

    #endregion
}