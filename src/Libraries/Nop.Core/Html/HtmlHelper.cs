using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Nop.Core.Html
{
    /// <summary>
    /// Represents a HTML helper
    /// </summary>
    public partial class HtmlHelper
    {
        #region Fields
        private readonly static Regex paragraphStartRegex = new Regex("<p>", RegexOptions.IgnoreCase);
        private readonly static Regex paragraphEndRegex = new Regex("</p>", RegexOptions.IgnoreCase);
        //private static Regex ampRegex = new Regex("&(?!(?:#[0-9]{2,4};|[a-z0-9]+;))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Utilities

        private static string EnsureOnlyAllowedHtml(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            const string allowedTags = "br,hr,b,i,u,a,div,ol,ul,li,blockquote,img,span,p,em,strong,font,pre,h1,h2,h3,h4,h5,h6,address,cite";

            var m = Regex.Matches(text, "<.*?>", RegexOptions.IgnoreCase);
            for (int i = m.Count - 1; i >= 0; i--)
            {
                string tag = text.Substring(m[i].Index + 1, m[i].Length - 1).Trim().ToLower();

                if (!IsValidTag(tag, allowedTags))
                {
                    text = text.Remove(m[i].Index, m[i].Length);
                }
            }

            return text;
        }

        private static bool IsValidTag(string tag, string tags)
        {
            string[] allowedTags = tags.Split(',');
            if (tag.IndexOf("javascript") >= 0) return false;
            if (tag.IndexOf("vbscript") >= 0) return false;
            if (tag.IndexOf("onclick") >= 0) return false;

            var endchars = new [] { ' ', '>', '/', '\t' };

            int pos = tag.IndexOfAny(endchars, 1);
            if (pos > 0) tag = tag.Substring(0, pos);
            if (tag[0] == '/') tag = tag.Substring(1);

            foreach (string aTag in allowedTags)
            {
                if (tag == aTag) return true;
            }

            return false;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Formats the text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="stripTags">A value indicating whether to strip tags</param>
        /// <param name="convertPlainTextToHtml">A value indicating whether HTML is allowed</param>
        /// <param name="allowHtml">A value indicating whether HTML is allowed</param>
        /// <param name="allowBBCode">A value indicating whether BBCode is allowed</param>
        /// <param name="resolveLinks">A value indicating whether to resolve links</param>
        /// <param name="addNoFollowTag">A value indicating whether to add "noFollow" tag</param>
        /// <returns>Formatted text</returns>
        public static string FormatText(string text, bool stripTags,
            bool convertPlainTextToHtml, bool allowHtml, 
            bool allowBBCode, bool resolveLinks, bool addNoFollowTag)
        {

            if (String.IsNullOrEmpty(text))
                return string.Empty;

            try
            {
                if (stripTags)
                {
                    text = StripTags(text);
                }

                text = allowHtml ? EnsureOnlyAllowedHtml(text) : HttpUtility.HtmlEncode(text);

                if (convertPlainTextToHtml)
                {
                    text = ConvertPlainTextToHtml(text);
                }

                if (allowBBCode)
                {
                    text = BBCodeHelper.FormatText(text, true, true, true, true, true, true, true);
                }

                if (resolveLinks)
                {
                    text = ResolveLinksHelper.FormatText(text);
                }

                if (addNoFollowTag)
                {
                    //add noFollow tag. not implemented
                }
            }
            catch (Exception exc)
            {
                text = string.Format("Text cannot be formatted. Error: {0}", exc.Message);
            }
            return text;
        }
        
        /// <summary>
        /// Strips tags
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string StripTags(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = Regex.Replace(text, @"(>)(\r|\n)*(<)", "><");
            text = Regex.Replace(text, "(<[^>]*>)([^<]*)", "$2");
            text = Regex.Replace(text, "(&#x?[0-9]{2,4};|&quot;|&amp;|&nbsp;|&lt;|&gt;|&euro;|&copy;|&reg;|&permil;|&Dagger;|&dagger;|&lsaquo;|&rsaquo;|&bdquo;|&rdquo;|&ldquo;|&sbquo;|&rsquo;|&lsquo;|&mdash;|&ndash;|&rlm;|&lrm;|&zwj;|&zwnj;|&thinsp;|&emsp;|&ensp;|&tilde;|&circ;|&Yuml;|&scaron;|&Scaron;)", "@");

            return text;
        }

        /// <summary>
        /// replace anchor text (remove a tag from the following url <a href="http://example.com">Name</a> and output only the string "Name")
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Text</returns>
        public static string ReplaceAnchorTags(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = Regex.Replace(text, @"<a\b[^>]+>([^<]*(?:(?!</a)<[^<]*)*)</a>", "$1", RegexOptions.IgnoreCase);
            return text;
        }

        /// <summary>
        /// Converts plain text to HTML
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string ConvertPlainTextToHtml(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = text.Replace("\r\n", "<br />");
            text = text.Replace("\r", "<br />");
            text = text.Replace("\n", "<br />");
            text = text.Replace("\t", "&nbsp;&nbsp;");
            text = text.Replace("  ", "&nbsp;&nbsp;");

            return text;
        }
        
        /// <summary>
        /// Converts HTML to plain text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="decode">A value indicating whether to decode text</param>
        /// <param name="replaceAnchorTags">A value indicating whether to replace anchor text (remove a tag from the following url <a href="http://example.com">Name</a> and output only the string "Name")</param>
        /// <returns>Formatted text</returns>
        public static string ConvertHtmlToPlainText(string text,
            bool decode = false, bool replaceAnchorTags = false)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            if (decode)
                text = HttpUtility.HtmlDecode(text);

            text = text.Replace("<br>", "\n");
            text = text.Replace("<br >", "\n");
            text = text.Replace("<br />", "\n");
            text = text.Replace("&nbsp;&nbsp;", "\t");
            text = text.Replace("&nbsp;&nbsp;", "  ");

            if (replaceAnchorTags)
                text = ReplaceAnchorTags(text);

            return text;
        }

        /// <summary>
        /// Converts text to paragraph
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string ConvertPlainTextToParagraph(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = paragraphStartRegex.Replace(text, string.Empty);
            text = paragraphEndRegex.Replace(text, "\n");
            text = text.Replace("\r\n", "\n").Replace("\r", "\n");
            text = text + "\n\n";
            text = text.Replace("\n\n", "\n");
            var strArray = text.Split(new [] { '\n' });
            var builder = new StringBuilder();
            foreach (string str in strArray)
            {
                if ((str != null) && (str.Trim().Length > 0))
                {
                    builder.AppendFormat("<p>{0}</p>\n", str);
                }
            }
            return builder.ToString();
        }
        #endregion
    }
}
