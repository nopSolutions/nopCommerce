namespace Nop.Services.Html.CodeFormatter;

/// <summary>
/// Handles all of the options for changing the rendered code.
/// </summary>
public partial class HighlightOptions
{
    /// <summary>
    /// Code
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Display line numbers
    /// </summary>
    public bool DisplayLineNumbers { get; set; }

    /// <summary>
    /// Language
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Alternate line numbers
    /// </summary>
    public bool AlternateLineNumbers { get; set; }
}