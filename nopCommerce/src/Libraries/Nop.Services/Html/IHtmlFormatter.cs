namespace Nop.Services.Html;

/// <summary>
/// Represents a HTML formatter
/// </summary>
public partial interface IHtmlFormatter
{
    /// <summary>
    /// Formats the text
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    string FormatText(string text);

    /// <summary>
    /// Strips tags
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    string StripTags(string text);

    /// <summary>
    /// Converts HTML to plain text
    /// </summary>
    /// <param name="text">Text</param>
    /// <param name="decode">A value indicating whether to decode text</param>
    /// <param name="replaceAnchorTags">A value indicating whether to replace anchor text (remove a tag from the following URL <a href="http://example.com">Name</a> and output only the string "Name")</param>
    /// <returns>Formatted text</returns>
    string ConvertHtmlToPlainText(string text,
        bool decode = false,
        bool replaceAnchorTags = false);
}