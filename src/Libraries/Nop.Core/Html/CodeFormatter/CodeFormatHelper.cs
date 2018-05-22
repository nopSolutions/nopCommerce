using System.Net;
using System.Text.RegularExpressions;

namespace Nop.Core.Html.CodeFormatter
{
    /// <summary>
    /// Represents a code format helper
    /// </summary>
    public partial class CodeFormatHelper
    {
        #region Fields

        //private static Regex regexCode1 = new Regex(@"(?<begin>\[code:(?<lang>.*?)(?:;ln=(?<linenumbers>(?:on|off)))?(?:;alt=(?<altlinenumbers>(?:on|off)))?(?:;(?<title>.*?))?\])(?<code>.*?)(?<end>\[/code\])", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex _regexHtml = new Regex("<[^>]*>", RegexOptions.Compiled);

        private static readonly Regex _regexCode =
            new Regex(@"\[code\](?<inner>(.*?))\[/code\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Utilities

        /// <summary>
        /// Code evaluator method
        /// </summary>
        /// <param name="match">Match</param>
        /// <returns>Formatted text</returns>
        private static string CodeEvaluatorSimple(Match match)
        {
            if (!match.Success)
                return match.Value;

            var options = new HighlightOptions
            {
                Language = "c#",
                Code = match.Groups["inner"].Value,
                DisplayLineNumbers = false,
                Title = string.Empty,
                AlternateLineNumbers = false
            };

            var result = match.Value;
            result = Highlight(options, result);
            return result;
        }

        /// <summary>
        /// Strips HTML
        /// </summary>
        /// <param name="html">HTML</param>
        /// <returns>Formatted text</returns>
        private static string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            return _regexHtml.Replace(html, string.Empty);
        }

        /// <summary>
        /// Returns the formatted text.
        /// </summary>
        /// <param name="options">Whatever options were set in the regex groups.</param>
        /// <param name="text">Send the e.body so it can get formatted.</param>
        /// <returns>The formatted string of the match.</returns>
        private static string Highlight(HighlightOptions options, string text)
        {
            switch (options.Language)
            {
                case "c#":
                    var csf = new CSharpFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                    return WebUtility.HtmlDecode(csf.FormatCode(text));

                case "vb":
                    var vbf = new VisualBasicFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                    return vbf.FormatCode(text);

                case "js":
                    var jsf = new JavaScriptFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                    return WebUtility.HtmlDecode(jsf.FormatCode(text));

                case "html":
                    var htmlf = new HtmlFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                    text = StripHtml(text).Trim();
                    var code = htmlf.FormatCode(WebUtility.HtmlDecode(text)).Trim();
                    return code.Replace("\r\n", "<br />").Replace("\n", "<br />");

                case "xml":
                    var xmlf = new HtmlFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                    text = text.Replace("<br />", "\r\n");
                    text = StripHtml(text).Trim();
                    var xml = xmlf.FormatCode(WebUtility.HtmlDecode(text)).Trim();
                    return xml.Replace("\r\n", "<br />").Replace("\n", "<br />");

                case "tsql":
                    var tsqlf = new TsqlFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                    return WebUtility.HtmlDecode(tsqlf.FormatCode(text));

                case "msh":
                    var mshf = new MshFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                    return WebUtility.HtmlDecode(mshf.FormatCode(text));
            }

            return string.Empty;
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

            text = _regexCode.Replace(text, CodeEvaluatorSimple);
            text = _regexCode.Replace(text, "$1");
            return text;
        }

        #endregion
    }
}
