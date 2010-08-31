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
// Contributor(s): Danny Battison (gabehabe@googlemail.com). 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using NopSolutions.NopCommerce.Common.Utils.Html.CodeFormatter;
using System.Data;

namespace NopSolutions.NopCommerce.Common.Utils.Html
{
    /// <summary>
    /// Represents a BBCode helper
    /// </summary>
    public partial class BBCodeHelper
    {
        #region Fields
        private static readonly Regex regexBold = new Regex(@"\[b\](.+?)\[/b\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexItalic = new Regex(@"\[i\](.+?)\[/i\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexUnderLine = new Regex(@"\[u\](.+?)\[/u\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexUrl1 = new Regex(@"\[url\=([^\]]+)\]([^\]]+)\[/url\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexUrl2 = new Regex(@"\[url\](.+?)\[/url\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexQuote = new Regex(@"\[quote=(.+?)\](.+?)\[/quote\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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
        /// <returns>Formatted text</returns>
        public static string FormatText(string text, bool replaceBold, bool replaceItalic,
            bool replaceUnderline, bool replaceUrl, bool replaceCode, bool replaceQuote)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            if (replaceBold)
            {
                // format the bold tags: [b][/b]
                // becomes: <strong></strong>
                text = regexBold.Replace(text, "<strong>$1</strong>");
            }

            if (replaceItalic)
            {
                // format the italic tags: [i][/i]
                // becomes: <em></em>
                text = regexItalic.Replace(text, "<em>$1</em>");
            }

            if (replaceUnderline)
            {
                // format the underline tags: [u][/u]
                // becomes: <u></u>
                text = regexUnderLine.Replace(text, "<u>$1</u>");
            }

            if (replaceUrl)
            {
                // format the url tags: [url=http://www.nopCommerce.com]my site[/url]
                // becomes: <a href="http://www.nopCommerce.com">my site</a>
                text = regexUrl1.Replace(text, "<a href=\"$1\" rel=\"nofollow\">$2</a>");

                // format the url tags: [url]http://www.nopCommerce.com[/url]
                // becomes: <a href="http://www.nopCommerce.com">http://www.nopCommerce.com</a>
                text = regexUrl2.Replace(text, "<a href=\"$1\" rel=\"nofollow\">$1</a>");
            }

            if (replaceQuote)
            {
                while (regexQuote.IsMatch(text))
                    text = regexQuote.Replace(text, "<b>$1 wrote:</b><div class=\"quote\">$2</div>");
            }

            if (replaceCode)
            {
                text = CodeFormatHelper.FormatTextSimple(text);
            }

            return text;
        }

        /// <summary>
        /// Removes all quotes from string
        /// </summary>
        /// <param name="str">Source string</param>
        /// <returns>string</returns>
        public static string RemoveQuotes(string str)
        {
            str = Regex.Replace(str, @"\[quote=(.+?)\]", String.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\[/quote\]", String.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return str;
        }
        #endregion
    }
}
