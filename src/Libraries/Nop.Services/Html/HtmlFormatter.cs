using System.Net;
using System.Text.RegularExpressions;

namespace Nop.Services.Html
{
    /// <summary>
    /// Represents the HTML formatter implementation
    /// </summary>
    public partial class HtmlFormatter : IHtmlFormatter
    {
        #region Fields

        protected readonly IBBCodeHelper _bbCodeHelper;

        #endregion

        #region Ctor

        public HtmlFormatter(IBBCodeHelper bbCodeHelper)
        {
            _bbCodeHelper = bbCodeHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Ensure only allowed HTML tags
        /// </summary>
        /// <param name="text">Text to check</param>
        /// <returns>True - if the text contains only valid tags, false otherwise</returns>
        protected static string EnsureOnlyAllowedHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            const string allowedTags = "br,hr,b,i,u,a,div,ol,ul,li,blockquote,img,span,p,em,strong,font,pre,h1,h2,h3,h4,h5,h6,address,cite";

            var m = Regex.Matches(text, "<.*?>", RegexOptions.IgnoreCase);
            
            for (var i = m.Count - 1; i >= 0; i--)
            {
                var tag = text[(m[i].Index + 1)..(m[i].Index + m[i].Length)].Trim().ToLower();

                if (!IsValidTag(tag, allowedTags))
                    text = text.Remove(m[i].Index, m[i].Length);
            }

            return text;
        }

        /// <summary>
        /// Indicates whether the HTML tag is valid
        /// </summary>
        /// <param name="tag">HTMl tag to check</param>
        /// <param name="tags">List of valid tags</param>
        /// <returns>True - if the tag if valid, false otherwise</returns>
        protected static bool IsValidTag(string tag, string tags)
        {
            var allowedTags = tags.Split(',');
            if (tag.Contains("javascript", StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (tag.Contains("vbscript", StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (tag.Contains("onclick", StringComparison.InvariantCultureIgnoreCase))
                return false;

            var endChars = new[] { ' ', '>', '/', '\t' };

            var pos = tag.IndexOfAny(endChars, 1);
            if (pos > 0)
                tag = tag[0..pos];
            if (tag[0] == '/')
                tag = tag[1..^0];

            foreach (var aTag in allowedTags)
            {
                if (tag == aTag)
                    return true;
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
        public virtual string FormatText(string text, bool stripTags,
            bool convertPlainTextToHtml, bool allowHtml,
            bool allowBBCode, bool resolveLinks, bool addNoFollowTag)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            try
            {
                if (stripTags)
                {
                    text = StripTags(text);
                }

                text = allowHtml ? EnsureOnlyAllowedHtml(text) : WebUtility.HtmlEncode(text);

                if (convertPlainTextToHtml)
                {
                    text = ConvertPlainTextToHtml(text);
                }

                if (allowBBCode)
                {
                    text = _bbCodeHelper.FormatText(text, true, true, true, true, true, true, true);
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
        /// Replace anchor text (remove a tag from the following URL <a href="http://example.com">Name</a> and output only the string "Name")
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Text</returns>
        public virtual string ReplaceAnchorTags(string text)
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
        public virtual string ConvertPlainTextToHtml(string text)
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
}