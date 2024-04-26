namespace Nop.Services.Html;

public partial interface IBBCodeHelper
{
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
    string FormatText(string text,
        bool replaceBold,
        bool replaceItalic,
        bool replaceUnderline,
        bool replaceUrl,
        bool replaceCode,
        bool replaceQuote,
        bool replaceImg);

    /// <summary>
    /// Removes all quotes from string
    /// </summary>
    /// <param name="str">Source string</param>
    /// <returns>string</returns>
    string RemoveQuotes(string str);
}