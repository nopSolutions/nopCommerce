namespace Nop.Services.Html
{
    /// <summary>
    /// Represents a HTML formatter
    /// </summary>
    public interface IHtmlFormatter
    {
        /// <summary>
        /// Formats the text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="stripTags">A value indicating whether to strip tags</param>
        /// <param name="convertPlainTextToHtml">A value indicating whether HTML is allowed</param>
        /// <param name="allowHtml">A value indicating whether HTML is allowed</param>
        /// <param name="allowBbCode">A value indicating whether BBCode is allowed</param>
        /// <param name="resolveLinks">A value indicating whether to resolve links</param>
        /// <param name="addNoFollowTag">A value indicating whether to add "noFollow" tag</param>
        /// <returns>Formatted text</returns>
        string FormatText(string text,
            bool stripTags,
            bool convertPlainTextToHtml,
            bool allowHtml,
            bool allowBbCode,
            bool resolveLinks,
            bool addNoFollowTag);

        /// <summary>
        /// Strips tags
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        string StripTags(string text);

        /// <summary>
        /// replace anchor text (remove a tag from the following URL <a href="http://example.com">Name</a> and output only the string "Name")
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Text</returns>
        string ReplaceAnchorTags(string text);

        /// <summary>
        /// Converts plain text to HTML
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        string ConvertPlainTextToHtml(string text);

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
}