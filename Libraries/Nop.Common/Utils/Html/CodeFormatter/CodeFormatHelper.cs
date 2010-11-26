//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): BlogEngine.NET (http://www.dotnetblogengine.net/). 
//------------------------------------------------------------------------------


using System;
using System.Text.RegularExpressions;
using System.Web;

namespace NopSolutions.NopCommerce.Common.Utils.Html.CodeFormatter
{
	/// <summary>
	/// Represents a code format helper
	/// </summary>
    public partial class CodeFormatHelper
    {
        #region Fields
        //private static Regex regexCode1 = new Regex(@"(?<begin>\[code:(?<lang>.*?)(?:;ln=(?<linenumbers>(?:on|off)))?(?:;alt=(?<altlinenumbers>(?:on|off)))?(?:;(?<title>.*?))?\])(?<code>.*?)(?<end>\[/code\])", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static Regex regexHtml = new Regex("<[^>]*>", RegexOptions.Compiled);
        private static readonly Regex regexCode2 = new Regex(@"\[code\](?<inner>(.*?))\[/code\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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

            HighlightOptions options = new HighlightOptions();

            options.Language = match.Groups["lang"].Value;
            options.Code = match.Groups["code"].Value;
            options.DisplayLineNumbers = match.Groups["linenumbers"].Value == "on" ? true : false;
            options.Title = match.Groups["title"].Value;
            options.AlternateLineNumbers = match.Groups["altlinenumbers"].Value == "on" ? true : false;

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

            HighlightOptions options = new HighlightOptions();

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
                    CSharpFormat csf = new CSharpFormat();
                    csf.LineNumbers = options.DisplayLineNumbers;
                    csf.Alternate = options.AlternateLineNumbers;
                    return HttpUtility.HtmlDecode(csf.FormatCode(text));

                case "vb":
                    VisualBasicFormat vbf = new VisualBasicFormat();
                    vbf.LineNumbers = options.DisplayLineNumbers;
                    vbf.Alternate = options.AlternateLineNumbers;
                    return vbf.FormatCode(text);

                case "js":
                    JavaScriptFormat jsf = new JavaScriptFormat();
                    jsf.LineNumbers = options.DisplayLineNumbers;
                    jsf.Alternate = options.AlternateLineNumbers;
                    return HttpUtility.HtmlDecode(jsf.FormatCode(text));

                case "html":
                    HtmlFormat htmlf = new HtmlFormat();
                    htmlf.LineNumbers = options.DisplayLineNumbers;
                    htmlf.Alternate = options.AlternateLineNumbers;
                    text = StripHtml(text).Trim();
                    string code = htmlf.FormatCode(HttpUtility.HtmlDecode(text)).Trim();
                    return code.Replace("\r\n", "<br />").Replace("\n", "<br />");

                case "xml":
                    HtmlFormat xmlf = new HtmlFormat();
                    xmlf.LineNumbers = options.DisplayLineNumbers;
                    xmlf.Alternate = options.AlternateLineNumbers;
                    text = text.Replace("<br />", "\r\n");
                    text = StripHtml(text).Trim();
                    string xml = xmlf.FormatCode(HttpUtility.HtmlDecode(text)).Trim();
                    return xml.Replace("\r\n", "<br />").Replace("\n", "<br />");

                case "tsql":
                    TsqlFormat tsqlf = new TsqlFormat();
                    tsqlf.LineNumbers = options.DisplayLineNumbers;
                    tsqlf.Alternate = options.AlternateLineNumbers;
                    return HttpUtility.HtmlDecode(tsqlf.FormatCode(text));

                case "msh":
                    MshFormat mshf = new MshFormat();
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

