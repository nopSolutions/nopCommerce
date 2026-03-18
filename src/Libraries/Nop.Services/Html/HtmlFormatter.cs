using System.Net;
using System.Text.RegularExpressions;

namespace Nop.Services.Html;

/// <summary>
/// Represents the HTML formatter implementation
/// </summary>
public partial class HtmlFormatter : IHtmlFormatter
{
    #region Utilities
    
    /// <summary>
    /// Replace anchor text (remove a tag from the following URL <a href="http://example.com">Name</a> and output only the string "Name")
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Text</returns>
    protected virtual string ReplaceAnchorTags(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = Regex.Replace(text, @"<a\b[^>]+>([^<]*(?:(?!</a)<[^<]*)*)</a>", "$1", RegexOptions.IgnoreCase);
        return text;
    }

    /// <summary>
    /// Converts plain text to HTML
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    protected virtual string ConvertPlainTextToHtml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = text.Replace("\r\n", "<br />");
        text = text.Replace("\r", "<br />");
        text = text.Replace("\n", "<br />");
        text = text.Replace("\t", "&nbsp;&nbsp;");
        text = text.Replace("  ", "&nbsp;&nbsp;");

        return text;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats the text
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    public virtual string FormatText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        try
        {
            text = ConvertPlainTextToHtml(WebUtility.HtmlEncode(text));
        }
        catch (Exception exc)
        {
            text = $"Text cannot be formatted. Error: {exc.Message}";
        }

        return text;
    }

    /// <summary>
    /// Strips tags
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    public virtual string StripTags(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = Regex.Replace(text, @"(>)(\r|\n)*(<)", "><");
        text = Regex.Replace(text, "(<[^>]*>)([^<]*)", "$2");
        text = Regex.Replace(text, "(&#x?[0-9]{2,4};|&quot;|&amp;|&nbsp;|&lt;|&gt;|&euro;|&copy;|&reg;|&permil;|&Dagger;|&dagger;|&lsaquo;|&rsaquo;|&bdquo;|&rdquo;|&ldquo;|&sbquo;|&rsquo;|&lsquo;|&mdash;|&ndash;|&rlm;|&lrm;|&zwj;|&zwnj;|&thinsp;|&emsp;|&ensp;|&tilde;|&circ;|&Yuml;|&scaron;|&Scaron;)", "@");

        return text;
    }

    /// <summary>
    /// Converts HTML to plain text
    /// </summary>
    /// <param name="text">Text</param>
    /// <param name="decode">A value indicating whether to decode text</param>
    /// <param name="replaceAnchorTags">A value indicating whether to replace anchor text (remove a tag from the following URL <a href="http://example.com">Name</a> and output only the string "Name")</param>
    /// <returns>Formatted text</returns>
    public virtual string ConvertHtmlToPlainText(string text,
        bool decode = false, bool replaceAnchorTags = false)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (decode)
            text = WebUtility.HtmlDecode(text);

        text = text.Replace("<br>", "\n");
        text = text.Replace("<br >", "\n");
        text = text.Replace("<br />", "\n");
        text = text.Replace("&nbsp;&nbsp;", "\t");
        text = text.Replace("&nbsp;&nbsp;", "  ");

        if (replaceAnchorTags)
            text = ReplaceAnchorTags(text);

        return text;
    }

    #endregion
}