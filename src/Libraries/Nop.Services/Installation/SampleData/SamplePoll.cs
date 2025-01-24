namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample poll
/// </summary>
public partial class SamplePoll
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the system keyword
    /// </summary>
    public string SystemKeyword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the entity is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity should be shown on home page
    /// </summary>
    public bool ShowOnHomepage { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the answers
    /// </summary>
    public List<SamplePollAnswer> Answers { get; set; } = new();

    #region Nested class

    /// <summary>
    /// Represents a poll answer
    /// </summary>
    public partial class SamplePollAnswer
    {
        /// <summary>
        /// Gets or sets the poll answer name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }

    #endregion
}