using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Nop.Core.Html.CodeFormatter
{
	/// <summary>
	/// Represents a code format helper
	/// </summary>
    public partial class CodeFormatHelper
    {
        #region Fields
        //private static Regex regexCode1 = new Regex(@"(?<begin>\[code:(?<lang>.*?)(?:;ln=(?<linenumbers>(?:on|off)))?(?:;alt=(?<altlinenumbers>(?:on|off)))?(?:;(?<title>.*?))?\])(?<code>.*?)(?<end>\[/code\])", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly static Regex regexHtml = new Regex("<[^>]*>", RegexOptions.Compiled);
        private readonly static Regex regexCode2 = new Regex(@"\[code\](?<inner>(.*?))\[/code\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion

        #region Utilities

        /// <summary>
        /// Code evaluator method
        /// </summary>
        /// <param name="match">Match</param>
        /// <returns>Formatted text</returns>
        private static string CodeEvaluator(Match match)
        {
            if (!match.Success)
                return match.Value;

            var options = new HighlightOptions();

            options.Language = match.Groups["lang"].Value;
            options.Code = match.Groups["code"].Value;
            options.DisplayLineNumbers = match.Groups["linenumbers"].Value == "on";
            options.Title = match.Groups["title"].Value;
            options.AlternateLineNumbers = match.Groups["altlinenumbers"].Value == "on";

            string result = match.Value.Replace(match.Groups["begin"].Value, "");
            result = result.Replace(match.Groups["end"].Value, "");
            result = Highlight(options, result);
            return result;

        }

        /// <summary>
        /// Code evaluator method
        /// </summary>
        /// <param name="match">Match</param>
        /// <returns>Formatted text</returns>
        private static string CodeEvaluatorSimple(Match match)
        {
            if (!match.Success)
                return match.Value;

            var options = new HighlightOptions();

            options.Language = "c#";
            options.Code = match.Groups["inner"].Value;
            options.DisplayLineNumbers = false;
            options.Title =string.Empty;
            options.AlternateLineNumbers =false;

            string result = match.Value;
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

            return regexHtml.Replace(html, string.Empty);
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
                    var csf = new CSharpFormat();
                    csf.LineNumbers = options.DisplayLineNumbers;
                    csf.Alternate = options.AlternateLineNumbers;
                    return HttpUtility.HtmlDecode(csf.FormatCode(text));

                case "vb":
                    var vbf = new VisualBasicFormat();
                    vbf.LineNumbers = options.DisplayLineNumbers;
                    vbf.Alternate = options.AlternateLineNumbers;
                    return vbf.FormatCode(text);

                case "js":
                    var jsf = new JavaScriptFormat();
                    jsf.LineNumbers = options.DisplayLineNumbers;
                    jsf.Alternate = options.AlternateLineNumbers;
                    return HttpUtility.HtmlDecode(jsf.FormatCode(text));

                case "html":
                    var htmlf = new HtmlFormat();
                    htmlf.LineNumbers = options.DisplayLineNumbers;
                    htmlf.Alternate = options.AlternateLineNumbers;
                    text = StripHtml(text).Trim();
                    string code = htmlf.FormatCode(HttpUtility.HtmlDecode(text)).Trim();
                    return code.Replace("\r\n", "<br />").Replace("\n", "<br />");

                case "xml":
                    var xmlf = new HtmlFormat();
                    xmlf.LineNumbers = options.DisplayLineNumbers;
                    xmlf.Alternate = options.AlternateLineNumbers;
                    text = text.Replace("<br />", "\r\n");
                    text = StripHtml(text).Trim();
                    string xml = xmlf.FormatCode(HttpUtility.HtmlDecode(text)).Trim();
                    return xml.Replace("\r\n", "<br />").Replace("\n", "<br />");

                case "tsql":
                    var tsqlf = new TsqlFormat();
                    tsqlf.LineNumbers = options.DisplayLineNumbers;
                    tsqlf.Alternate = options.AlternateLineNumbers;
                    return HttpUtility.HtmlDecode(tsqlf.FormatCode(text));

                case "msh":
                    var mshf = new MshFormat();
                    mshf.LineNumbers = options.DisplayLineNumbers;
                    mshf.Alternate = options.AlternateLineNumbers;
                    return HttpUtility.HtmlDecode(mshf.FormatCode(text));

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
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Contains("[/code]"))
            {
                text = regexCode2.Replace(text, new MatchEvaluator(CodeEvaluatorSimple));
                text = regexCode2.Replace(text, "$1");
            }
            return text;
        }

        #endregion
    }
}

