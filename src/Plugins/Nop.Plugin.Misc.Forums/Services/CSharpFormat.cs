using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Misc.Forums.Services;

/// <summary>
/// Generates color-coded HTML 4.01 from C# source code
/// </summary>
public partial class CSharpFormat
{
    #region Fields

    private const byte TAB_SPACES = 4;
    private const string KEYWORDS = "abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override partial params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using value virtual void volatile where while yield";
    private const string PREPROCESSORS = "#if #else #elif #endif #define #undef #warning #error #line #region #endregion #pragma";

    private static readonly Regex _codeRegex;

    #endregion

    #region Ctor

    /// <summary/>
    static CSharpFormat()
    {
        //generate the keyword and preprocessor regexes from the keyword lists
        var r = RegexKeywordAndPreprocessor();
        var regKeyword = r.Replace(KEYWORDS, @"(?<=^|\W)$0(?=\W)");
        var regPreproc = r.Replace(PREPROCESSORS, @"(?<=^|\s)$0(?=\s|$)");
        r = new Regex(" +");
        regKeyword = r.Replace(regKeyword, "|");
        regPreproc = r.Replace(regPreproc, "|");

        if (regPreproc.Length == 0)
            regPreproc = "(?!.*)_{37}(?<!.*)"; //use something quite impossible...

        //build a master regex with capturing groups
        var regAll = new StringBuilder();
        regAll.Append('(');
        //regular expression string to match comments
        regAll.Append(@"/\*.*?\*/|//.*?(?=\r|\n)");
        regAll.Append(")|(");
        //regular expression string match strings literals
        regAll.Append("""@?""|@?".*?(?!\\)."|''|'.*?(?!\\).'""");
        regAll.Append(")|(");
        regAll.Append(regPreproc);
        regAll.Append(")|(");
        regAll.Append(regKeyword);
        regAll.Append(')');

        _codeRegex = new Regex(regAll.ToString(), RegexOptions.Singleline);
    }

    #endregion

    #region Utilities
    
    /// <summary>
    /// Called to evaluate the HTML fragment corresponding to each 
    /// matching token in the code.
    /// </summary>
    /// <param name="match">The <see cref="Match"/> resulting from a 
    /// single regular expression match.</param>
    /// <returns>A string containing the HTML code fragment.</returns>
    private static string MatchEval(Match match)
    {
        if (match.Groups[1].Success) //comment
        {
            var reader = new StringReader(match.ToString());
            var sb = new StringBuilder();

            while (reader.ReadLine() is { } line)
            {
                if (sb.Length > 0)
                    sb.Append('\n');

                sb.Append("<span class=\"rem\">");
                sb.Append(line);
                sb.Append("</span>");
            }

            return sb.ToString();
        }

        if (match.Groups[2].Success) //string literal
            return "<span class=\"str\">" + match + "</span>";

        if (match.Groups[3].Success) //preprocessor keyword
            return "<span class=\"preproc\">" + match + "</span>";

        if (match.Groups[4].Success) //keyword
            return "<span class=\"kwrd\">" + match + "</span>";

        return string.Empty; //none of the above
    }

    [GeneratedRegex(@"\w+|-\w+|#\w+|@@\w+|#(?:\\(?:s|w)(?:\*|\+)?\w+)+|@\\w\*+")]
    private static partial Regex RegexKeywordAndPreprocessor();

    [GeneratedRegex(@"\[code\](?<inner>(.*?))\[/code\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex RegexCode();

    /// <summary>
    /// Transforms a source code string to HTML 4.01.
    /// </summary>
    /// <returns>A string containing the HTML formatted code.</returns>
    private static string FormatCode(string source)
    {
        //replace special characters
        var sb = new StringBuilder(source);

        sb.Replace("&", "&amp;");
        sb.Replace("<", "&lt;");
        sb.Replace(">", "&gt;");
        sb.Replace("\t", string.Empty.PadRight(TAB_SPACES));

        //color the code
        source = _codeRegex.Replace(sb.ToString(), MatchEval);

        sb = new StringBuilder();

        //have to use a <pre> because IE below ver 6 does not understand 
        //the "white-space: pre" CSS value
        sb.Append("<pre class=\"csharpcode\">\n");

        sb.Append(source);
        sb.Append("</pre>");

        return sb.ToString();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats the text
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    public static string FormatTextSimple(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (!text.Contains("[/code]"))
            return text;

        var regexCode = RegexCode();

        text = regexCode.Replace(text, codeEvaluatorSimple);
        text = regexCode.Replace(text, "$1");

        return text;

        static string codeEvaluatorSimple(Match match)
        {
            if (!match.Success)
                return match.Value;

            return WebUtility.HtmlDecode(FormatCode(match.Value));
        }
    }

    #endregion
}