using System.Text.RegularExpressions;
using Nop.Core.Domain.Common;
using Nop.Services.Html.CodeFormatter;

namespace Nop.Services.Html;

/// <summary>
/// Represents a BBCode helper
/// </summary>
public partial class BBCodeHelper : IBBCodeHelper
{
    #region Fields

    protected readonly CommonSettings _commonSettings;

    protected static readonly Regex _regexBold = new(@"\[b\](.+?)\[/b\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    protected static readonly Regex _regexItalic = new(@"\[i\](.+?)\[/i\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    protected static readonly Regex _regexUnderLine = new(@"\[u\](.+?)\[/u\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    protected static readonly Regex _regexUrl1 = new(@"\[url\=(https?:.+?)\]([^\]]+)\[/url\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    protected static readonly Regex _regexUrl2 = new(@"\[url\](https?:.+?)\[/url\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    protected static readonly Regex _regexQuote = new(@"\[quote=(.+?)\](.+?)\[/quote\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    protected static readonly Regex _regexImg = new(@"\[img\](.+?)\[/img\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    #endregion

    #region Ctor

    public BBCodeHelper(CommonSettings commonSettings)
    {
        _commonSettings = commonSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats the text
    /// </summary>
    /// <param name="text">Text</param>
    /// <param name="replaceBold">A value indicating whether to replace Bold</param>
    /// <param name="replaceItalic">A value indicating whether to replace Italic</param>
    /// <param name="replaceUnderline">A value indicating whether to replace Underline</param>
    /// <param name="replaceUrl">A value indicating whether to replace URL</param>
    /// <param name="replaceCode">A value indicating whether to replace Code</param>
    /// <param name="replaceQuote">A value indicating whether to replace Quote</param>
    /// <param name="replaceImg">A value indicating whether to replace Img</param>
    /// <returns>Formatted text</returns>
    public virtual string FormatText(string text, bool replaceBold, bool replaceItalic,
        bool replaceUnderline, bool replaceUrl, bool replaceCode, bool replaceQuote, bool replaceImg)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (replaceBold)
            // format the bold tags: [b][/b] becomes: <strong></strong>
            text = _regexBold.Replace(text, "<strong>$1</strong>");

        if (replaceItalic)
            // format the italic tags: [i][/i] becomes: <em></em>
            text = _regexItalic.Replace(text, "<em>$1</em>");

        if (replaceUnderline)
            // format the underline tags: [u][/u] becomes: <u></u>
            text = _regexUnderLine.Replace(text, "<u>$1</u>");

        if (replaceUrl)
        {
            var newWindow = _commonSettings.BbcodeEditorOpenLinksInNewWindow;
            // format the URL tags: [url=https://www.nopCommerce.com]my site[/url]
            // becomes: <a href="https://www.nopCommerce.com">my site</a>
            text = _regexUrl1.Replace(text, $"<a href=\"$1\" rel=\"nofollow\"{(newWindow ? " target=_blank" : "")}>$2</a>");

            // format the URL tags: [url]https://www.nopCommerce.com[/url]
            // becomes: <a href="https://www.nopCommerce.com">https://www.nopCommerce.com</a>
            text = _regexUrl2.Replace(text, $"<a href=\"$1\" rel=\"nofollow\"{(newWindow ? " target=_blank" : "")}>$1</a>");
        }

        if (replaceQuote)
            while (_regexQuote.IsMatch(text))
                text = _regexQuote.Replace(text, "<b>$1 wrote:</b><div class=\"quote\">$2</div>");

        if (replaceCode)
            text = CodeFormatHelper.FormatTextSimple(text);

        if (replaceImg)
            // format the img tags: [img]https://www.nopCommerce.com/Content/Images/Image.jpg[/img]
            // becomes: <img src="https://www.nopCommerce.com/Content/Images/Image.jpg">
            text = _regexImg.Replace(text, "<img src=\"$1\" class=\"user-posted-image\" alt=\"\">");

        return text;
    }

    /// <summary>
    /// Removes all quotes from string
    /// </summary>
    /// <param name="str">Source string</param>
    /// <returns>string</returns>
    public virtual string RemoveQuotes(string str)
    {
        str = Regex.Replace(str, @"\[quote=(.+?)\]", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        str = Regex.Replace(str, @"\[/quote\]", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return str;
    }

    #endregion
}