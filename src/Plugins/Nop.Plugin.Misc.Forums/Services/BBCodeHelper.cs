using System.Text.RegularExpressions;

namespace Nop.Plugin.Misc.Forums.Services;

/// <summary>
/// Represents a BBCode helper
/// </summary>
public partial class BBCodeHelper
{
    #region Fields

    protected readonly ForumSettings _forumSettings;

    #endregion

    #region Ctor

    public BBCodeHelper(ForumSettings forumSettings)
    {
        _forumSettings = forumSettings;
    }

    #endregion

    #region Utilities
    
    [GeneratedRegex(@"\[b\](.+?)\[/b\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex RegexBold();

    [GeneratedRegex(@"\[i\](.+?)\[/i\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex RegexItalic();

    [GeneratedRegex(@"\[u\](.+?)\[/u\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex RegexUnderLine();

    [GeneratedRegex(@"\[url\=(https?:.+?)\]([^\]]+)\[/url\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex RegexUrl1();

    [GeneratedRegex(@"\[url\](https?:.+?)\[/url\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex RegexUrl2();

    [GeneratedRegex(@"\[quote=(.+?)\](.+?)\[/quote\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex RegexQuote();

    [GeneratedRegex(@"\[img\](.+?)\[/img\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex RegexImg();

    [GeneratedRegex(@"\[quote=(.+?)\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex RegexRemoveQuotes1();

    [GeneratedRegex(@"\[/quote\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex RegexRemoveQuotes2();

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

        // format the bold tags: [b][/b] becomes: <strong></strong>
        text = RegexBold().Replace(text, "<strong>$1</strong>");

        // format the italic tags: [i][/i] becomes: <em></em>
        text = RegexItalic().Replace(text, "<em>$1</em>");

        // format the underline tags: [u][/u] becomes: <u></u>
        text = RegexUnderLine().Replace(text, "<u>$1</u>");

        var newWindow = _forumSettings.BbcodeEditorOpenLinksInNewWindow;
        // format the URL tags: [url=https://www.nopCommerce.com]my site[/url]
        // becomes: <a href="https://www.nopCommerce.com">my site</a>
        text = RegexUrl1().Replace(text, $"<a href=\"$1\" rel=\"nofollow\"{(newWindow ? " target=_blank" : "")}>$2</a>");

        // format the URL tags: [url]https://www.nopCommerce.com[/url]
        // becomes: <a href="https://www.nopCommerce.com">https://www.nopCommerce.com</a>
        text = RegexUrl2().Replace(text, $"<a href=\"$1\" rel=\"nofollow\"{(newWindow ? " target=_blank" : "")}>$1</a>");

        var regexQuote = RegexQuote();
        while (regexQuote.IsMatch(text))
            text = regexQuote.Replace(text, "<b>$1 wrote:</b><div class=\"quote\">$2</div>");

        text = CSharpFormat.FormatTextSimple(text);
        
        // format the img tags: [img]https://www.nopCommerce.com/Content/Images/Image.jpg[/img]
        // becomes: <img src="https://www.nopCommerce.com/Content/Images/Image.jpg">
        text = RegexImg().Replace(text, "<img src=\"$1\" class=\"user-posted-image\" alt=\"\">");

        return text;
    }

    /// <summary>
    /// Removes all quotes from string
    /// </summary>
    /// <param name="str">Source string</param>
    /// <returns>string</returns>
    public virtual string RemoveQuotes(string str)
    {
        str = RegexRemoveQuotes1().Replace(str, string.Empty);
        str = RegexRemoveQuotes2().Replace(str, string.Empty);
        return str;
    }

    #endregion
}